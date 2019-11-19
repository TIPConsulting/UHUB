using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.ErrorHandling.Exceptions
{
    /// <summary>
    /// CMS system disabled exception. Thrown when the CMS system is disabled
    /// </summary>
    public sealed class SystemDisabledException : Exception
    {

        /// <summary>
        /// Initializer
        /// </summary>
        public SystemDisabledException() : base("The system has been disabled.  Contact the developer for further details")
        {
            
        }
    }
}
