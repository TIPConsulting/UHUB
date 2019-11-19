using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Posts.Interfaces
{
    public interface IPost_C_Public
    {
        string Name { get; set; }

        string Content { get; set; }

        bool CanComment { get; set; }

        bool IsPublic { get; set; }

        long ParentID { get; set; }

    }
}
