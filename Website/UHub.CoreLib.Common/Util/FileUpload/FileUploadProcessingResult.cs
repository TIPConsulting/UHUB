using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Util.APIControllers.FileUpload
{
    public sealed class UploadProcessingResult
    {
        /// <summary>
        /// Flag signifying whether a chunked upload is complete
        /// </summary>
        public bool IsComplete { get; set; }
        /// <summary>
        /// Original client file name without extension
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Original file extension (ex ".png")
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// Original declared MIME type
        /// </summary>
        public string FileMIME { get; set; }
        /// <summary>
        /// Internal server file path
        /// </summary>
        public string LocalFilePath { get; set; }

        public NameValueCollection FileMetadata { get; set; }
    }
}
