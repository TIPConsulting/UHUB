using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_Logging
    {
        /// <summary>
        /// Logging target mode.  Defines drop zone for log messages
        /// <para></para>
        /// Default: LocalFile
        /// </summary>
        public EventLoggingMode EventLogMode { get; set; } = EventLoggingMode.File;
        public bool CreateMissingDirectories { get; set; } = false;
        public string LogFileDirectory { get; set; }
        /// <summary>
        /// Name of log folder if using SystemEvents
        /// <para></para>
        /// Default: Application
        /// </summary>
        public LocalSysLoggingSource LoggingSource { get; set; } = LocalSysLoggingSource.Application;

        public UsageLoggingMode UsageLogMode { get; set; } = UsageLoggingMode.None;
        /// <summary>
        /// Key for google analytics tracking
        /// </summary>
        public string GoogleAnalyticsKey { get; set; }
    }
}
