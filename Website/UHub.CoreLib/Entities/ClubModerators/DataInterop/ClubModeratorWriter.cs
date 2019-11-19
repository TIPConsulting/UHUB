using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using static UHub.CoreLib.DataInterop.SqlConverters;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.ClubModerators.DataInterop
{
    [Obsolete("Should not be used directly.  Use ClubModeratorManager instead.")]
    public sealed partial class ClubModeratorWriter
    {
        private static SqlConfig _dbConn = null;

        static ClubModeratorWriter()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

    }
}
