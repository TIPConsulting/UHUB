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
using UHub.CoreLib.Tools;


namespace UHub.CoreLib.Security.Recovery.Management
{
    public static partial class RecoveryManager
    {
        private const string RECOVER_URL_FORMAT = "{0}/{1}";
        public static string GetRecoveryURL(IUserRecoveryContext RecoverContext)
        {
            if (RecoverContext == null)
            {
                return "";
            }
            else if (RecoverContext.RecoveryID.IsEmpty())
            {
                return "";
            }
            else
            {

                var recoverUrlBase = CoreFactory.Singleton.Properties.AcctPswdRecoveryURL;
                var recoverUrlAdj = string.Format(RECOVER_URL_FORMAT, recoverUrlBase, RecoverContext.RecoveryID);

                return recoverUrlAdj;

            }
        }





        public static AcctRecoveryResultCode ValidateRecoveryKey(IUserRecoveryContext RecoverContext, string Key)
        {
            if (RecoverContext == null)
            {
                return AcctRecoveryResultCode.NullArgument;
            }


            if (RecoverContext.EffFromDate > DateTimeOffset.Now || RecoverContext.EffToDate < DateTimeOffset.Now)
            {
                TryDelete(RecoverContext);
                return AcctRecoveryResultCode.RecoveryContextExpired;
            }

            if (!RecoverContext.IsEnabled)
            {
                return AcctRecoveryResultCode.RecoveryContextDisabled;
            }

            if (RecoverContext.AttemptCount > CoreFactory.Singleton.Properties.AcctPswdResetMaxAttemptCount)
            {
                TryDelete(RecoverContext);
                return AcctRecoveryResultCode.RecoveryContextDestroyed;
            }


            bool isValid = false;
            if (CoreFactory.Singleton.Properties.PswdHashType == CryptoHashType.Bcrypt)
            {
                isValid = BCrypt.Net.BCrypt.Verify(Key, RecoverContext.RecoveryKey);
            }
            else
            {
                isValid = RecoverContext.RecoveryKey == Key.GetCryptoHash(CoreFactory.Singleton.Properties.PswdHashType);
            }


            if (isValid)
            {
                return AcctRecoveryResultCode.Success;
            }
            else
            {
                return AcctRecoveryResultCode.RecoveryKeyInvalid;
            }
        }



        
    }
}
