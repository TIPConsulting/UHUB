using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Util;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_API
    {
        /// <summary>
        /// Register CMS API endpoints for user interaction
        /// Must be enabled for proper CMS API operation
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool RegisterAPIRoutes { get; set; } = false;
        /// <summary>
        /// Allow API to display detailed error messages regarding failures
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableDetailedAPIErrors { get; set; } = false;
        /// <summary>
        /// Allow API to display GUID codes regarding failuresS
        /// </summary>
        public bool EnableInternalAPIErrors { get; set; } = false;
        /// <summary>
        /// Allow clients to obtain auth tokens through the REST API
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableAPIAuthService { get; set; } = false;
        /// <summary>
        /// Allow users to upload files via API
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableAPIFileUploads { get; set; } = false;
        /// <summary>
        /// Maximum file size that can be uploaded by files (0 is infinite)
        /// <para></para>
        /// Default: 50 mB
        /// </summary>
        public FileSize MaxFileUploadSize { get; set; } = new FileSize(FileSizeUnit.Mebibyte, 50);
        /// <summary>
        /// List of allowable file categories. (Empty is all)
        /// <para></para>
        /// Default: empty set
        /// </summary>
        public IEnumerable<FileCategory> AllowedFileUploadTypes { get; set; } = new List<FileCategory>();
    }
}
