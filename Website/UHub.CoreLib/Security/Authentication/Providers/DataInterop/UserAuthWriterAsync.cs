using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Security.Authentication.Providers.DataInterop
{
    internal static partial class UserAuthWriter
    {

        public static async Task LogFailedPsdAttemptAsync(long UserID)
        {

            await SqlWorker.ExecNonQueryAsync(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_LogFailedPswdAttemptByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });
        }

    }
}
