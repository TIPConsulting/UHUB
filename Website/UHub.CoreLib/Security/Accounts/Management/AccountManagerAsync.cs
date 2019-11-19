using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SysSec = System.Web.Security;
using System.Web;
using UHub.CoreLib.RegExp;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Management;
using UHub.CoreLib.ClientFriendly;
using UHub.CoreLib.EmailInterop;
using UHub.CoreLib.EmailInterop.Templates;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Security.Accounts.Interfaces;
using UHub.CoreLib.Security.Authentication;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Security.Recovery.Management;
using UHub.CoreLib.Security.Accounts.DataInterop;

namespace UHub.CoreLib.Security.Accounts.Management
{
    /// <summary>
    /// Wrapper for UserWriter functionality.  Controls user account create/edit/delete functionality while also adding error callback functionality
    /// </summary>
    public partial class AccountManager
    {
        /// <summary>
        /// Try to create a new user in the CMS system
        /// </summary>
        /// <param name="UserEmail">New user email</param>
        /// <param name="UserPassword">New user password</param>
        /// <param name="AttemptAutoLogin">Should system automatically login user after creating account</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler">Args: new user object, auto login [T|F]</param>
        /// <returns>Status Flag</returns>
        public async Task<AcctCreateResultCode> TryCreateUserAsync(
            User NewUser,
            bool AttemptAutoLogin,
            Action<User, bool> SuccessHandler = null)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (NewUser == null)
            {
                return AcctCreateResultCode.UserInvalid;
            }


            Shared.TryCreate_HandleAttrTrim(ref NewUser);


            var attrValidation = Shared.TryCreate_ValidateUserAttrs(NewUser);
            if (attrValidation != 0)
            {
                return attrValidation;
            }

            var taskDoesEmailExist = UserReader.DoesUserExistAsync(NewUser.Email);
            //Validate user domain and school
            var domain = NewUser.Email.GetEmailDomain();
            var taskGetUserSchool = SchoolReader.TryGetSchoolByDomainAsync(domain);
            var taskDoesUsernameExist = UserReader.DoesUserExistAsync(NewUser.Username, domain);


            var pswdValidation = Shared.ValidateUserPswd(NewUser);
            if (pswdValidation != 0)
            {
                return (AcctCreateResultCode)pswdValidation;
            }


