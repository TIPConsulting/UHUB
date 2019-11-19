using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;

namespace UHub.CoreLib.Entities.Images
{
    [DataClass]
    public sealed partial class Image : DBEntityBase
    {
        [DataProperty]
        public long? ID { get; set; }

        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadOnly { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public long ViewCount { get; set; }

        [DataProperty]
        public string FilePath { get; set; }

        [DataProperty]
        public string Description { get; set; }

        [DataProperty]
        public string FileHash_SHA256 { get; set; }

        [DataProperty]
        public string SourceName { get; set; }

        [DataProperty]
        public string SourceType { get; set; }

        [DataProperty]
        public string DownloadName { get; set; }

        [DataProperty]
        public string ParentID { get; set; }

        [DataProperty]
        public bool IsDeleted { get; set; }

        [DataProperty]
        public long CreatedBy { get; set; }

        [DataProperty]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty]
        public long? ModifiedBy { get; set; }

        [DataProperty]
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
