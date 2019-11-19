using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging.Interfaces;

namespace UHub.CoreLib.Logging.Providers
{
    internal sealed class EventLocalSysProvider : IEventLogProvider
    {
        //EVENT FOLDER
        private const string uhubEventSource = "UHUB-CMS";
        /// <summary>
        /// Event "folder"
        /// </summary>
        private string LogNameAdj { get; }
        /// <summary>
        /// Event source
        /// </summary>
        private string EventSourceAdj { get; }

        internal EventLocalSysProvider(LocalSysLoggingSource LogSrc, string SiteFriendlyName)
        {

            if (LogSrc == LocalSysLoggingSource.Application)
            {
                LogNameAdj = "Application";
                EventSourceAdj = uhubEventSource + ": " + SiteFriendlyName;
            }
            else
            {
                LogNameAdj = uhubEventSource;
                EventSourceAdj = SiteFriendlyName;
            }

        }

        public bool CreateLog(EventLogData EventData)
        {
            try
            {

                EventLogEntryType SysEventType = EventLogEntryType.Information;
                short category = 0;
                short eventID = 0;
                if (EventData.EventType == EventType.Success)
                {
                    SysEventType = EventLogEntryType.SuccessAudit;
                    category = 1;
                    eventID = 101;
                }
                else if (EventData.EventType == EventType.Information)
                {
                    SysEventType = EventLogEntryType.Information;
                    category = 2;
                    eventID = 201;
                }
                else if (EventData.EventType == EventType.Warning)
                {
                    SysEventType = EventLogEntryType.Warning;
                    category = 3;
                    eventID = 301;
                }
                else if (EventData.EventType == EventType.Failure)
                {
                    SysEventType = EventLogEntryType.FailureAudit;
                    category = 4;
                    eventID = 401;
                }
                else if (EventData.EventType == EventType.Error)
                {
                    SysEventType = EventLogEntryType.Error;
                    category = 5;
                    eventID = 501;
                }



                using (EventLog eventLog = new EventLog(LogNameAdj))
                {
                    eventLog.Source = EventSourceAdj;
                    eventLog.WriteEntry(EventData.ToString(), SysEventType, eventID, category);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
