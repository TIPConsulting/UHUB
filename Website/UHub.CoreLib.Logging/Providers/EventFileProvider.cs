using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging.Interfaces;
using UHub.CoreLib.Logging.Management;

namespace UHub.CoreLib.Logging.Providers
{
    internal sealed class EventFileProvider : IEventLogProvider
    {
        internal EventFileProvider()
        {

        }



        public bool CreateLog(EventLogData EventData)
        {
            var suffix = $"_{EventData.EventType.ToString()}";

            try
            {
                string path1 = LoggingFactory.Singleton.Properties.LogFileDirectory;
                string path2 = DateTime.UtcNow.ToString("yyyy-dd-MM_hh-mm-ss-fff") + suffix + ".txt";

                string path = Path.Combine(path1, path2);


                File.AppendAllText(path, EventData.ToString());

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
