using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security.Accounts;

namespace UHub.CoreLib.Security.Recovery.Management
{
    public static partial class RecoveryManager
    {
        /// <summary>
        /// Increment the attempt count in DB
        /// </summary>
        public static async Task<AcctRecoveryResultCode> TryIncrementAttemptCountAsync(IUserRecoveryContext RecoverContext)
        {
            if (RecoverContext == null)
            {
                return AcctRecoveryResultCode.NullArgument;
            }


#pragma warning disable 612, 618
            if (RecoverContext.AttemptCount >= CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                await TryDeleteAsync(RecoverContext);
                return AcctRecoveryResultCode.RecoveryContextDestroyed;
            }

            //Forced reset does not respect attempt count
            //But no error should be reported
            if (!RecoverContext.IsOptional)
            {
                return AcctRecoveryResultCode.Success;
            }


            if (RecoverContext.AttemptCount > CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                await TryDeleteAsync(RecoverContext);
                return AcctRecoveryResultCode.RecoveryContextDestroyed;
            }


            RecoverContext.AttemptCount++;
            try
            {
                await UserWriter.LogFailedRecoveryContextAttemptAsync(RecoverContext.RecoveryID);
                return AcctRecoveryResultCode.Success;
            }
            catch (Exception ex)
            {
                var exID = new Guid("1030F56D-0B6B-473C-9732-C828981FC332");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AcctRecoveryResultCode.UnknownError;
            }
#pragma warning restore
        }



        /// <summary>
        /// Delete this recovery context from the DB
        /// </summary>
        public static async Task<bool> TryDeleteAsync(IUserRecoveryContext RecoverContext)
        {
            if (RecoverContext == null)
            {
                return false;
            }


#pragma warning disable 612, 618

            try
            {
                await UserWriter.DeleteRecoveryContextAsync(RecoverContext.RecoveryID);
                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("042119E4-FB51-49A9-866B-F78DCFA0C567");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }
#pragma warning restore
        }

    }
}
