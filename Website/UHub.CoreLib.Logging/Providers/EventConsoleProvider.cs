using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging.DataInterop;
using UHub.CoreLib.Logging.Interfaces;

namespace UHub.CoreLib.Logging.Providers
{
    internal sealed class EventConsoleProvider : IEventLogProvider
    {
#pragma warning disable
        public bool CreateLog(EventLogData EventData)
        {
            Console.WriteLine(EventData.ToString());


            return true;
        }
#pragma warning restore

    }
}
