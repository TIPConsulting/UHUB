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
        public static async Task<long?> TryGetPostCountByParentAsync(long ParentID, bool IncludePrivatePosts)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return await SqlWorker.ExecScalarAsync<long>(
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
                var exID = new Guid("5C6B616F-F4C2-49EC-8E1C-4FE11BF15724");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }

        }


        /// <summary>
        /// Get all the posts in the DB by parent
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> TryGetPostsByParentAsync(long ParentID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return await SqlWorker.ExecEntityQueryAsync<Post>(
                    _dbConn,
                    "[dbo].[Posts_GetByParent]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("05ABBA2F-7F9A-4B12-86D6-E77C366A109C");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the posts in the DB by parent page
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Post>> TryGetPostsByParentPageAsync(long ParentID, long? StartID, int? PageNum, short? ItemCount)
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
                var exID = new Guid("82BA07FF-3D9F-453B-9F0F-A3F017B81B03");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the posts in the DB by parent page.  Uses default StartID/PageNum of 0
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="StartID"></param>
        /// <param name="PageNum"></param>
        /// <param name="ItemCount"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<Post> PostSet, long StartID)> TryGetPostsByParentPageAsync(long ParentID, short? ItemCount)
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
                    "[dbo].[Posts_GetByParentPage]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
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
                var exID = new Guid("9D627668-B2F7-4053-894A-7332FA7520F7");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return (null, 0);
            }
        }

    }
}
