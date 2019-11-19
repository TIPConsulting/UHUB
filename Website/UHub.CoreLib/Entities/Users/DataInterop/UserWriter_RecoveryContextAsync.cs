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
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    internal static partial class UserWriter
    {

        /// <summary>
        /// Create an account recovery context for a specified user
        /// </summary>
        /// <param name="UserUID"></param>
        /// <param name="RecoveryKey"></param>
        /// <returns>RecoveryID for the recovery context</returns>
        internal static async Task<IUserRecoveryContext> CreateRecoveryContextAsync(long UserID, string RecoveryKey, bool IsOptional)
        {
            DateTimeOffset resetExpiration;

            if (IsOptional)
            {
                var span = CoreFactory.Singleton.Properties.AcctPswdRecoveryLifespan;
                if (span.Ticks == 0)
                {
                    resetExpiration = DateTimeOffset.MaxValue.Date;
                }
                else
                {
                    resetExpiration = DateTimeOffset.Now.Add(span);
                }
            }
            else
            {
                resetExpiration = DateTimeOffset.MaxValue.Date;
            }


            var temp = await SqlWorker.ExecEntityQueryAsync<UserRecoveryContext>(
                _dbConn,
                "[dbo].[User_CreateRecoveryContext]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = HandleParamEmpty(UserID);
                    cmd.Parameters.Add("@RecoveryKey", SqlDbType.NVarChar).Value = RecoveryKey;
                    cmd.Parameters.Add("@EffToDate", SqlDbType.DateTimeOffset).Value = resetExpiration;
                    cmd.Parameters.Add("@IsOptional", SqlDbType.Bit).Value = IsOptional;
                });


            return temp.SingleOrDefault();

        }

        internal static async Task LogFailedRecoveryContextAttemptAsync(string RecoveryID)
        {
            await SqlWorker.ExecNonQueryAsync(
                _dbConn,
                "[dbo].[User_LogFailedRecoveryContextAttempt]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                });
        }

        internal static async Task DeleteRecoveryContextAsync(string RecoveryID)
        {

            await SqlWorker.ExecNonQueryAsync(
                _dbConn,
                "[dbo].[User_DeleteUserRecoveryContextByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                });

        }

        internal static async Task TryDeleteRecoveryContextAsync(int UserID)
        {

            await SqlWorker.ExecNonQueryAsync(
                _dbConn,
                "[dbo].[User_DeleteUserRecoveryContextByUserID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });

        }

    }
}
