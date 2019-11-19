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
        public static long TryGetUserLikeCount(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return SqlWorker.ExecScalar<long>(
                    _dbConn,
                    "[dbo].[Post_GetUserLikeCount]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("E7CCA92C-2FFA-4DDE-B862-7CA2CFB01AAF");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return -1L;
            }

        }


    }
}
