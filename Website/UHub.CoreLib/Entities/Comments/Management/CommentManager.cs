using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.RegExp;


namespace UHub.CoreLib.Entities.Comments.Management
{
    public static partial class CommentManager
    {

        private static class Shared
        {
            internal static void TryCreate_HandleAttrTrim(ref Comment NewComment)
            {
                NewComment.Content = NewComment.Content?.Trim();
            }



            internal static CommentResultCode TryCreate_ValidateCommentAttrs(in Comment NewComment)
            {

                //Validate Content
                if (NewComment.Content.IsEmpty())
                {
                    return CommentResultCode.ContentEmpty;
                }
                if (!NewComment.Content.RgxIsMatch(RgxPtrns.EntComment.CONTENT_B, RegexOptions.Multiline))
                {
                    return CommentResultCode.ContentInvalid;
                }


                return CommentResultCode.Success;
            }



            internal static void TryCreate_AttrConversionHandler(ref Comment NewComment)
            {

                var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
                if ((sanitizerMode & HtmlSanitizerMode.OnWrite) != 0)
                {
                    NewComment.Content = NewComment.Content?.SanitizeHtml().HtmlDecode();
                }

            }
        }


    }
}
