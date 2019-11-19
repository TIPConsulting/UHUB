using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.ClubModerators.DataInterop
{
    public static partial class ClubModeratorReader
    {
        /// <summary>
        /// Check if user is able to write post to specified ent parent
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static bool TryValidateClubModerator(long ClubID, long UserID)
        {

            try
            {
                return SqlWorker.ExecScalar<bool>(
                    _dbConn,
                    "[dbo].[User_ValidateClubModerator]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@CLubID", SqlDbType.BigInt).Value = ClubID;
                    });


            }
            catch (Exception ex)
            {
                var exID = new Guid("AFB6D420-F9C2-433E-9EF5-9DF877C2409E");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return false;
            }

        }


    }
}
