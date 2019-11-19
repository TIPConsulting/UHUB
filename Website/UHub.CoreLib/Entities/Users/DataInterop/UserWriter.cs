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
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    [Obsolete("Should not be used directly.  Use AccountManager instead.")]
    internal static partial class UserWriter
    {
        private static SqlConfig _dbConn = null;

        static UserWriter()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }


    }
}
