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
using UHub.CoreLib.Entities.Users;

namespace UHub.CoreLib.Entities.Posts.DataInterop
{

    public static partial class PostReader
    {

        /// <summary>
        /// Get DB post full detail by LONG ID
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        public static Post TryGetPost(long PostID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecEntityQuery<Post>(
                    _dbConn,
                    "[dbo].[Post_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("CCE3A0A5-DC88-42B6-ADC0-F2F95995203C");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the posts in the DB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Post> TryGetAllPosts()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return SqlWorker.ExecEntityQuery<Post>(_dbConn, "[dbo].[Posts_GetAll]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("00877A2B-2DE3-492A-9EFE-5FDB837E9C59");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return Enumerable.Empty<Post>();
            }
        }




        /// <summary>
        /// Get set of clubs with associated post counts
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PostClusteredCount> TryGetPostClusteredCounts()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return SqlWorker.ExecEntityQuery<PostClusteredCount>(_dbConn, "[dbo].[Posts_GetClusteredCounts]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("D93AA28B-A453-4BC1-B32C-882BEAEDCEDE");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }



        public static IEnumerable<User> TryGetPostCommenters(long PostID)
        {
            try
            {
                return SqlWorker.ExecEntityQuery<User>(
                    _dbConn,
                    "[dbo].[Users_GetPostCommenters]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@PostID", SqlDbType.BigInt).Value = PostID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("C21265BA-9169-4F85-A2AB-72151DFB896F");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

    }
}
