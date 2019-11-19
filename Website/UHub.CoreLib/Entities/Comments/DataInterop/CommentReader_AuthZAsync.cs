using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Comments.DataInterop
{
    public static partial class CommentReader
    {
        /// <summary>
        /// Check if user is able to comment to specified ent parent
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<bool> TryValidateCommentParentAsync(long UserID, long ParentID)
        {
            try
            {

                return await SqlWorker.ExecScalarAsync<bool>(
                    _dbConn,
                    "[dbo].[User_ValidateCommentParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = UserID;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("806327DF-92D9-4400-974C-11D7EB2C1793");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }

        }


    }
}
