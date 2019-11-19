using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SysSec = System.Web.Security;
using UHub.CoreLib.RegExp;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Management;
using UHub.CoreLib.ClientFriendly;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Security.Accounts.Interfaces;
using UHub.CoreLib.Security.Authentication;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using UHub.CoreLib.EmailInterop;
using UHub.CoreLib.EmailInterop.Templates;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Security.Recovery.Management;
using UHub.CoreLib.Security.Accounts.DataInterop;

namespace UHub.CoreLib.Security.Accounts.Management
{
    /// <summary>
    /// Wrapper for UserWriter functionality.  Controls user account create/edit/delete functionality while also adding error callback functionality
    /// </summary>
    public sealed partial class AccountManager
    {

        /// <summary>
        /// Try to create a new user in the CMS system
        /// </summary>
        /// <param name="UserEmail">New user email</param>
        /// <param name="UserPassword">New user password</param>
        /// <param name="AttemptAutoLogin">Should system automatically login user after creating account</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="SuccessHandler">Args: new user object, auto login [T|F]</param>
        /// <returns>Status Flag</returns>
        public AcctCreateResultCode TryCreateUser(
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

            var pswdValidation = Shared.ValidateUserPswd(NewUser);
            if (pswdValidation != 0)
            {
                return (AcctCreateResultCode)pswdValidation;
            }


            //check for duplicate email
            try
            {
                if (UserReader.DoesUserExist(NewUser.Email))
                {
                    return AcctCreateResultCode.EmailDuplicate;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("AADC0919-B3BE-4C98-BB8E-FCB283E6F02E");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }


            //Validate user domain and school
            var domain = NewUser.Email.GetEmailDomain();

            try
            {
                var tmpSchool = SchoolReader.TryGetSchoolByDomain(domain);
                if (tmpSchool == null || tmpSchool.ID == null)
                {
                    return AcctCreateResultCode.EmailDomainInvalid;
                }
                NewUser.SchoolID = tmpSchool.ID;
            }
            catch (Exception ex)
            {
                var exID = new Guid("B48A52B7-CDCC-42DB-9CFF-CF60C3C7067A");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }


            //check for duplicate username
            try
            {
                if (UserReader.DoesUserExist(NewUser.Username, domain))
                {
                    return AcctCreateResultCode.UsernameDuplicate;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("84B9559A-4F55-4B00-9160-7C713D22D29C");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }


            //check for valid major (chosen via dropdown)
            try
            {
                var major = NewUser.Major;
                var majorValidationSet = SchoolMajorReader
                                                .TryGetMajorsBySchool(NewUser.SchoolID.Value)
                                                .Select(x => x.Name)
                                                .ToHashSet();

                if (!majorValidationSet.Contains(major))
                {
                    return AcctCreateResultCode.MajorInvalid;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("90820E9B-F97A-48F2-BEB2-C23E17DE22D0");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }



            Shared.TryCreate_HandleUserDefaults(ref NewUser);


            long? userID = null;
            try
            {
                //create CMS user
#pragma warning disable 612, 618
                userID = UserWriter.CreateUser(NewUser);
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
                var exID = new Guid("A983AFB8-920A-4850-9197-3DDE7F6E89CC");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }




            if (userID == null)
            {
                var exID = new Guid("2669D182-B402-44A4-A8ED-F507994E8C2D");
                CoreFactory.Singleton.Logging.CreateFailureLog("UserID Empty", exID);
                return AcctCreateResultCode.UnknownError;
            }


            //try to create password
            //if failed, then purge the remaining CMS account components so user can try again
#pragma warning disable 612, 618
            var isPswdChanged = TryDoPasswordWork(userID.Value, NewUser.Password);
            if (!isPswdChanged)
            {
                UserWriter.TryPurgeUser((long)userID);
                return AcctCreateResultCode.UnknownError;
            }
#pragma warning restore


            //get cms user
            User cmsUser = null;
            IUserConfirmToken confirmToken = null;
            try
            {
                cmsUser = UserReader.GetUser(userID.Value);
                confirmToken = UserAccountReader.GetConfirmToken(userID.Value);
            }
            catch (Exception ex)
            {
                var exID = new Guid("03AE31F9-5EA5-4456-9FB8-67765ECBC7D7");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctCreateResultCode.UnknownError;
            }

            if (cmsUser == null)
            {
                var exID = new Guid("FBDE65A7-281C-456B-9CCE-2F599D15F8F1");
                CoreFactory.Singleton.Logging.CreateFailureLog(exID);
                return AcctCreateResultCode.UnknownError;
            }

            if (confirmToken == null)
            {
                var exID = new Guid("33EC210D-5BE2-433B-8E39-FA5E3AD57312");
                CoreFactory.Singleton.Logging.CreateFailureLog("Confirmation Token Not Available", exID);
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
                    //CoreFactory.Singleton.Logging.CreateApiActionLogAsync(UsageLogType.UserLogin);
                }
                catch
                {
                    //account creating, but auto login failed
                    //should only ever occur during tests
                    var exID = new Guid("A275649B-AD89-43E3-8DE2-B81B6F47FE6A");
                    CoreFactory.Singleton.Logging.CreateWarningLog("Test Failure - Ignore", exID);
                    canLogin = false;
                }
            }
            else if (!CoreFactory.Singleton.Properties.AutoConfirmNewAccounts)
            {
                //get full confirmation URL
                var siteName = CoreFactory.Singleton.Properties.SiteFriendlyName;
                var confirmUrlAdj = GetConfirmationURL(confirmToken);


                var msg = new EmailMessage_ConfirmAcct($"{siteName} Account Confirmation", siteName, NewUser.Email)
                {
                    ConfirmationURL = confirmUrlAdj
                };

                var emailSendStatus = CoreFactory.Singleton.Mail.TrySendMessage(msg);
                if (emailSendStatus != EmailResultCode.Success)
                {
                    var exID = new Guid("AEBDE62B-31D5-4B48-8D26-3123AA5219A3");
                    CoreFactory.Singleton.Logging.CreateFailureLog("Email Failed to Send", exID);
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
        public AcctUpdateResultCode TryUpdateUser(User CmsUser)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (CmsUser == null)
            {
                return AcctUpdateResultCode.UserInvalid;
            }


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
                var majorValidationSet = SchoolMajorReader
                                                .TryGetMajorsBySchool(CmsUser.SchoolID.Value)
                                                .Select(x => x.Name)
                                                .ToHashSet();

                if (!majorValidationSet.Contains(major))
                {
                    return AcctUpdateResultCode.MajorInvalid;
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("194B1895-E9AB-44B3-A8CD-FCACCA4286FB");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctUpdateResultCode.UnknownError;
            }


            try
            {
                //create CMS user
#pragma warning disable 612, 618
                UserWriter.UpdateUser(CmsUser);
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
                var exID = new Guid("9E33013D-A3E9-4E96-A2A1-1E5462446125");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctUpdateResultCode.UnknownError;
            }



            return AcctUpdateResultCode.Success;
        }



        /// <summary>
        /// Confirm CMS user in DB
        /// </summary>
        /// <param name="RefUID">User reference UID</param>
        /// <exception cref="ArgumentException"></exception>
        public AcctConfirmResultCode TryConfirmUser(string RefUID)
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


            try
            {

                //get Today - ConfLifespan 
                //Determine the earliest date that a confirmation email could be created and still be valid
                //If ConfLifespan is 0, then allow infinite time
                DateTimeOffset minCreatedDate = DateTimeOffset.MinValue;
                var confLifespan = CoreFactory.Singleton.Properties.AcctConfirmLifespan;
                if (confLifespan != TimeSpan.Zero)
                {
                    minCreatedDate = DateTimeOffset.UtcNow - confLifespan;
                }

#pragma warning disable 612, 618
                var isValid = UserWriter.ConfirmUser(RefUID, minCreatedDate);
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
                var exID = new Guid("8E19A36A-57F3-463A-8F7C-BCFDE476D09A");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctConfirmResultCode.UnknownError;
            }
        }


        /// <summary>
        /// Update the approval status of a user
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="IsApproved">Approval Status</param>
        public bool TryUpdateApprovalStatus(long UserID, bool IsApproved)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

#pragma warning disable 612, 618
            try
            {
                UserWriter.UpdateUserApproval(UserID, IsApproved);
                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("D5D6FADE-9B32-40C3-A576-E1756884FCFD");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return false;
            }
#pragma warning restore
        }



        /// <summary>
        /// Attempt to update the user token version
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public bool TryUpdateUserVersion(long UserID)
        {
            var version = SysSec.Membership.GeneratePassword(USER_VERSION_LEN, 0);
            //sterilize for token processing
            //TODO:
            version = version.Replace('|', '0');


#pragma warning disable 612, 618
            try
            {

                UserWriter.UpdateUserVersion(UserID, version);
                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("695B957C-585E-42C2-95B2-1926257732B9");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
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
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        public AcctPswdResultCode TryUpdatePassword(
                string UserEmail,
                string OldPassword,
                string NewPassword,
                bool DeviceLogout)
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
                var exID = new Guid("FE482294-6197-4048-99DC-4DE4421823B9");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctPswdResultCode.UnknownError;
            }

            if (ID == null)
            {
                return AcctPswdResultCode.UserInvalid;
            }

            return TryUpdatePassword(
                ID.Value,
                OldPassword,
                NewPassword,
                DeviceLogout);
        }

        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserID">User ID</param>
        /// <param name="OldPassword">Old user password</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="DeviceLogout">If true, user will be logged out of all other devices</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns>Status flag</returns>
        public AcctPswdResultCode TryUpdatePassword(
            long UserID,
            string OldPassword,
            string NewPassword,
            bool DeviceLogout)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

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
                var modUser = UserReader.GetUser(UserID);
                if (modUser == null || modUser.ID == null)
                {
                    return AcctPswdResultCode.UserInvalid;
                }


                var authStatusCode = CoreFactory.Singleton.Auth.TryAuthenticateUser(modUser.Email, OldPassword);
                if (authStatusCode != 0)
                {
                    return AcctPswdResultCode.LoginFailed;
                }

                //try to change password
                var isPswdChanged = TryDoPasswordWork(modUser.ID.Value, NewPassword);
                if (!isPswdChanged)
                {
                    var exID = new Guid("FAF8E35E-A038-460D-893F-284ACB5B5FB3");
                    CoreFactory.Singleton.Logging.CreateFailureLog(exID);
                    return AcctPswdResultCode.UnknownError;
                }



                //if everything worked, increment user version to force global re auth
                if (DeviceLogout)
                {
                    var versionResult = TryUpdateUserVersion(modUser.ID.Value);
                    if (!versionResult)
                    {
                        var exID = new Guid("CB720EC3-43E3-460E-A77C-F1220333E1A9");
                        CoreFactory.Singleton.Logging.CreateFailureLog(exID);
                        return AcctPswdResultCode.UnknownError;
                    }

                    //re auth current user to prevent lapse in service
                    CoreFactory.Singleton.Auth.TrySetClientAuthToken(modUser.Email, NewPassword, false);
                }


                //remove any recovery contexts
                var recoverContext = TryGetActiveRecoveryContext(modUser.ID.Value);
                if (recoverContext != null)
                {
                    RecoveryManager.TryDelete(recoverContext);
                }


                return AcctPswdResultCode.Success;

            }
            catch (Exception ex)
            {
                var exID = new Guid("B9932471-7779-4710-A97E-BB1FA147A995");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
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
        public AcctRecoveryResultCode TryRecoverPassword(
            string RecoveryContextID,
            string RecoveryKey,
            string NewPassword,
            bool DeviceLogout)
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
                recoveryContext = UserReader.GetRecoveryContext(RecoveryContextID);
            }
            catch (Exception ex)
            {
                var exID = new Guid("8B2D0169-6AFA-46FB-807B-1CA3F88C90C2");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
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
                    RecoveryManager.TryIncrementAttemptCount(recoveryContext);

                }
                catch (Exception ex)
                {
                    var exID = new Guid("3F6883F6-3241-4B55-A7FB-7C2C8C43153A");
                    CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                }
                return resultCode;
            }


