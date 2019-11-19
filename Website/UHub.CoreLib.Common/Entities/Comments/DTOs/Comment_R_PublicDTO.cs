using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.Comments.Interfaces;

namespace UHub.CoreLib.Entities.Comments.DTOs
{
    [DtoClass(typeof(Comment))]
    public sealed partial class Comment_R_PublicDTO : DtoEntityBase, IComment_R_Public
    {
        public long? ID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsReadOnly { get; set; }
        public string Content { get; set; }
        public bool IsModified { get; set; }
        public long ViewCount { get; set; }
        public long ParentID { get; set; }
        public long CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
