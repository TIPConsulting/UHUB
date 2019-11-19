using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.ClubModerators.DataInterop
{
    public static partial class ClubModeratorReader
    {
        private static SqlConfig _dbConn = null;

        static ClubModeratorReader()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }


    }
}
