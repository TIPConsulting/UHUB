using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_Storage
    {
        /// <summary>
        /// Flag to set whether storage directories should be created if the specified directories cannot be found
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool CreateMissingStorageDirectories { get; set; } = false;
        /// <summary>
        /// File directory used for storing CMS files
        /// <para></para>
        /// Default: null
        /// </summary>
        public string FileStoreDirectory { get; set; } = null;
        /// <summary>
        /// File directory used for storing CMS images
        /// <para></para>
        /// Default: null
        /// </summary>
        public string ImageStoreDirectory { get; set; } = null;
        /// <summary>
        /// File directory used for storing temporary user file uploads
        /// <para></para>
        /// Default: null
        /// </summary>
        public string TempCacheDirectory { get; set; } = null;
    }
}
