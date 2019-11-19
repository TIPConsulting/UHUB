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
    public sealed partial class Post_U_PublicDTO : DtoEntityBase, IPost_U_Public
    {
        public long? ID { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool CanComment { get; set; }
        public bool IsPublic { get; set; }
    }
}
