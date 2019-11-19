using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Logging.Management
{
    public class LoggingConfig
    {

        public LoggingConfig() { }
        internal LoggingConfig(LoggingConfig Config)
        {
            this.ApplicationFriendlyName = Config.ApplicationFriendlyName;
            this.ApplicationVersionNumber = Config.ApplicationVersionNumber;
            this.SqlDBConfig = Config.SqlDBConfig;
            this.EventLogMode = Config.EventLogMode;
            this.CreateMissingDirectories = Config.CreateMissingDirectories;
            this.LogFileDirectory = Config.LogFileDirectory;
            this.LocalSysLoggingSource = Config.LocalSysLoggingSource;
            this.UsageLogMode = Config.UsageLogMode;
            this.GoogleAnalyticsKey = Config.GoogleAnalyticsKey;
        }




        public string ApplicationFriendlyName { get; set; }
        public string ApplicationVersionNumber { get; set; }


        public SqlConfig SqlDBConfig { get; set; }

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
        public LocalSysLoggingSource LocalSysLoggingSource { get; set; } = LocalSysLoggingSource.Application;

        public UsageLoggingMode UsageLogMode { get; set; } = UsageLoggingMode.None;
        /// <summary>
        /// Key for google analytics tracking
        /// </summary>
        public string GoogleAnalyticsKey { get; set; }




        /// <summary>
        /// Attempt to validate file and database connections
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Validate()
        {
            var result = true;


            var taskValDB = ValidateDatabase();


            result &= ValidateLogDirectory();


            if (UsageLogMode == UsageLoggingMode.GoogleAnalytics)
            {
                result &= GoogleAnalyticsKey.IsNotEmpty();
            }


            result &= await taskValDB;
            return result;
        }


        private bool ValidateLogDirectory()
        {

            if ((EventLogMode & EventLoggingMode.File) != 0)
            {

                try
                {
                    //validatae
                    ConfigValidators.ValidateDirectory(LogFileDirectory, nameof(LogFileDirectory), CreateMissingDirectories);
                    //Convert to absolute
                    LogFileDirectory = Converters.GetAbsFileDirectory(LogFileDirectory);




                    var fileID = Guid.NewGuid().ToString();
                    var fileName = Path.Combine(LogFileDirectory, fileID + ".tmp");


                    using (File.Create(fileName)) ;
                    File.Delete(fileName);

                    return true;
                }
                catch
                {
                    return false;
                }

            }
            return true;
        }

        private async Task<bool> ValidateDatabase()
        {
            if (!SqlDBConfig.ValidateConnection())
            {
                return false;
            }

            var taskList = new List<Task<bool>>();

            taskList.Add(SqlHelper.DoesTableExistAsync(SqlDBConfig, "dbo.EventLog"));
            taskList.Add(SqlHelper.DoesTableExistAsync(SqlDBConfig, "dbo.EventLogTypes"));
            taskList.Add(SqlHelper.DoesTableExistAsync(SqlDBConfig, "dbo.SessionLog"));
            taskList.Add(SqlHelper.DoesTableExistAsync(SqlDBConfig, "dbo.LocationLogXRef"));
            taskList.Add(SqlHelper.DoesTableExistAsync(SqlDBConfig, "dbo.LocationLogTypes"));


            await Task.WhenAll(taskList);


            return taskList.All(x => x.Result);


        }
    }
}
