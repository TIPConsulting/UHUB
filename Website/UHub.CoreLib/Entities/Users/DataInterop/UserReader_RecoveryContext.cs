using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Users.Interfaces;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    public static partial class UserReader
    {

        public static IUserRecoveryContext GetRecoveryContext(long UserID)
        {
            return SqlWorker.ExecEntityQuery<UserRecoveryContext>(
                _dbConn,
                "[dbo].[User_GetRecoveryContextByUserID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                })
                .SingleOrDefault();

        }

        public static IUserRecoveryContext GetRecoveryContext(string RecoveryID)
        {
            return SqlWorker.ExecEntityQuery<UserRecoveryContext>(
                _dbConn,
                "[dbo].[User_GetRecoveryContextByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@RecoveryID", SqlDbType.NVarChar).Value = RecoveryID;
                })
                .SingleOrDefault();
        }

    }
}