            var userID = recoveryContext.UserID;

            return TryResetPassword(
                userID,
                NewPassword,
                DeviceLogout);
        }




        /// <summary>
        /// Attempt to update a user password. Requires validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserEmail">User email</param>
        /// <param name="NewPassword">New user password</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        public AcctRecoveryResultCode TryResetPassword(
            string UserEmail,
            string NewPassword,
            bool DeviceLogout)
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
                ID = UserReader.GetUserID(UserEmail);
            }
            catch (Exception ex)
            {
                var exID = new Guid("C16CA5A3-B142-45FB-8337-F8FB8CB5F0B4");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctRecoveryResultCode.UnknownError;
            }

            if (ID == null)
            {
                return AcctRecoveryResultCode.UserInvalid;
            }

            return TryResetPassword(
                ID.Value,
                NewPassword,
                DeviceLogout);
        }

        /// <summary>
        /// Attempts to reset a user password.  System level function that overrides validation against the current password. User will be signed out of all locations upon completion
        /// </summary>
        /// <param name="UserUID">User UID</param>
        /// <param name="NewPassword">New password</param>
        /// <param name="ResultCode">Code returned to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <exception cref="SystemDisabledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Status flag</returns>
        public AcctRecoveryResultCode TryResetPassword(
            long UserID,
            string NewPassword,
            bool DeviceLogout)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (!CoreFactory.Singleton.Properties.EnablePswdRecovery)
            {
                return AcctRecoveryResultCode.RecoveryNotEnabled;
            }

            //check for valid password
            var pswdValidation = Shared.ValidateUserPswd(NewPassword);
            if (pswdValidation != 0)
            {
                return (AcctRecoveryResultCode)pswdValidation;
            }


            try
            {
                if (!UserReader.DoesUserExist(UserID))
                {
                    return AcctRecoveryResultCode.UserInvalid;
                }

                var modUser = UserReader.GetUser(UserID);
                if (modUser == null || modUser.ID == null)
                {
                    return AcctRecoveryResultCode.UserInvalid;
                }


                //try to change password
                var isPswdChanged = TryDoPasswordWork(modUser.ID.Value, NewPassword);
                if (!isPswdChanged)
                {
                    return AcctRecoveryResultCode.UnknownError;
                }


                //if everything worked, increment user version to force global re auth
                if (DeviceLogout)
                {
                    var versionResult = TryUpdateUserVersion(modUser.ID.Value);
                    if (!versionResult)
                    {
                        return AcctRecoveryResultCode.UnknownError;
                    }

                    //re auth current user to prevent lapse in service
                    CoreFactory.Singleton.Auth.TrySetClientAuthToken(modUser.Email, NewPassword, false);
                }



                //re auth current user to prevent lapse in service
                try
                {
                    CoreFactory.Singleton.Auth.TrySetClientAuthToken(modUser.Email, NewPassword, false);
                }
                catch (Exception ex)
                {
                    var exID = new Guid("6B11B5D6-6B8D-419C-B45D-0444EE23EA32");
                    CoreFactory.Singleton.Logging.CreateWarningLog(ex, exID);
                }

                //remove any recovery contexts
                var recoverContext = TryGetActiveRecoveryContext(modUser.ID.Value);
                if (recoverContext != null)
                {
                    RecoveryManager.TryDelete(recoverContext);
                }


                return AcctRecoveryResultCode.Success;


            }
            catch (Exception ex)
            {
                var exID = new Guid("8641F1E3-B29B-4CC1-ABA5-90B8693625EE");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctRecoveryResultCode.UnknownError;
            }
        }





        /// <summary>
        /// Attempt to get a user's active recovery context (if one exists)
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public IUserRecoveryContext TryGetActiveRecoveryContext(long UserID)
        {
            try
            {
                return UserReader.GetRecoveryContext(UserID);
            }
            catch (Exception ex)
            {
                var exID = new Guid("8CA322D4-FAD2-4C03-9290-47BED6C9C89B");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
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
        public (AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey) TryCreateUserRecoveryContext(
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
                id = UserReader.GetUserID(UserEmail);

            }
            catch (Exception ex)
            {
                var exID = new Guid("06ADB8E2-6A9C-44DD-B871-0713D51CD700");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return (AcctRecoveryResultCode.UnknownError, null, null);
            }


            if (id == null)
            {
                return (AcctRecoveryResultCode.UserInvalid, null, null);
            }


            return TryCreateUserRecoveryContext(
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
        public (AcctRecoveryResultCode ResultCode, IUserRecoveryContext RecoveryContext, string RecoveryKey) TryCreateUserRecoveryContext(
            long UserID,
            bool IsOptional)
        {
            try
            {
                var cmsUser = UserReader.GetUser(UserID);

                if (cmsUser == null || cmsUser.ID == null)
                {
                    return (AcctRecoveryResultCode.UserInvalid, null, null);
                }


                //prevent a new recovery context from being made if the user already has an active forced reset
                //a new recovery context would override the prior and allow a bypass
                //Allow user to create new context in case the previous was lost
                var recoveryContext = UserReader.GetRecoveryContext(cmsUser.ID.Value);
                if (recoveryContext != null && !recoveryContext.IsOptional)
                {
                    IsOptional = false;
                }

                string recoveryKey = SysSec.Membership.GeneratePassword(R_KEY_LEN, 5);
                string hashedKey = recoveryKey.GetCryptoHash(CoreFactory.Singleton.Properties.PswdHashType);


#pragma warning disable 612, 618
                //will attempt to delete all old recovery contexts and create a new one
                //any failures will result in system state rollback
                var context = UserWriter.CreateRecoveryContext(UserID, hashedKey, IsOptional);
#pragma warning restore


                if (context == null)
                {
                    var exID = new Guid("C60DF37E-2FC2-4CC3-96BC-C414F00ED4A2");
                    CoreFactory.Singleton.Logging.CreateFailureLog("Recovery Context Not Found", exID);
                    return (AcctRecoveryResultCode.UnknownError, null, null);
                }



                return (AcctRecoveryResultCode.Success, context, recoveryKey);
            }
            catch (Exception ex)
            {
                var exID = new Guid("84D8B77E-6C6E-4D2B-A37A-4433F5A4E102");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return (AcctRecoveryResultCode.UserInvalid, null, null);
            }

        }






        /// <summary>
        /// Delete user by ID.
        /// </summary>
        /// <param name="UserID"></param>
        public bool TryDeleteUser(long UserID)
        {
            var modUser = UserReader.GetUser(UserID);
            return _deleteUser(modUser);
        }

        /// <summary>
        /// Delete user by Email
        /// </summary>
        /// <param name="Email"></param>
        public bool TryDeleteUser(string Email)
        {
            var modUser = UserReader.GetUser(Email);
            return _deleteUser(modUser);
        }

        /// <summary>
        /// Delete user by Username and Domain
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Domain"></param>
        public bool TryDeleteUser(string Username, string Domain)
        {
            var modUser = UserReader.GetUser(Username, Domain);
            return _deleteUser(modUser);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="RequestedBy"></param>
        /// <param name="CmsUser"></param>
        private bool _deleteUser(User CmsUser)
        {
#pragma warning disable 612, 618
            try
            {
                if (CmsUser == null || CmsUser.ID == null)
                {
                    return false;
                }


                TryUpdateUserVersion(CmsUser.ID.Value);
                UserWriter.DeleteUser(CmsUser.ID.Value);

                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("54879964-C1BD-420C-B54D-BFBECFB71A52");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
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
        private static bool TryDoPasswordWork(long UserID, string UserPswd)
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
                var exID = new Guid("AA3E2DB3-5CCF-400D-8046-1D982E723F58");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return false;
            }
            if (pswdHash.IsEmpty())
            {
                var exID = new Guid("3C722062-9158-4FCB-8A9D-2D132B6784E5");
                CoreFactory.Singleton.Logging.CreateFailureLog("Pswd Not Set", exID);
                return false;
            }
            try
            {
                //SET DB PASSWORD
                UserWriter.UpdateUserPassword(UserID, pswdHash, salt);
            }
            catch (Exception ex)
            {
                var exID = new Guid("047BD588-A27E-4B49-AB5B-8157589AAA4B");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return false;
            }

            return true;
#pragma warning restore
        }



    }
}
