using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.Posts.Interfaces;

namespace UHub.CoreLib.Entities.Posts.DTOs
{
    [DtoClass(typeof(Post))]
    public sealed partial class Post_R_PublicDTO : DtoEntityBase, IPost_R_Public
    {
        public long? ID { get; set; }
        public bool IsReadOnly { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsModified { get; set; }
        public long ViewCount { get; set; }
        public bool IsLocked { get; set; }
        public bool CanComment { get; set; }
        public bool IsPublic { get; set; }
        public long ParentID { get; set; }
        public long CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
