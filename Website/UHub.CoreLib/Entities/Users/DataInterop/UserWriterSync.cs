using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using static UHub.CoreLib.DataInterop.SqlConverters;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    internal static partial class UserWriter
    {
        /// <summary>
        /// Attempts to create a new CMS user in the database and returns the UserUID if successful
        /// </summary>
        /// <returns></returns>
        internal static long? CreateUser(User cmsUser)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }



            long? userID = SqlWorker.ExecScalar<long?>(
                _dbConn,
                "[dbo].[User_Create]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = HandleParamEmpty(cmsUser.SchoolID);
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Email);
                    cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = HandleParamEmpty(cmsUser.Username);
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Name);
                    cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.PhoneNumber);
                    cmd.Parameters.Add("@Major", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Major);
                    cmd.Parameters.Add("@Year", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Year);
                    cmd.Parameters.Add("@GradDate", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.ExpectedGradDate);
                    cmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Company);
                    cmd.Parameters.Add("@JobTitle", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.JobTitle);
                    cmd.Parameters.Add("@IsFinished", SqlDbType.Bit).Value = DBNull.Value;
                    cmd.Parameters.Add("@Version", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Version);
                    cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = HandleParamEmpty(cmsUser.IsApproved);
                    cmd.Parameters.Add("@IsConfirmed", SqlDbType.Bit).Value = HandleParamEmpty(cmsUser.IsConfirmed);
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = DBNull.Value;
                });

            return userID;
        }

        internal static void UpdateUser(User cmsUser)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (cmsUser.ID == null)
            {
                throw new ArgumentException("Invalid target user");
            }


            //run sproc
            SqlWorker.ExecNonQuery(
                _dbConn,
                "[dbo].[User_Update]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntID", SqlDbType.BigInt).Value = cmsUser.ID.Value;
                    //-------------------------------------------------
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Name);
                    cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.PhoneNumber);
                    cmd.Parameters.Add("@Major", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Major);
                    cmd.Parameters.Add("@Year", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Year);
                    cmd.Parameters.Add("@GradDate", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.ExpectedGradDate);
                    cmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.Company);
                    cmd.Parameters.Add("@JobTitle", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsUser.JobTitle);
                    cmd.Parameters.Add("@IsFinished", SqlDbType.Bit).Value = cmsUser.IsFinished;
                    //-------------------------------------------------
                    cmd.Parameters.Add("@ModifiedBy", SqlDbType.BigInt).Value = cmsUser.ModifiedBy;
                });

        }

        /// <summary>
        /// Confirm user account
        /// </summary>
        /// <param name="RefUID"></param>
        /// <param name="MinCreatedDate">If the confirmation token is create before this time, then it is expired</param>
        internal static bool ConfirmUser(string RefUID, DateTimeOffset MinCreatedDate)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecScalar<bool>(
                    _dbConn,
                    "[dbo].[User_UpdateConfirmFlag]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@RefUID", SqlDbType.NVarChar).Value = RefUID;
                        cmd.Parameters.Add("@MinCreatedDate", SqlDbType.DateTimeOffset).Value = MinCreatedDate;
                    });

        }

        /// <summary>
        /// Update user approval status
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="IsApproved"></param>
        internal static void UpdateUserApproval(long UserUID, bool IsApproved)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            SqlWorker.ExecNonQuery(
                _dbConn,
                "[dbo].[User_UpdateApprovalFlag]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserUID;
                    cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = IsApproved;
                });
        }

        /// <summary>
        /// Update the User's ticket version to invalidate existing login tickets
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="Version"></param>
        internal static void UpdateUserVersion(long UserID, string Version)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            SqlWorker.ExecNonQuery(
                _dbConn,
                "[dbo].[User_UpdateVersionByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    cmd.Parameters.Add("@Version", SqlDbType.NVarChar).Value = Version;
                });
        }


        internal static void UpdateUserPassword(string UserEmail, string PswdHash, string Salt)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            if (!Validators.IsValidEmail(UserEmail))
            {
                throw new ArgumentException();
            }


            SqlWorker.ExecNonQuery(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_UpdatePasswordByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = UserEmail;
                    cmd.Parameters.Add("@PswdHash", SqlDbType.NVarChar).Value = PswdHash;
                    cmd.Parameters.Add("@Salt", SqlDbType.NVarChar).Value = Salt;
                });
        }


        internal static void UpdateUserPassword(long UserID, string PswdHash, string Salt)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            SqlWorker.ExecNonQuery(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_UpdatePasswordByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    cmd.Parameters.Add("@PswdHash", SqlDbType.NVarChar).Value = PswdHash;
                    cmd.Parameters.Add("@Salt", SqlDbType.NVarChar).Value = Salt;
                });
        }



        /// <summary>
        /// Delete a specified user from the CMS DB
        /// </summary>
        /// <param name="RequestedBy">The user executing the delete command</param>
        /// <param name="UserUID">The user being deleted</param>
        internal static void DeleteUser(long UserID, long? DeletedBy = null)
        {

            SqlWorker.ExecNonQuery(
                _dbConn,
                "[dbo].[User_DeleteByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    cmd.Parameters.Add("@DeletedBy", SqlDbType.BigInt).Value = DeletedBy;
                });
        }
        /// <summary>
        /// Attempt to purge a user from the DB
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        internal static bool TryPurgeUser(long UserID)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_PurgeByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });

                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("84986584-BD1D-4DDB-8DAA-475A3BB874C1");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);

                return false;
            }
        }

        /// <summary>
        /// Attempt to purge a user from the DB
        /// </summary>
        /// <returns></returns>
        internal static bool TryPurgeUser(string Email)
        {
            try
            {
                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[User_PurgeByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                    });

                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("CDFB440C-2271-4DB4-BDE8-FC198D1FDACC");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);

                return false;
            }
        }

        /// <summary>
        /// Attempt to purge users that have not yet been confirmed and are older than the specified age tolerance
        /// </summary>
        internal static void TryPurgeUnconfirmedUsers(TimeSpan AcctAgeTolerance)
        {
            try
            {
                //users created before this date are subject to be purged
                //users created afgter this date will be ignored
                var minKeepDate = DateTimeOffset.UtcNow - AcctAgeTolerance;

                SqlWorker.ExecNonQuery(
                    _dbConn,
                    "[dbo].[Users_PurgeUnconfirmed]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@MinKeepDate", SqlDbType.UniqueIdentifier).Value = minKeepDate;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("6FE73439-372D-4935-92C9-912B47822499");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
            }
        }

    }
}
