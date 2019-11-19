using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    [Flags]
    public enum UsageLoggingMode
    {
        None = 0,

        [Obsolete("Not Implemented")]
        Database = 1,
        GoogleAnalytics = 2,

        All = GoogleAnalytics
    }
}
