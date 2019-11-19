using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Tools
{
    public static partial class Converters
    {

      
        /// <summary>
        /// Get physical URL from virtual or return a formatted copy of a physical address
        /// </summary>
        /// <param name="srcUrl"></param>
        /// <returns></returns>
        public static string GetAbsURL(string baseUrl, string srcUrl)
        {
            if (srcUrl.IsEmpty())
            {
                return "";
            }
            if (srcUrl.StartsWith("~/"))
            {
                var temp = new Uri(new Uri(baseUrl), VirtualPathUtility.ToAbsolute(srcUrl)).ToString();
                return temp.Trim().Trim('/');
            }
            else
            {
                return srcUrl.Trim().Trim('/');
            }
        }

        
        /// <summary>
        /// Get physical file path given a virutal or physical path
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string GetAbsFileDirectory(string directory)
        {
            if (directory.IsEmpty())
            {
                return "";
            }
            //check for virtual directory and get physical path
            //  ~/foo
            //  ../foo
            //  /foo
            //  NOT //foo
            if (directory.RgxIsMatch(@"^\~(\/|\\)") || directory.RgxIsMatch(@"^(\.)+(\/|\\)") || (directory.RgxIsMatch(@"^(\/|\\)") && !directory.RgxIsMatch(@"^(\/\/|\\\\)")))
            {
                var temp = HostingEnvironment.MapPath(directory);
                return temp.Trim().TrimEnd('/').TrimEnd('\\');
            }
            else
            {
                return directory.Trim().TrimEnd('/').TrimEnd('\\');
            }
        }

    }
}
