using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;
using UHub.CoreLib.DataInterop;
using static UHub.CoreLib.DataInterop.SqlConverters;


namespace UHub.CoreLib.Security.Authentication.Providers.DataInterop
{
    internal static partial class UserAuthReader
    {

        public static async Task<AccountAuthData> TryGetUserAuthDataAsync(long UserID)
        {

            try
            {
                var temp = await SqlWorker.ExecEntityQueryAsync<AccountAuthData>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_GetAuthInfoByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });


                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                var exID = new Guid("1CD376D5-0252-4E0C-B177-F3FEF5F63FA6");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }

        public static async Task<AccountAuthData> TryGetUserAuthDataAsync(string Email)
        {
            try
            {
                var temp = await SqlWorker.ExecEntityQueryAsync<AccountAuthData>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_GetAuthInfoByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
                    });


                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                var exID = new Guid("9CF5197D-DC03-4750-955C-5F68D8208F33");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


        public static async Task<AccountAuthData> TryGetUserAuthDataAsync(string Username, string Domain)
        {
            try
            {

                var temp = await SqlWorker.ExecEntityQueryAsync<AccountAuthData>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_GetAuthInfoByUsername]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                    });


                return temp.SingleOrDefault();
            }
            catch (Exception ex)
            {
                var exID = new Guid("CB1F4E24-9902-46E8-B85D-8C3D5A010533");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


    }
}
