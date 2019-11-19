using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging.Interfaces
{
    internal interface IUsageLogProvider
    {
        void CreatePageActionLog(string ResourceUrl, UsageLogData Data);

        void CreateApiActionLog(string ResourceUrl, UsageLogData Data);

        void CreateClientEventLog(string ResourceUrl, UsageLogType EventType, UsageLogData Data);

    }
}
