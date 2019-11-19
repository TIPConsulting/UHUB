using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Comments.DataInterop
{
    public static partial class CommentReader
    {

        /// <summary>
        /// Get all the comments in the DB from a post
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Comment>> TryGetCommentsByPostAsync(long PostID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return await SqlWorker.ExecEntityQueryAsync<Comment>(
                    _dbConn,
                    "[dbo].[Comments_GetByPost]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("66C367D8-119F-4539-92DD-48F01738BDA1");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the comments in the DB from a single parent
        /// Will only search one level deep based on their direct parent
        /// </summary>
        /// /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Comment>> TryGetCommentsByParentAsync(long ParentID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return await SqlWorker.ExecEntityQueryAsync<Comment>(
                    _dbConn,
                    "[dbo].[Comments_GetByParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("63E18CDB-F9A3-41EE-AF75-954AD55A4E40");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }
    }
}
