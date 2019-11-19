using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.ErrorHandling.Exceptions
{
    /// <summary>
    /// CMS invalid DB cast exception. Thrown when a DB result cannot be cast to a target class
    /// </summary>
    public sealed class InvalidDBCastException : Exception
    {
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        public InvalidDBCastException(string Message) : base(Message)
        {

        }

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        /// <param name="InnerException">Inner Exception</param>
        public InvalidDBCastException(string Message, Exception InnerException) : base(Message, InnerException)
        {

        }
    }
}
