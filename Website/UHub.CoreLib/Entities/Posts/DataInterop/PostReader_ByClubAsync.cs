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
        public static async Task<long?> TryGetPostCountByClubAsync(long ClubID, bool IncludePrivatePosts)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return await SqlWorker.ExecScalarAsync<long>(
                    _dbConn,
                    "[dbo].[Posts_GetCountByClub]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                        cmd.Parameters.Add("@IncludePrivatePosts", SqlDbType.Bit).Value = IncludePrivatePosts;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("E4B4FFE6-9340-41D9-9F72-E01518E56B16");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }

        }



        /// <summary>
        /// Get all the posts in the DB by school club
        /// </summary>
        /// <param name="SchoolClubID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> TryGetPostsByClubAsync(long SchoolClubID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecEntityQueryAsync<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetByClub]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("9DF4547A-C900-421B-8726-0C9418F1F316");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
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
        public static async Task<IEnumerable<Post>> TryGetPostsByClubPageAsync(long ClubID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            try
            {

                return await SqlWorker.ExecEntityQueryAsync<Post>(
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
                var exID = new Guid("363FF733-9C59-42BC-B4B1-D681A0A5DF68");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
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
        public static async Task<(List<Post> PostSet, long StartID)> TryGetPostsByClubPageAsync(long ClubID, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            try
            {

                var postSetRaw = await SqlWorker.ExecEntityQueryAsync<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetByClubPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ClubID", SqlDbType.BigInt).Value = ClubID;
                        cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(null);
                        cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(null);
                        cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                    });


                var postSet = postSetRaw.ToList();

                long maxID = -1;
                for (int i = 0; i < postSet.Count; i++)
                {
                    if ((postSet[i]?.ID ?? -1) > maxID)
                    {
                        maxID = postSet[i].ID.Value;
                    }
                }


                return (postSet, maxID);

            }
            catch (Exception ex)
            {
                var exID = new Guid("E4B4FFE6-9340-41D9-9F72-E01518E56B16");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return (null, 0);
            }


        }
    }
}
