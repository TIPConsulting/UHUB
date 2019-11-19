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
        /// Get set of clubs with associated post counts
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<PostClusteredCount>> TryGetPostClusteredCountsAsync(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecEntityQueryAsync<PostClusteredCount>(
                    _dbConn,
                    "[dbo].[Posts_GetClusteredCountsBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("03631697-0818-490E-9F8A-F9865994EA85");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get number of posts in school
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static async Task<long?> TryGetPostCountBySchoolAsync(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return await SqlWorker.ExecScalarAsync<long>(
                    _dbConn,
                    "[dbo].[Posts_GetCountBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("7036C9FE-F904-420E-9F6E-D101D367DB44");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }

        }



        /// <summary>
        /// Get all the posts in the DB by school
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> TryGetPostsBySchoolAsync(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return await SqlWorker.ExecEntityQueryAsync<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("D36EEB74-6E95-47EE-A396-F05C6E85AC26");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the posts in the DB by school page
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> TryGetPostsBySchoolPageAsync(long SchoolID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            //SET DEFAULT ITEM COUNT
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            try
            {
                return await SqlWorker.ExecEntityQueryAsync<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetBySchoolPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                        cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(StartID);
                        cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(PageNum);
                        cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                    });


            }
            catch (Exception ex)
            {
                var exID = new Guid("BC55929E-8421-4C59-B93D-70BD70847A9D");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }




        /// <summary>
        /// Get all the posts in the DB by school page.  Uses default StartID/PageNum of 0
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<Post> PostSet, long StartID)> TryGetPostsBySchoolPageAsync(long SchoolID, short? ItemCount)
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
                    "[dbo].[Posts_GetBySchoolPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
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
                var exID = new Guid("700E9FF9-4912-4547-97F1-ADFF83E3E105");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return (null, 0);
            }
        }

    }
}
