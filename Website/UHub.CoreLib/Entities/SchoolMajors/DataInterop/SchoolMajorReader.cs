using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolMajors.DataInterop
{
    public static partial class SchoolMajorReader
    {
        private static SqlConfig _dbConn = null;

        static SchoolMajorReader()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }


    }
}
