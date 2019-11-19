using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using static UHub.CoreLib.DataInterop.SqlConverters;

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop
{
    [Obsolete("Should not be used directly.  Use SchoolClubManager instead.")]
    internal static partial class SchoolClubWriter
    {
        private static SqlConfig _dbConn = null;

        static SchoolClubWriter()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }
    }
}
