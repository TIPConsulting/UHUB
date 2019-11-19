using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Logging.Interfaces;
using UHub.CoreLib.Logging.Util;

namespace UHub.CoreLib.Logging.Management
{
    // <summary>
    // Manage system logging | core init
    // </summary>
    public sealed partial class LoggingManager
    {

        
        public async Task CreatePageActionLogAsync(string Url, long? UserID = null)
        {
            var data = LoggingHelpers.GetUserClientData(UserID);

            await Task.Run(() => usageProviders.ForEach(x => x.CreatePageActionLog(Url, data)));
        }


        public async Task CreateApiActionLogAsync(string Url, long? UserID = null)
        {
            var data = LoggingHelpers.GetUserClientData(UserID);

            await Task.Run(() => usageProviders.ForEach(x => x.CreateApiActionLog(Url, data)));
        }


        public async Task CreateApiActionLogAsync(string Url, UsageLogType EventType, long? UserID = null)
        {
            var data = LoggingHelpers.GetUserClientData(UserID);

            await Task.Run(() => usageProviders.ForEach(x => x.CreateClientEventLog(Url, EventType, data)));
        }


    }
}
