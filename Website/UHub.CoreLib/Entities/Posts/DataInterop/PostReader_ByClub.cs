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
        /// Get number of posts in club
        /// </summary>
        /// <param name="ClubID"></param>
        /// <param name="IncludePrivatePosts"></param>
        /// <returns></returns>
        public static long? TryGetPostCountByClub(long ClubID, bool IncludePrivatePosts)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecScalar<long>(
                    _dbConn,
                    "[dbo].[Posts_GetCountByClub]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                        cmd.Parameters.Add("@IncludePrivatePosts", SqlDbType.BigInt).Value = IncludePrivatePosts;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("15DBCA1B-D428-4584-975A-2CFFE96A65CA");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }

        }



        /// <summary>
        /// Get all the posts in the DB by school club
        /// </summary>
        /// <param name="SchoolClubID"></param>
        /// <returns></returns>
        public static IEnumerable<Post> TryGetPostsByClub(long SchoolClubID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecEntityQuery<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetByClub]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("7697B4EE-AFAD-42D3-8468-0E24FAB42533");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the posts in the DB by club page
        /// </summary>
        /// <param name="ClubID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static IEnumerable<Post> TryGetPostsByClubPage(long ClubID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            try
            {

                return SqlWorker.ExecEntityQuery<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetByClubPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                        cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(StartID);
                        cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(PageNum);
                        cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("60FDED7B-BF1D-4EE2-A1C9-85CAE25152AF");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the posts in the DB by club page.  Uses default StartID/PageNum of 0
        /// </summary>
        /// <param name="ClubID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static List<Post> TryGetPostsByClubPage(long ClubID, short? ItemCount, out long StartID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            try
            {

                var postSet = SqlWorker.ExecEntityQuery<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetByClubPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                        cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(null);
                        cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(null);
                        cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                    }).ToList();



                long maxID = -1;
                for (int i = 0; i < postSet.Count; i++)
                {
                    if ((postSet[i]?.ID ?? -1) > maxID)
                    {
                        maxID = postSet[i].ID.Value;
                    }
                }
                StartID = maxID;


                return postSet;

            }
            catch (Exception ex)
            {
                var exID = new Guid("C3406AA4-2E97-4D83-BE47-0597F8773898");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                StartID = 0;
                return null;
            }

        }

    }
}
