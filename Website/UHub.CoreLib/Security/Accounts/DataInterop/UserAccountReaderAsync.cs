using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security.Accounts.Interfaces;

namespace UHub.CoreLib.Security.Accounts.DataInterop
{
    internal static partial class UserAccountReader
    {

        public static async Task<IUserConfirmToken> GetConfirmTokenAsync(long UserID)
        {

            var temp = await SqlWorker.ExecEntityQueryAsync<AccountConfirmToken>(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_GetConfirmTokenByUserID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });

            return temp.SingleOrDefault();


        }

        public static async Task<IUserConfirmToken> GetConfirmTokenAsync(string RefUID)
        {

            var temp = await SqlWorker.ExecEntityQueryAsync<AccountConfirmToken>(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_GetConfirmTokenByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@RefUID", SqlDbType.NVarChar).Value = RefUID;
                });


            return temp.SingleOrDefault();

        }

    }
}
