using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging.Interfaces
{
    public interface IEventLogProvider
    {


        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="Message"></param>
        bool CreateLog(EventLogData EventData);

    }
}
