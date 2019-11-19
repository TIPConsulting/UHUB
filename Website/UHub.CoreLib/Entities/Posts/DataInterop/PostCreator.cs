using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using static UHub.CoreLib.DataInterop.SqlConverters;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Posts.DataInterop
{
    internal static partial class PostWriter
    {

        /// <summary>
        /// Attempts to create a new CMS post in the database and returns the PostID if successful
        /// </summary>
        /// <param name="cmsPost"></param>
        /// <returns></returns>
        internal static long? CreatePost(Post cmsPost)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            long? PostID = SqlWorker.ExecScalar<long?>(
                _dbConn,
                "[dbo].[Post_Create]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsPost.Name);
                    cmd.Parameters.Add("@Content", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsPost.Content);
                    cmd.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = HandleParamEmpty(cmsPost.IsLocked);
                    cmd.Parameters.Add("@CanComment", SqlDbType.Bit).Value = HandleParamEmpty(cmsPost.CanComment);
                    cmd.Parameters.Add("@IsPublic", SqlDbType.Bit).Value = HandleParamEmpty(cmsPost.IsPublic);
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = HandleParamEmpty(cmsPost.ParentID);
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = HandleParamEmpty(cmsPost.CreatedBy);
                    cmd.Parameters.Add("@IsReadonly", SqlDbType.Bit).Value = HandleParamEmpty(cmsPost.IsReadOnly);
                });

            return PostID;
        }


    }
}
