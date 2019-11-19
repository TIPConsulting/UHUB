using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_Errors
    {
        /// <summary>
        /// Allows error pages to show error status (otherwise all error pages return 200)
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableCustomErrorCodes { get; set; } = false;
    }
}
