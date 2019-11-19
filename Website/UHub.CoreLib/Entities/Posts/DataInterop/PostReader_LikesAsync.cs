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
        public static async Task<long> TryGetUserLikeCountAsync(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return await SqlWorker.ExecScalarAsync<long>(
                    _dbConn,
                    "[dbo].[Post_GetUserLikeCount]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("34998758-FF3E-47E6-ACB5-C8BB22994B17");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return -1L;
            }

        }


    }
}
