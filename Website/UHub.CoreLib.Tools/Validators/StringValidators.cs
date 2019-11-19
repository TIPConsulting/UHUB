using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.RegExp;
using UHub.CoreLib.RegExp.Compiled;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Tools
{
    public static partial class Validators
    {

        /// <summary>
        /// Check if a string is a valid email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidEmail(string email)
        {
            if (email.IsEmpty())
            {
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if a string is a valid email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidEmailDomain(string domain)
        {
            if (domain.IsEmpty())
            {
                return false;
            }

            if (!domain.StartsWith("@"))
            {
                return false;
            }

            return domain.RgxIsMatch(RgxPtrns.EntUser.EMAIL_DOMAIN_B);

        }

        /// <summary>
        /// Checks if a string is a valid URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidURL(string url)
        {
            if (url.IsEmpty())
            {
                return false;
            }


            Uri tempOut;
            return Uri.TryCreate(url, UriKind.Absolute, out tempOut) && (tempOut.Scheme == Uri.UriSchemeHttp || tempOut.Scheme == Uri.UriSchemeHttps);
        }


        /// <summary>
        /// Check if string is a valid file name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidFileName(string name)
        {
            name = name.Trim();

            if (name.IsEmpty())
                return false;

            //ensure that the name is a valid char set
            //ensure that the name is more than whitespace

            return
                RgxCompiled.FileUpload.FILE_NAME_B.IsMatch(name)
                && !name.RgxIsMatch(@"^[\s.\-]*$");
        }

    }
}
