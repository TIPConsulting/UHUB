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
    public sealed partial class Comment_C_PublicDTO : DtoEntityBase, IComment_C_Public
    {
        public string Content { get; set; }
        public long ParentID { get; set; }
    }
}
