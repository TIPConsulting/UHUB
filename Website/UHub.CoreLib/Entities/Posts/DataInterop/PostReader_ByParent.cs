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
        /// Get the number of posts direct-descendant from a parent entity
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="IncludePrivatePosts"></param>
        /// <returns></returns>
        public static long? TryGetPostCountByParent(long ParentID, bool IncludePrivatePosts)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return SqlWorker.ExecScalar<long>(
                    _dbConn,
                    "[dbo].[Posts_GetCountByParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                        cmd.Parameters.Add("@IncludePrivatePosts", SqlDbType.Bit).Value = IncludePrivatePosts;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("A99B9CE7-E898-41D1-A424-33EAB87CF979");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }

        }




        /// <summary>
        /// Get all the posts in the DB direct-descendant from a parent entity
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static IEnumerable<Post> TryGetPostsByParent(long ParentID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecEntityQuery<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetByParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("DA787BC5-7E37-4FEA-B157-D24A1A870FA7");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get a page of posts in the DB direct-descendant from a parent entity
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static IEnumerable<Post> TryGetPostsByParentPage(long ParentID, long? StartID, int? PageNum, short? ItemCount)
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
                    "[dbo].[Posts_GetByParentPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                        cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(StartID);
                        cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(PageNum);
                        cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("9E3B37E9-ECF6-44E0-86F6-158DFA8A1337");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get a page of posts in the DB direct-descendant from a parent entity.  Uses default StartID/PageNum of 0
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static List<Post> TryGetPostsByParentPage(long ParentID, short? ItemCount, out long StartID)
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
                    "[dbo].[Posts_GetByParentPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                        cmd.Parameters.Add("@StartID", SqlDbType.BigInt).Value = HandleParamEmpty(null);
                        cmd.Parameters.Add("@PageNum", SqlDbType.Int).Value = HandleParamEmpty(null);
                        cmd.Parameters.Add("@ItemCount", SqlDbType.SmallInt).Value = ItemCount;
                    })
                    .ToList();


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
                var exID = new Guid("E4B4FFE6-9340-41D9-9F72-E01518E56B16");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                StartID = 0;
                return null;
            }
        }

    }
}
