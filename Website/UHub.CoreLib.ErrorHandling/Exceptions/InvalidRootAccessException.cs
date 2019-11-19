using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.ErrorHandling.Exceptions
{
    /// <summary>
    /// CMS invalid root access exception. Thrown when ent root cannot be accessed
    /// </summary>
    public sealed class InvalidRootAccessException : Exception
    {
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        public InvalidRootAccessException(string Message) : base(Message)
        {

        }

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        /// <param name="InnerException">Inner Exception</param>
        public InvalidRootAccessException(string Message, Exception InnerException) : base(Message, InnerException)
        {

        }
    }
}
