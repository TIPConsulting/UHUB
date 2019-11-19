using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.Comments.Interfaces;

namespace UHub.CoreLib.Entities.Comments
{
    [DataClass]
    public sealed partial class Comment : DBEntityBase, IComment
    {
        [DataProperty]
        public long? ID { get; set; }

        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadOnly { get; set; }

        [DataProperty]
        public string Content { get; set; }

        [DataProperty]
        public bool IsModified { get; set; }

        [DataProperty]
        public long ViewCount { get; set; }

        [DataProperty]
        public long ParentID { get; set; }

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