            //check for duplicate email
            try
            {
                if (await taskDoesEmailExist)
                {
                    return AcctCreateResultCode.EmailDuplicate;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("94553D4D-B1E3-45FC-A166-2A2E1D816276");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }


            //Check for valid school domain
            try
            {
                var tmpSchool = await taskGetUserSchool;
                if (tmpSchool == null || tmpSchool.ID == null)
                {
                    return AcctCreateResultCode.EmailDomainInvalid;
                }
                NewUser.SchoolID = tmpSchool.ID;
            }
            catch (Exception ex)
            {
                var exID = new Guid("1F60C82E-B33E-478D-BC7C-D796134C7990");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }
            var taskSchoolMajors = SchoolMajorReader.TryGetMajorsBySchoolAsync(NewUser.SchoolID.Value);


            //check for duplicate username
            try
            {
                if (await taskDoesUsernameExist)
                {
                    return AcctCreateResultCode.UsernameDuplicate;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("7798B041-235E-42DD-BFA3-A8FD3957A2AD");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }



            //check for valid major (chosen via dropdown)
            try
            {
                var major = NewUser.Major;
                var majorValidationSet = (await taskSchoolMajors)
                                            .Select(x => x.Name)
                                            .ToHashSet();

                if (!majorValidationSet.Contains(major))
                {
                    return AcctCreateResultCode.MajorInvalid;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("946FA6BE-5F49-472E-B021-D39F30D38650");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }


            Shared.TryCreate_HandleUserDefaults(ref NewUser);


            //create CMS user
            long? userID = null;
            try
            {
#pragma warning disable 612, 618
                userID = await UserWriter.CreateUserAsync(NewUser);
#pragma warning restore
            }
            catch (DuplicateNameException)
            {
                return AcctCreateResultCode.EmailDuplicate;
            }
            catch (ArgumentOutOfRangeException)
            {
                return AcctCreateResultCode.InvalidArgument;
            }
            catch (ArgumentNullException)
            {
                return AcctCreateResultCode.NullArgument;
            }
            catch (ArgumentException)
            {
                return AcctCreateResultCode.InvalidArgument;
            }
            catch (InvalidCastException)
            {
                return AcctCreateResultCode.InvalidArgumentType;
            }
            catch (InvalidOperationException)
            {
                return AcctCreateResultCode.InvalidOperation;
            }
            catch (AccessForbiddenException)
            {
                return AcctCreateResultCode.AccessDenied;
            }
            catch (EntityGoneException)
            {
                return AcctCreateResultCode.InvalidOperation;
            }
            catch (Exception ex)
            {
                var exID = new Guid("9206C3AE-215E-491D-9621-B7607A6AF91D");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }



            if (userID == null)
            {
                var exID = new Guid("6205BDC3-FDFF-4C2C-A98B-C30CA8950B06");
                await CoreFactory.Singleton.Logging.CreateFailureLogAsync("UserID Empty", exID);
                return AcctCreateResultCode.UnknownError;
            }


            //try to create password
            //if failed, then purge the remaining CMS account components so user can try again


#pragma warning disable 612, 618
            var isPswdChanged = await TryDoPasswordWorkAsync(userID.Value, NewUser.Password);
            if (!isPswdChanged)
            {
                await UserWriter.TryPurgeUserAsync((long)userID);
                return AcctCreateResultCode.UnknownError;
            }
#pragma warning restore



            //get cms user
            var taskConfirmToken = UserAccountReader.GetConfirmTokenAsync(userID.Value);
            User cmsUser = null;
            try
            {
                cmsUser = UserReader.GetUser(userID.Value);
            }
            catch (Exception ex)
            {
                var exID = new Guid("0D703D60-C47B-4475-AE60-6BBA1704A7F3");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }


            if (cmsUser == null)
            {
                var exID = new Guid("7A327386-FC5A-423B-9B72-3D5BC2FF0729");
                await CoreFactory.Singleton.Logging.CreateFailureLogAsync(exID);
                return AcctCreateResultCode.UnknownError;
            }


            //attempt autologin
            //autoconfirm user -> auto login
            bool canLogin =
                AttemptAutoLogin
                && CoreFactory.Singleton.Properties.AutoConfirmNewAccounts
                && CoreFactory.Singleton.Properties.AutoApproveNewAccounts;


            if (canLogin)
            {
                try
                {
                    //set login
                    CoreFactory.Singleton.Auth.TrySetClientAuthToken(NewUser.Email, NewUser.Password, false);

                    //TODO: log to DB
                    //CoreFactory.Singleton.Logging.CreateDBActivityLog(ActivityLogTypes.UserLogin);
                }
                catch
                {
                    //account creating, but auto login failed
                    //should only ever occur during tests
                    var exID = new Guid("F475C097-FC34-45E8-BEEF-EF37C9BC48B0");
                    await CoreFactory.Singleton.Logging.CreateWarningLogAsync("Test Failure - Ignore", exID);
                    canLogin = false;
                }
            }
            else if (!CoreFactory.Singleton.Properties.AutoConfirmNewAccounts)
            {
                IUserConfirmToken confirmToken = null;
                try
                {
                    confirmToken = await taskConfirmToken;
                }
                catch (Exception ex)
                {
                    var exID = new Guid("F22EAFDD-5728-4DF6-9D6E-015DF260B3D6");
                    await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                    return AcctCreateResultCode.UnknownError;
                }
                if (confirmToken == null)
                {
                    var exID = new Guid("508B5155-E218-4DB9-8348-789EBC588B05");
                    await CoreFactory.Singleton.Logging.CreateFailureLogAsync(exID);
                    return AcctCreateResultCode.UnknownError;
                }


                //get full confirmation URL
                var siteName = CoreFactory.Singleton.Properties.SiteFriendlyName;
                var confirmUrlAdj = GetConfirmationURL(confirmToken);

                var msg = new EmailMessage_ConfirmAcct($"{siteName} Account Confirmation", siteName, NewUser.Email)
                {
                    ConfirmationURL = confirmUrlAdj
                };

                var emailSendStatus = await CoreFactory.Singleton.Mail.TrySendMessageAsync(msg);
                if (emailSendStatus != EmailResultCode.Success)
                {
                    var exID = new Guid("0ADC52B0-89BB-4346-84F3-1F6CAC63DACF");
                    await CoreFactory.Singleton.Logging.CreateErrorLogAsync("Email Failed to Send", exID);
                    return AcctCreateResultCode.UnknownError;
                }
            }



            SuccessHandler?.Invoke(cmsUser, canLogin);
            return AcctCreateResultCode.Success;
        }



        /// <summary>
        /// Attempt to update user attributes in DB
        /// </summary>
        /// <param name="CmsUser"></param>
        /// <returns></returns>
        public async Task<AcctUpdateResultCode> TryUpdateUserAsync(User CmsUser)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (CmsUser == null)
            {
                return AcctUpdateResultCode.UserInvalid;
            }

            var taskMajorSet = SchoolMajorReader.TryGetMajorsBySchoolAsync(CmsUser.SchoolID.Value);


            Shared.TryCreate_HandleAttrTrim(ref CmsUser);


            var attrValidation = Shared.TryCreate_ValidateUserAttrs(CmsUser);
            if (attrValidation != 0)
            {
                return (AcctUpdateResultCode)attrValidation;
            }


            //check for valid major (chosen via dropdown)
            try
            {
                var major = CmsUser.Major;
                var majorValidationSet = (await taskMajorSet)
                                                .Select(x => x.Name)
                                                .ToHashSet();

                if (!majorValidationSet.Contains(major))
                {
                    return AcctUpdateResultCode.MajorInvalid;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("91DAE5AD-81CD-47E7-9118-FF9DD8A3F5BF");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctUpdateResultCode.UnknownError;
            }


            try
            {
                //create CMS user
#pragma warning disable 612, 618
                await UserWriter.UpdateUserAsync(CmsUser);
#pragma warning restore

            }
            catch (ArgumentOutOfRangeException)
            {
                return AcctUpdateResultCode.InvalidArgument;
            }
            catch (ArgumentNullException)
            {
                return AcctUpdateResultCode.NullArgument;
            }
            catch (ArgumentException)
            {
                return AcctUpdateResultCode.InvalidArgument;
            }
            catch (InvalidCastException)
            {
                return AcctUpdateResultCode.InvalidArgumentType;
            }
            catch (InvalidOperationException)
            {
                return AcctUpdateResultCode.InvalidOperation;
            }
            catch (AccessForbiddenException)
            {
                return AcctUpdateResultCode.AccessDenied;
            }
            catch (EntityGoneException)
            {
                return AcctUpdateResultCode.InvalidOperation;
            }
            catch (Exception ex)
            {
                var exID = new Guid("2345C8EC-0D17-4F8C-B7C8-6CAE5580176E");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctUpdateResultCode.UnknownError;
            }



            return AcctUpdateResultCode.Success;
        }



        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        /// <exception cref="ArgumentException"></exception>
        public async Task<AcctConfirmResultCode> TryConfirmUserAsync(string RefUID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (RefUID.IsEmpty())
            {
                return AcctConfirmResultCode.RefUIDEmpty;
            }

            if (!RefUID.RgxIsMatch(RgxPtrns.EntUser.REF_UID_B))
            {
                return AcctConfirmResultCode.RefUIDInvalid;
            }


            //get Today - ConfLifespan 
            //Determine the earliest date that a confirmation email could be created and still be valid
            //If ConfLifespan is 0, then allow infinite time
            DateTimeOffset minCreatedDate = DateTimeOffset.MinValue;
            var confLifespan = CoreFactory.Singleton.Properties.AcctConfirmLifespan;
            if (confLifespan != TimeSpan.Zero)
            {
                minCreatedDate = DateTimeOffset.UtcNow - confLifespan;
            }

            try
            {
#pragma warning disable 612, 618
                var isValid = await UserWriter.ConfirmUserAsync(RefUID, minCreatedDate);
#pragma warning restore


                if (isValid)
                {
                    return AcctConfirmResultCode.Success;
                }
                else
                {
                    return AcctConfirmResultCode.ConfirmTokenInvalid;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("1BFDCBB0-48F6-4CE1-8393-EE308281F321");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctConfirmResultCode.UnknownError;
            }
        }


        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="IsApproved">Approval Status</param>
        public async Task<bool> TryUpdateApprovalStatusAsync(long UserID, bool IsApproved)
        {
#pragma warning disable 612, 618
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                await UserWriter.TryUpdateUserApprovalAsync(UserID, IsApproved);
                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("FC19EE94-D93C-4198-BAE2-A857B5B2F0CD");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }
#pragma warning restore
        }



        /// <summary>
        /// Attempt to update the user token version
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public async Task<bool> TryUpdateUserVersionAsync(long UserID)
        {
            var version = SysSec.Membership.GeneratePassword(USER_VERSION_LEN, 0);
            //sterilize for token processing
            //TODO:
            version = version.Replace('|', '0');


#pragma warning disable 612, 618
            try
            {
                await UserWriter.UpdateUserVersionAsync(UserID, version);
                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("BD071E24-EDCB-488C-9BD6-69A27F5A97E4");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }
#pragma warning restore

        }



        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        public async Task<AcctPswdResultCode> TryUpdatePasswordAsync(
            string UserEmail,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context)
        {

            if (UserEmail.IsEmpty())
            {
                return AcctPswdResultCode.EmailEmpty;
            }

            UserEmail = UserEmail.Trim();

            long? ID = null;
            try
            {
                ID = UserReader.GetUserID(UserEmail);
            }
            catch (Exception ex)
            {
                var exID = new Guid("B124CF3B-0E9D-45B8-9051-31FC6E979D96");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctPswdResultCode.UnknownError;
            }

            if (ID == null)
            {
                return AcctPswdResultCode.UserInvalid;
            }

            return await TryUpdatePasswordAsync(
                ID.Value,
                OldPassword,
                NewPassword,
                DeviceLogout,
                Context);
        }

        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        public async Task<AcctPswdResultCode> TryUpdatePasswordAsync(
            long UserID,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var taskGetRequestedUser = UserReader.GetUserAsync(UserID);

            //check for valid OLD password
            var pswdValidation = Shared.ValidateUserPswd(OldPassword);
            if (pswdValidation != 0)
            {
                return pswdValidation;
            }
            //check for valid NEW password
            pswdValidation = Shared.ValidateUserPswd(NewPassword);
            if (pswdValidation != 0)
            {
                return pswdValidation;
            }


            //check to see if the new password is the same as the old password
            if (OldPassword == NewPassword)
            {
                return AcctPswdResultCode.PswdNotChanged;
            }


            try
            {

                var modUser = await taskGetRequestedUser;
                if (modUser == null || modUser.ID == null)
                {
                    return AcctPswdResultCode.UserInvalid;
                }


                var authResult = await CoreFactory.Singleton.Auth.TryAuthenticateUserAsync(modUser.Email, OldPassword);
                if (authResult != 0)
                {
                    return AcctPswdResultCode.LoginFailed;
                }

                //try to change password
                var isPswdChanged = await TryDoPasswordWorkAsync(modUser.ID.Value, NewPassword);
                if (!isPswdChanged)
                {
                    var exID = new Guid("BB36362F-0415-485F-B89D-A1D9CB920C8B");
                    await CoreFactory.Singleton.Logging.CreateFailureLogAsync(exID);
                    return AcctPswdResultCode.UnknownError;
                }



                //if everything worked, increment user version to force global re auth
                if (DeviceLogout)
                {
                    var versionResult = await TryUpdateUserVersionAsync(modUser.ID.Value);
                    if (!versionResult)
                    {
                        var exID = new Guid("3069253E-2B22-4B00-B9AA-2DBA12CADF6D");
                        await CoreFactory.Singleton.Logging.CreateFailureLogAsync(exID);
                        return AcctPswdResultCode.UnknownError;
                    }

                    //re auth current user to prevent lapse in service
                    await CoreFactory.Singleton.Auth.TrySetClientAuthTokenAsync(modUser.Email, NewPassword, false, Context);
                }


                //remove any recovery contexts
                var recoverContext = await TryGetActiveRecoveryContextAsync(modUser.ID.Value);
                if (recoverContext != null)
                {
                    await RecoveryManager.TryDeleteAsync(recoverContext);
                }


                return AcctPswdResultCode.Success;

            }
            catch (Exception ex)
            {
                var exID = new Guid("17A42823-0767-4BA4-9F24-6A868509558B");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctPswdResultCode.UnknownError;
            }
        }



        /// <summary>
        /// Attempt to recover account password using a recovery context ID and key
        /// </summary>
        /// <param name="RecoveryContextID"></param>
        /// <param name="RecoveryKey"></param>
        /// <param name="NewPassword"></param>
        /// <param name="DeviceLogout"></param>
        /// <param name="GeneralFailHandler"></param>
        /// <returns></returns>
        public async Task<AcctRecoveryResultCode> TryRecoverPasswordAsync(
            string RecoveryContextID,
            string RecoveryKey,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context)
        {
            //TODO: fix side channel attack vector that would allow attacker to bypass attempt limit with large number of simultaneous requests
            //Possible solution:
            //Use string interning + thread lock to prevent concurrent threads using the same ContextID
            //Each call would get a shared string reference using string.intern
            //This shared reference can be used as a lock object

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!CoreFactory.Singleton.Properties.EnablePswdRecovery)
            {
                return AcctRecoveryResultCode.RecoveryNotEnabled;
            }



            IUserRecoveryContext recoveryContext = null;
            try
            {
                recoveryContext = await UserReader.GetRecoveryContextAsync(RecoveryContextID);
            }
            catch (Exception ex)
            {
                var exID = new Guid("2391BC17-9B8D-41AF-BD64-57BFE47B08CC");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctRecoveryResultCode.UnknownError;
            }


            if (recoveryContext == null)
            {
                return AcctRecoveryResultCode.RecoveryContextInvalid;
            }


            var resultCode = RecoveryManager.ValidateRecoveryKey(recoveryContext, RecoveryKey);
            if (resultCode != 0)
            {
                try
                {
                    await RecoveryManager.TryIncrementAttemptCountAsync(recoveryContext);
                }
                catch (Exception ex)
                {
                    var exID = new Guid("DC21C490-16B6-4D64-9528-196FD42B4A32");
                    await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                }
                return resultCode;
            }


            var userID = recoveryContext.UserID;

            return await TryResetPasswordAsync(
                userID,
                NewPassword,
                DeviceLogout,
                Context);
        }





        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        public async Task<AcctRecoveryResultCode> TryResetPasswordAsync(
            string UserEmail,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (!CoreFactory.Singleton.Properties.EnablePswdRecovery)
            {
                return AcctRecoveryResultCode.RecoveryNotEnabled;
            }

            if (UserEmail.IsEmpty())
            {
                return AcctRecoveryResultCode.EmailEmpty;
            }

            UserEmail = UserEmail.Trim();


            long? ID = null;
            try
            {
                ID = await UserReader.GetUserIDAsync(UserEmail);
            }
            catch (Exception ex)
            {
                var exID = new Guid("C16CA5A3-B142-45FB-8337-F8FB8CB5F0B4");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctRecoveryResultCode.UnknownError;
            }

            if (ID == null)
            {
                return AcctRecoveryResultCode.UserInvalid;
            }

            return await TryResetPasswordAsync(
                ID.Value,
                NewPassword,
                DeviceLogout,
                Context);
        }

        /// <summary>
        /// Attempts to reset a user password.  System level function that overrides validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="NewPassword">New password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        public async Task<AcctRecoveryResultCode> TryResetPasswordAsync(
            long UserID,
            string NewPassword,
            bool DeviceLogout,
            HttpContext Context)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (!CoreFactory.Singleton.Properties.EnablePswdRecovery)
            {
                return AcctRecoveryResultCode.RecoveryNotEnabled;
            }


