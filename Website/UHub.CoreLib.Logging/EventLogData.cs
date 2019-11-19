using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    public sealed class EventLogData
    {
        public EventType EventType { get; set; }
        public string EventID { get; set; }
        public string Content { get; set; }
        public long? CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }


        /// <summary>
        /// Custom string overload
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var msgBuilder = new StringBuilder();
            msgBuilder.AppendFormat("{0:o}", this.CreatedDate);
            msgBuilder.AppendLine();
            msgBuilder.AppendFormat("Event Type: {0}", this.EventType.ToString());
            msgBuilder.AppendLine();
            msgBuilder.AppendFormat("Event ID: {0}", this.EventID);
            msgBuilder.AppendLine();
            msgBuilder.AppendFormat("Created By: {0}", this.CreatedBy);
            msgBuilder.AppendLine();
            msgBuilder.AppendLine();
            msgBuilder.Append(this.Content);
            msgBuilder.AppendLine();


            return msgBuilder.ToString();
        }

    }
}
