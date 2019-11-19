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
        public static IEnumerable<PostClusteredCount> TryGetPostClusteredCounts(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return SqlWorker.ExecEntityQuery<PostClusteredCount>(
                    _dbConn,
                    "[dbo].[Posts_GetClusteredCountsBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("F70A563D-BD5C-4304-A7F8-E83A869F96E4");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }

        }



        /// <summary>
        /// Get number of posts in school
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static long? TryGetPostCountBySchool(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecScalar<long>(
                    _dbConn,
                    "[dbo].[Posts_GetCountBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("56B2BB40-B3BC-4B75-92F8-C70D646BFA4F");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }

        }



        /// <summary>
        /// Get all the posts in the DB by school
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static IEnumerable<Post> TryGetPostsBySchool(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return SqlWorker.ExecEntityQuery<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("20788253-3045-419B-BBA4-D72A4A54EC36");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
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
        public static IEnumerable<Post> TryGetPostsBySchoolPage(long SchoolID, long? StartID, int? PageNum, short? ItemCount)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }
            //SET DEFAULT ITEM COUNT
            ItemCount = ItemCount ?? DEFAULT_PAGE_SIZE;


            try
            {

                return SqlWorker.ExecEntityQuery<Post>(
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
                var exID = new Guid("38436ACD-707E-42C1-86E3-8C9B252F582A");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
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
        public static List<Post> TryGetPostsBySchoolPage(long SchoolID, short? ItemCount, out long StartID)
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
                    "[dbo].[Posts_GetBySchoolPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
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
                var exID = new Guid("38436ACD-707E-42C1-86E3-8C9B252F582A");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                StartID = 0;
                return null;
            }
        }



    }
}
