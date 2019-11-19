using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;

namespace UHub.CoreLib.Entities.Posts.Management
{
#pragma warning disable 612,618

    public static partial class PostManager
    {

        public static (long? PostID, PostResultCode ResultCode) TryCreatePost(Post NewPost)
        {
            if (NewPost == null)
            {
                return (null, PostResultCode.NullArgument);
            }


            Shared.TryCreate_HandleAttrTrim(ref NewPost);

            Shared.TryCreate_AttrConversionHandler(ref NewPost);

            var attrValidateCode = Shared.TryCreate_ValidatePostAttrs(NewPost);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }


            long? id = null;
            try
            {
                id = PostWriter.CreatePost(NewPost);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (null, PostResultCode.InvalidArgument);
            }
            catch (ArgumentNullException)
            {
                return (null, PostResultCode.NullArgument);
            }
            catch (ArgumentException)
            {
                return (null, PostResultCode.InvalidArgument);
            }
            catch (InvalidCastException)
            {
                return (null, PostResultCode.InvalidArgumentType);
            }
            catch (InvalidOperationException)
            {
                return (null, PostResultCode.InvalidOperation);
            }
            catch (AccessForbiddenException)
            {
                return (null, PostResultCode.AccessDenied);
            }
            catch (EntityGoneException)
            {
                return (null, PostResultCode.InvalidOperation);
            }
            catch (Exception ex)
            {
                var exID = new Guid("372741E4-6439-41F8-BADC-15BEABA99165");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return (null, PostResultCode.UnknownError);
            }




            if (id == null)
            {
                return (id, PostResultCode.UnknownError);
            }
            return (id, PostResultCode.Success);

        }



        public static bool? TryIncrementViewCount(long PostID, long UserID)
        {

            bool? val = null;
            try
            {
                val = PostWriter.IncrementViewCount(PostID, UserID);
            }
            catch (Exception ex)
            {
                var exID = new Guid("8A829937-3470-4A7C-9E62-36234575FB88");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
            }


            return val;
        }


        public static PostResultCode TryUpdatePost(Post CmsPost)
        {
            if (CmsPost == null)
            {
                return PostResultCode.NullArgument;
            }



            Shared.TryCreate_HandleAttrTrim(ref CmsPost);

            Shared.TryCreate_AttrConversionHandler(ref CmsPost);

            var attrValidateCode = Shared.TryCreate_ValidatePostAttrs(CmsPost);
            if (attrValidateCode != 0)
            {
                return attrValidateCode;
            }


            
            try
            {
                PostWriter.UpdatePost(CmsPost);
            }
            catch (ArgumentOutOfRangeException)
            {
                return PostResultCode.InvalidArgument;
            }
            catch (ArgumentNullException)
            {
                return PostResultCode.NullArgument;
            }
            catch (ArgumentException)
            {
                return PostResultCode.InvalidArgument;
            }
            catch (InvalidCastException)
            {
                return PostResultCode.InvalidArgumentType;
            }
            catch (InvalidOperationException)
            {
                return PostResultCode.InvalidOperation;
            }
            catch (AccessForbiddenException)
            {
                return PostResultCode.AccessDenied;
            }
            catch (EntityGoneException)
            {
                return PostResultCode.InvalidOperation;
            }
            catch (Exception ex)
            {
                var exID = new Guid("CDB83704-5E14-48DB-AEB9-FA947EA91D0B");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return PostResultCode.UnknownError;
            }


            return PostResultCode.Success;
        }


        public static bool? TryCreateUserLike(long PostID, long UserID)
        {

            bool? val = null;
            try
            {
                val = PostWriter.CreateUserLike(PostID, UserID);
            }
            catch (Exception ex)
            {
                var exID = new Guid("76A086E9-7C41-4464-8607-ED2EBE178FC5");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
            }


            return val;
        }
    }

#pragma warning restore
}
