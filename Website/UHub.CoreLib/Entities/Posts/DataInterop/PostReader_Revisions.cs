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
using static UHub.CoreLib.DataInterop.SqlConverters;

namespace UHub.CoreLib.Entities.Posts.DataInterop
{

    public static partial class PostReader
    {

        /// <summary>
        /// Get DB post revision history by LONG ID
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        public static IEnumerable<Post> TryGetPostRevisions(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return SqlWorker.ExecEntityQuery<Post>(
                    _dbConn,
                    "[dbo].[Post_GetRevisions]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@EntID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("D9CCACBF-7D52-4941-AD6E-FE57DC32322F");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

    }
}
