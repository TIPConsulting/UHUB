using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.ErrorHandling.Exceptions
{
    /// <summary>
    /// CMS configuration exception. Thrown when there is an invalid system configuration
    /// </summary>
    public sealed class ConfigurationException : Exception
    {
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        public ConfigurationException(string Message) : base(Message)
        {

        }

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        /// <param name="InnerException">Inner exception</param>
        public ConfigurationException(string Message, Exception InnerException) : base(Message, InnerException)
        {

        }
    }
}
