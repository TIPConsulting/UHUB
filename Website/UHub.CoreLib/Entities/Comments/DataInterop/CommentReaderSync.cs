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
        public static IEnumerable<Comment> TryGetCommentsByPost(long PostID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecEntityQuery<Comment>(
                    _dbConn,
                    "[dbo].[Comments_GetByPost]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("88E483DA-53AE-492D-95CA-C02E14FC5868");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the comments in the DB from a single parent
        /// Will only search one level deep based on their direct parent
        /// </summary>
        /// /// <param name="ParentID"></param>
        /// <returns></returns>
        public static IEnumerable<Comment> TryGetCommentsByParent(long ParentID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return SqlWorker.ExecEntityQuery<Comment>(
                    _dbConn,
                    "[dbo].[Comments_GetByParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("EE567558-4FFB-4017-A4BF-EE7D3B5A5365");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

    }
}
