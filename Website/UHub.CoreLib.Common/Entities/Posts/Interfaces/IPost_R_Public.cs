using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Posts.Interfaces
{
    public interface IPost_R_Public
    {
        long? ID { get; set; }

        bool IsReadOnly { get; set; }

        string Name { get; set; }

        string Content { get; set; }

        bool IsModified { get; set; }

        long ViewCount { get; set; }

        bool IsLocked { get; set; }

        bool CanComment { get; set; }

        bool IsPublic { get; set; }

        long ParentID { get; set; }

        long CreatedBy { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        long? ModifiedBy { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

    }
}
