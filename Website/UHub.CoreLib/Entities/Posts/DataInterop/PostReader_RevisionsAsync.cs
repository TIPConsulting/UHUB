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
        public static async Task<IEnumerable<Post>> TryGetPostRevisionsAsync(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return await SqlWorker.ExecEntityQueryAsync<Post>(
                    _dbConn,
                    "[dbo].[Post_GetRevisions]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@EntID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("7EC420E2-55D7-4F75-B335-1428C715A39A");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }

    }
}