            var taskGetUser = UserReader.GetUserAsync(UserID);


            //check for valid password
            var pswdValidation = Shared.ValidateUserPswd(NewPassword);
            if (pswdValidation != 0)
            {
                return (AcctRecoveryResultCode)pswdValidation;
            }


            try
            {

                var modUser = await taskGetUser;
                if (modUser == null || modUser.ID == null)
                {
                    return AcctRecoveryResultCode.UserInvalid;
                }


                //try to change password
                var isPswdChanged = await TryDoPasswordWorkAsync(modUser.ID.Value, NewPassword);
                if (!isPswdChanged)
                {
                    return AcctRecoveryResultCode.UnknownError;
                }


                //if everything worked, increment user version to force global re auth
                var versionResult = await TryUpdateUserVersionAsync(modUser.ID.Value);
                if (!versionResult)
                {
                    return AcctRecoveryResultCode.UnknownError;
                }

                //re auth current user to prevent lapse in service
                try
                {
                    await CoreFactory.Singleton.Auth.TrySetClientAuthTokenAsync(modUser.Email, NewPassword, false, Context);
                }
                catch(Exception ex)
                {
                    var exID = new Guid("7420DAF1-AA96-4080-9C17-728190C89B60");
                    await CoreFactory.Singleton.Logging.CreateWarningLogAsync(ex, exID);
                }


                //remove any recovery contexts
                var recoverContext = await TryGetActiveRecoveryContextAsync(modUser.ID.Value);
                if (recoverContext != null)
                {
                    await RecoveryManager.TryDeleteAsync(recoverContext);
                }


                return AcctRecoveryResultCode.Success;
            }
            catch (Exception ex)
            {
                var exID = new Guid("347A406B-85D2-4C6D-B966-3B3D81120DC3");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctRecoveryResultCode.UnknownError;

            }
        }


