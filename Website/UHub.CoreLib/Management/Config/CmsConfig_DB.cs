using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_DB
    {
        /// <summary>
        /// Connection information for default CMS DB
        /// <para></para>
        /// Default: null
        /// </summary>
        public SqlConfig CmsDBConfig { get; set; } = null;
        /// <summary>
        /// Allow mutlithreaded DB processing. Might improve DB read times depending on server configuration
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableDBMultithreading { get; set; } = false;
    }
}
