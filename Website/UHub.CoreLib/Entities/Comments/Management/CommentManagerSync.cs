using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Comments.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Comments.Management
{
#pragma warning disable 612, 618
    public static partial class CommentManager
    {
        public static (long? CommentID, CommentResultCode ResultCode) TryCreateComment(Comment NewComment)
        {
            if (NewComment == null)
            {
                return (null, CommentResultCode.NullArgument);
            }



            Shared.TryCreate_HandleAttrTrim(ref NewComment);

            Shared.TryCreate_AttrConversionHandler(ref NewComment);

            var attrValidateCode = Shared.TryCreate_ValidateCommentAttrs(NewComment);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }


            long? id = null;

            try
            {
                id = CommentWriter.CreateComment(NewComment);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (null, CommentResultCode.InvalidArgument);
            }
            catch (ArgumentNullException)
            {
                return (null, CommentResultCode.NullArgument);
            }
            catch (ArgumentException)
            {
                return (null, CommentResultCode.InvalidArgument);
            }
            catch (InvalidCastException)
            {
                return (null, CommentResultCode.InvalidArgumentType);
            }
            catch (InvalidOperationException)
            {
                return (null, CommentResultCode.InvalidOperation);
            }
            catch (AccessForbiddenException)
            {
                return (null, CommentResultCode.AccessDenied);
            }
            catch (EntityGoneException)
            {
                return (null, CommentResultCode.InvalidOperation);
            }
            catch (Exception ex)
            {
                var exID = new Guid("49F489CE-8053-4248-86CB-D6474569D4B1");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return (null, CommentResultCode.UnknownError);
            }





            if (id == null)
            {
                return (id, CommentResultCode.UnknownError);
            }
            return (id, CommentResultCode.Success);

        }
    }


#pragma warning restore
}
