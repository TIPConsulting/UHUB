using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Files.DataInterop
{
    [Obsolete("Should not be used directly.  Use FileManager instead.")]
    internal static partial class FileWriter
    {
        private static string _dbConn = null;

        static FileWriter()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

    }
}
