using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Schools.DataInterop
{
    [Obsolete("Should not be used directly.  Use SchoolManager instead.")]
    internal static partial class SchoolWriter
    {
        private static string _dbConn = null;

        static SchoolWriter()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }
    }
}
