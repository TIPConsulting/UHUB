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
using static UHub.CoreLib.DataInterop.SqlConverters;

namespace UHub.CoreLib.Entities.Posts.DataInterop
{

    public static partial class PostReader
    {
        //TODO: abstract to config file
        private const short DEFAULT_PAGE_SIZE = 20;

        private static SqlConfig _dbConn = null;

        static PostReader()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

    }
}
