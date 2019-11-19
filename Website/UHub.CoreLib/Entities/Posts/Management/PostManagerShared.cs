using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.RegExp;

namespace UHub.CoreLib.Entities.Posts.Management
{

    public static partial class PostManager
    {
        private static class Shared
        {

            internal static void TryCreate_HandleAttrTrim(ref Post NewPost)
            {
                NewPost.Name = NewPost.Name?.Trim();
                NewPost.Content = NewPost.Content?.Trim();
            }



            internal static PostResultCode TryCreate_ValidatePostAttrs(in Post NewPost)
            {
                //Validate Name
                if (NewPost.Name.IsEmpty())
                {
                    return PostResultCode.NameEmpty;
                }
                if (!NewPost.Name.RgxIsMatch(RgxPtrns.EntPost.NAME_B, RegexOptions.Singleline))
                {
                    return PostResultCode.NameInvalid;
                }


                //Validate Content
                if (NewPost.Content.IsEmpty())
                {
                    return PostResultCode.ContentEmpty;
                }
                if (!NewPost.Content.RgxIsMatch(RgxPtrns.EntPost.CONTENT_B, RegexOptions.Multiline))
                {
                    return PostResultCode.ContentInvalid;
                }


                return PostResultCode.Success;
            }


            internal static void TryCreate_AttrConversionHandler(ref Post NewPost)
            {

                var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
                if ((sanitizerMode & HtmlSanitizerMode.OnWrite) != 0)
                {
                    NewPost.Content = NewPost.Content?.SanitizeHtml().HtmlDecode();
                }

            }
        }
    }
}
