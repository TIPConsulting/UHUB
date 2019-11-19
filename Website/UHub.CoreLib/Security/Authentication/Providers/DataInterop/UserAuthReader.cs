using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;
using UHub.CoreLib.DataInterop;
using static UHub.CoreLib.DataInterop.SqlConverters;
using System.Threading;

namespace UHub.CoreLib.Security.Authentication.Providers.DataInterop
{
    internal static partial class UserAuthReader
    {

        public static AccountAuthData TryGetUserAuthData(long UserID)
        {

            try
            {
                return SqlWorker.ExecEntityQuery<AccountAuthData>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_GetAuthInfoByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    })
                    .SingleOrDefault();
            }
            catch (Exception ex)
            {
                var exID = new Guid("CFB022ED-7385-45DF-9FB0-ED56E25ABD5C");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

        public static AccountAuthData TryGetUserAuthData(string Email)
        {
            try
            {
                return SqlWorker.ExecEntityQuery<AccountAuthData>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_GetAuthInfoByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
                    })
                    .SingleOrDefault();
            }
            catch (Exception ex)
            {
                var exID = new Guid("99E80625-980D-4F19-89B1-41FBA4D62A02");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }


        public static AccountAuthData TryGetUserAuthData(string Username, string Domain)
        {
            try
            {

                return SqlWorker.ExecEntityQuery<AccountAuthData>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_GetAuthInfoByUsername]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                    })
                    .SingleOrDefault();
            }
            catch (Exception ex)
            {
                var exID = new Guid("D77C69A0-CAE4-49F8-99DC-407F01B5E394");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

    }
}
