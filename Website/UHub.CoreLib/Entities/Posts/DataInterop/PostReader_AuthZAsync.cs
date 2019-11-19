using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Posts.DataInterop
{
    public static partial class PostReader
    {
        /// <summary>
        /// Check if user is able to write post to specified ent parent
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<bool> TryValidatePostParentAsync(long UserID, long ParentID)
        {
            try
            {
                return await SqlWorker.ExecScalarAsync<bool>(
                    _dbConn,
                    "[dbo].[User_ValidatePostParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("E8A7C53B-BC51-4556-A730-16A570952B5D");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }

        }


        

    }
}
