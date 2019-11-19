using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.ErrorHandling.Exceptions
{
    /// <summary>
    /// CMS entity gone exception. Thrown when entity cannot be found due to move/deletion
    /// </summary>
    public sealed class EntityGoneException : Exception
    {
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        public EntityGoneException(string Message) : base(Message)
        {
            
        }

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="Message">Exception message</param>
        /// <param name="InnerException">Inner exception</param>
        public EntityGoneException(string Message, Exception InnerException) : base(Message, InnerException)
        {

        }
    }
}
