using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Logging.Interfaces;
using UHub.CoreLib.Logging.Providers;

namespace UHub.CoreLib.Logging.Management
{
    // <summary>
    // Manage system logging | core init
    // </summary>
    public sealed partial class LoggingManager
    {
        private List<IEventLogProvider> eventProviders;
        private List<IUsageLogProvider> usageProviders;

        public LoggingConfig Properties { get; }


        internal LoggingManager(LoggingConfig Config)
        {
            Properties = new LoggingConfig(Config);


            eventProviders = new List<IEventLogProvider>();
            usageProviders = new List<IUsageLogProvider>();


            //EVENTS
            if ((Config.EventLogMode & EventLoggingMode.File) != 0)
            {
                var fileProvider = new EventFileProvider();
                eventProviders.Add(fileProvider);
            }
            if ((Config.EventLogMode & EventLoggingMode.LocalSystemEvents) != 0)
            {
                var logSrc = Config.LocalSysLoggingSource;
                var fName = Config.ApplicationFriendlyName;
                var eventProvider = new EventLocalSysProvider(logSrc, fName);

                eventProviders.Add(eventProvider);
            }
            if ((Config.EventLogMode & EventLoggingMode.Database) != 0)
            {
                var dbProvider = new EventDatabaseProvider();
                eventProviders.Add(dbProvider);
            }


            //USAGE
            if ((Config.UsageLogMode & UsageLoggingMode.GoogleAnalytics) != 0)
            {
                var googleProvider = new UsageGAnalyticsProvider();

                usageProviders.Add(googleProvider);
            }

        }



        public void AddProvider(IEventLogProvider LocalLogProvider)
        {
            eventProviders.Add(LocalLogProvider);
        }
        private void AddProvider(IUsageLogProvider UsageLogProvider)
        {
            usageProviders.Add(UsageLogProvider);
        }


    }
}
