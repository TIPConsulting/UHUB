using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop
{
    public static partial class SchoolClubReader
    {


        public static bool TryValidateMembership(long ClubID, long UserID)
        {
            try
            {
                return SqlWorker.ExecScalar<bool>(
                    _dbConn,
                    "[dbo].[SchoolClub_ValidateMembership]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("44219C4E-D8ED-4EFD-BA61-B5635D35C118");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return false;
            }
        }

        public static bool TryIsUserBanned(long ClubID, long UserID)
        {
            try
            {

                return SqlWorker.ExecScalar<bool>(
                    _dbConn,
                    "[dbo].[SchoolClub_IsUserBanned]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("948A81CC-83B7-4583-8BF9-A1D31A578A43");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return true;
            }
        }

    }
}
