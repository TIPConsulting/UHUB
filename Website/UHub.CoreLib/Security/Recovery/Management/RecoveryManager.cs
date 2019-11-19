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
        public static AcctRecoveryResultCode TryIncrementAttemptCount(IUserRecoveryContext RecoverContext)
        {
            if (RecoverContext == null)
            {
                return AcctRecoveryResultCode.NullArgument;
            }


#pragma warning disable 612, 618
            if (RecoverContext.AttemptCount >= CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                TryDelete(RecoverContext);
                return AcctRecoveryResultCode.RecoveryContextDestroyed;
            }

            //Forced reset does not respect attempt count
            //But no error should be reported
            if (!RecoverContext.IsOptional)
            {
                return AcctRecoveryResultCode.Success;
            }


            RecoverContext.AttemptCount++;
            try
            {
                UserWriter.LogFailedRecoveryContextAttempt(RecoverContext.RecoveryID);
                return AcctRecoveryResultCode.Success;
            }
            catch (Exception ex)
            {
                var exID = new Guid("320E32EC-416F-4627-ADA1-D38EA201FCF0");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return AcctRecoveryResultCode.UnknownError;
            }
#pragma warning restore
        }



        /// <summary>
        /// Delete this recovery context from the DB
        /// </summary>
        public static bool TryDelete(IUserRecoveryContext RecoverContext)
        {
            if(RecoverContext == null)
            {
                return false;
            }

#pragma warning disable 612, 618
            try
            {
                UserWriter.DeleteRecoveryContext(RecoverContext.RecoveryID);
                return true;
            }
            catch (Exception ex)
            {
                var exID = new Guid("AEB010D9-AF61-4108-9C5F-448CCAFE9EA8");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return false;
            }
#pragma warning restore
        }

    }
}