        /// <summary>
        /// Attempt to get a user's active recovery context (if one exists)
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public async Task<IUserRecoveryContext> TryGetActiveRecoveryContextAsync(long UserID)
        {
            try
            {
                return await UserReader.GetRecoveryContextAsync(UserID);
            }
            catch (Exception ex)
            {
                var exID = new Guid("38B20395-27C9-4852-853E-55163968F93E");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<(AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey)> TryCreateUserRecoveryContextAsync(
            string UserEmail,
            bool IsOptional)
        {
            //check for valid email format
            if (!Validators.IsValidEmail(UserEmail))
            {
                return (AcctRecoveryResultCode.EmailInvalid, null, null);
            }


            long? id = null;
            try
            {
                id = await UserReader.GetUserIDAsync(UserEmail);
            }
            catch (Exception ex)
            {
                var exID = new Guid("A37237C0-EE53-4B52-92E7-ADDEC84CEF06");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return (AcctRecoveryResultCode.UnknownError, null, null);
            }


            if (id == null)
            {
                return (AcctRecoveryResultCode.UserInvalid, null, null);
            }


            return await TryCreateUserRecoveryContextAsync(
                id.Value,
                IsOptional);
        }


        /// <summary>
        /// Create a user password recovery context. Allows users to create a new password if they forget their old password.  Can be used to force a user to reset their password by setting [IsOptional=TRUE]
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="IsOptional">Specify whether or not user will be forced to update password</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<(AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey)> TryCreateUserRecoveryContextAsync(
            long UserID,
            bool IsOptional)
        {
            try
            {
                var cmsUser = await UserReader.GetUserAsync(UserID);

                if (cmsUser == null || cmsUser.ID == null)
                {
                    return (AcctRecoveryResultCode.UserInvalid, null, null);
                }


                //prevent a new optional recovery context from being made if the user already has an active forced reset
                //a new recovery context would override the prior and allow a bypass
                //Allow user to create new context in case the previous was lost
                var recoveryContext = await UserReader.GetRecoveryContextAsync(cmsUser.ID.Value);
                if (recoveryContext != null && !recoveryContext.IsOptional)
                {
                    IsOptional = false;
                }


                var hashType = CoreFactory.Singleton.Properties.PswdHashType;
                string recoveryKey = SysSec.Membership.GeneratePassword(R_KEY_LEN, 5);
                string hashedKey = recoveryKey.GetCryptoHash(hashType);

#pragma warning disable 612, 618
                //will attempt to delete all old recovery contexts and create a new one
                //any failures will result in system state rollback
                var context = await UserWriter.CreateRecoveryContextAsync(UserID, hashedKey, IsOptional);
#pragma warning restore


                if (context == null)
                {
                    var exID = new Guid("492D2F3F-9727-46C7-9ECC-E37D975E909E");
                    await CoreFactory.Singleton.Logging.CreateFailureLogAsync("Recovery Context Not Found", exID);
                    return (AcctRecoveryResultCode.UnknownError, null, null);
                }


                return (AcctRecoveryResultCode.Success, context, recoveryKey);
            }
            catch (Exception ex)
            {
                var exID = new Guid("1DB703FD-5D16-47A8-8283-52836AD9B9FE");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return (AcctRecoveryResultCode.UserInvalid, null, null);
            }

        }



        /// <summary>
        /// Delete user by ID.
        /// </summary>
        /// <param name="UserID"></param>
        public async Task<bool> TryDeleteUserAsync(long UserID)
        {
            var modUser = await UserReader.GetUserAsync(UserID);
            return await _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user by Email
        /// </summary>
        /// <param name="Email"></param>
        public async Task<bool> TryDeleteUserAsync(string Email)
        {
            var modUser = await UserReader.GetUserAsync(Email);
            return await _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user by Username and Domain
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Domain"></param>
        public async Task<bool> TryDeleteUserAsync(string Username, string Domain)
        {
            var modUser = UserReader.GetUser(Username, Domain);
            return await _deleteUserAsync(modUser);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="CmsUser"></param>
        private async Task<bool> _deleteUserAsync(User CmsUser)
        {
#pragma warning disable 612, 618
            try
            {
                if (CmsUser == null || CmsUser.ID == null)
                {
                    return false;
                }


                await TryUpdateUserVersionAsync(CmsUser.ID.Value);
                await UserWriter.DeleteUserAsync(CmsUser.ID.Value);

                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("7AA4C25A-458D-4184-A50A-C85779E5DDAF");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }
#pragma warning restore
        }







        /// <summary>
        /// Handle password hash/write logic
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserPswd"></param>
        /// <returns></returns>
        private static async Task<bool> TryDoPasswordWorkAsync(long UserID, string UserPswd)
        {
#pragma warning disable 612, 618
            var salt = SysSec.Membership.GeneratePassword(SALT_LEN, 0);
            string pswdHash = null;
            try
            {
                var hashType = CoreFactory.Singleton.Properties.PswdHashType;
                pswdHash = UserPswd.GetCryptoHash(hashType, salt);
            }
            catch (Exception ex)
            {
                var exID = new Guid("2B4D3D70-9B71-43B5-960C-6D17CB24A3EE");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }
            if (pswdHash.IsEmpty())
            {
                var exID = new Guid("0073B441-46E3-4464-8F0A-9DF64C128EE7");
                await CoreFactory.Singleton.Logging.CreateFailureLogAsync("Pswd Not Set", exID);
                return false;
            }
            try
            {
                //SET DB PASSWORD
                await UserWriter.UpdateUserPasswordAsync(UserID, pswdHash, salt);
            }
            catch (Exception ex)
            {
                var exID = new Guid("D5E0D72C-057E-4B6A-9347-0924A69A7684");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }

            return true;
#pragma warning restore
        }


    }
}
