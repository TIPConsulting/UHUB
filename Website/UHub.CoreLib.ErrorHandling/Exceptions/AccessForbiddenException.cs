using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.ErrorHandling.Exceptions
{
    /// <summary>
    /// CMS access forbidden exception. Thrown when user does not have permission to access a resource
    /// </summary>
    public sealed class AccessForbiddenException : Exception
    {
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        public AccessForbiddenException(string Message) : base(Message)
        {

        }

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        /// <param name="InnerException">Inner exception</param>
        public AccessForbiddenException(string Message, Exception InnerException) : base(Message, InnerException)
        {

        }
    }
}
