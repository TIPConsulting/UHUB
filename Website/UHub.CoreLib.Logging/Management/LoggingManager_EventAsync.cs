using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Logging.Interfaces;

namespace UHub.CoreLib.Logging.Management
{
    // <summary>
    // Manage system logging | local server events
    // </summary>
    public sealed partial class LoggingManager
    {


        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateSuccessLogAsync<T>(T Data)
        {
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = uid,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateSuccessLogAsync<T>(T Data, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = UID.ToString(),
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateSuccessLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateSuccessLogAsync(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Success,
                EventID = UID.ToString(),
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }


        /// <summary>
        /// Create information message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateInfoLogAsync<T>(T Data)
        {
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = uid,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create information message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateInfoLogAsync<T>(T Data, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = UID.ToString(),
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create information message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateInfoLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create information message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateInfoLogAsync(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Information,
                EventID = UID.ToString(),
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }


        /// <summary>
        /// Create warning message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateWarningLogAsync<T>(T Data)
        {
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = uid,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create warning message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateWarningLogAsync<T>(T Data, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = UID.ToString(),
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create warning message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateWarningLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create warning message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateWarningLogAsync(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Warning,
                EventID = UID.ToString(),
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }



        /// <summary>
        /// Create failure message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateFailureLogAsync<T>(T Data)
        {
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }

            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = uid,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create failure message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateFailureLogAsync<T>(T Data, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = UID.ToString(),
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateFailureLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateFailureLogAsync(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Failure,
                EventID = UID.ToString(),
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }


        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateErrorLogAsync<T>(T Data)
        {
            EventLogData eventData = null;
            string content = "";
            string uid = null;

            if (Data is Exception ex)
            {
                content = ex.ToString();
                uid = ex.ToString();
            }
            else if (Data is Guid)
            {
                uid = Data.ToString();
            }
            else
            {
                content = Data.ToFormattedJSON();
            }

            eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = uid,
                Content = content,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateErrorLogAsync<T>(T Data, Guid UID)
        {
            EventLogData eventData = null;
            string content = "";

            if (Data is Exception ex)
            {
                content = ex.ToString();
            }
            else
            {
                content = Data.ToFormattedJSON();
            }

            eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = UID.ToString(),
                Content = content,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateErrorLogAsync(string Message)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create error message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateErrorLogAsync(string Message, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType.Error,
                EventID = UID.ToString(),
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }




        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateEventLogAsync<T>(T Data, EventType EventType)
        {
            string uid = null;
            if (Data is Guid)
            {
                uid = Data.ToString();
            }


            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = uid,
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create success message using anonymous type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task CreateEventLogAsync<T>(T Data, EventType EventType, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = UID.ToString(),
                Content = Data.ToFormattedJSON(),
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateEventLogAsync(string Message, EventType EventType)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = null,
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }
        /// <summary>
        /// Create success message
        /// </summary>
        /// <param name="message"></param>
        public async Task CreateEventLogAsync(string Message, EventType EventType, Guid UID)
        {
            var eventData = new EventLogData
            {
                EventType = EventType,
                EventID = UID.ToString(),
                Content = Message,
                CreatedBy = null,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await CreateEventLogAsync(eventData);
        }


        public async Task CreateEventLogAsync(EventLogData EventData)
        {
            await Task.Run(() => eventProviders.ForEach(x => x.CreateLog(EventData)));
        }

    }
}
