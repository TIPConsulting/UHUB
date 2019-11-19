using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using UHub.CoreLib.RegExp;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Tools
{
    public static partial class ConfigValidators
    {

        /// <summary>
        /// Check that string is not null/empty
        /// </summary>
        /// <param name="str">String to test</param>
        /// <param name="argName">Name of string variable</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateString(string str, string argName)
        {
            if (str.IsEmpty())
            {
                throw new ArgumentException(argName + " cannot be null or empty.");
            }
        }



        /// <summary>
        /// Ensure that string is a valid email
        /// </summary>
        /// <param name="email">Email to test</param>
        /// <param name="argName">Name of email variable</param>
        public static void ValidateEmail(string email, string argName)
        {
            if (!Validators.IsValidEmail(email))
            {
                throw new ArgumentException(argName + " is not a valid email address.");
            }
        }

        /// <summary>
        /// Attempt to verify that directories are valid. If selected, create missing directories
        /// </summary>
        /// <param name="dir">File directory to test</param>
        /// <param name="argName">Name of directory variable</param>
        public static void ValidateDirectory(string dir, string argName, bool CreateMissingDir)
        {
            ValidateString(dir, argName);


            string tempDir;
            if (dir.RgxIsMatch(@"^\~(\/|\\)") || dir.RgxIsMatch(@"^(\.)+(\/|\\)") || (dir.RgxIsMatch(@"^(\/|\\)") && !dir.RgxIsMatch(@"^(\/\/|\\\\)")))
            {
                try
                {
                    tempDir = HostingEnvironment.MapPath(dir);
                }
                catch
                {
                    throw new ArgumentException(argName + " is not a valid virtual directory");
                }
                if (tempDir.IsEmpty())
                {
                    throw new ArgumentException(argName + " is not a valid virtual directory");
                }
            }
            else
            {
                tempDir = dir.Trim();
            }


            if (!Directory.Exists(tempDir))
            {
                if (CreateMissingDir)
                {
                    try
                    {
                        Directory.CreateDirectory(tempDir);
                    }
                    catch
                    {
                        throw new InvalidOperationException(argName + " cannot be created due to an error");
                    }
                }
                else
                {
                    throw new ArgumentException(argName + " is not a valid directory");
                }
            }
        }



        /// <summary>
        /// Ensure that url is valid, either virtual or physical, and uses HTTPS where required
        /// </summary>
        /// <param name="url">URL to test</param>
        /// <param name="argName">Name of URL variable</param>
        public static void ValidateUrl(string url, string argName, bool ForceHttps, bool EnableVirtual = true)
        {
            //start with basic URL validation
            ValidateString(url, argName);


            if (EnableVirtual)
            {
                var isVirtual = url.RgxIsMatch(RgxPtrns.Config.INTERNAL_URL_B, RegexOptions.IgnoreCase);
                var isPhysical = !url.RgxIsMatch("[?#]") && Validators.IsValidURL(url);

                if (!isVirtual && !isPhysical)
                {
                    throw new ArgumentException(argName + " is not a valid URL.  URL must be physical or a root-relative virtual path");
                }
            }
            else
            {
                var isPhysical = !url.RgxIsMatch("[?#]") && Validators.IsValidURL(url);
                if (!isPhysical)
                {
                    throw new ArgumentException(argName + " is not a valid URL.  URL must be physical path");
                }

            }



            //check for secure URL if required
            if (!ForceHttps)
            {
                return;
            }

            var lowerUrl = url.ToLower();
            if (!lowerUrl.StartsWith("~/") && !lowerUrl.StartsWith("https://"))
            {
                throw new ArgumentException(argName + " must use the https protocol when ForceHTTPS is enabled.");
            }
        }


    }
}
