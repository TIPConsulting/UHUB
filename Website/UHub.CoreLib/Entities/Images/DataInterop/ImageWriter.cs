using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Images.DataInterop
{
    [Obsolete("Should not be used directly.  Use ImageManager instead.")]
    internal static partial class ImageWriter
    {
        private static string _dbConn = null;

        static ImageWriter()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

    }
}
