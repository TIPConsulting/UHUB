using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    public enum LocalSysLoggingSource
    {
        /// <summary>
        /// Default option. Used to create logs under the standard "Application" folder.
        /// </summary>
        Application,
        /// <summary>
        /// Used to create logs under a specific UHUB_CMS folder.  Better organization, but may require greater privileges
        /// </summary>
        UHUB_CMS
    }
}
