using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    [Flags]
    public enum EventLoggingMode
    {
        None = 0,

        File = 1,
        LocalConsole = 2,
        LocalSystemEvents = 4,
        Database = 8,

        All = File | LocalConsole | LocalSystemEvents | Database
    }
}
