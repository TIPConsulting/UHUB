using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Comments.Interfaces
{
    public interface IComment_R_Public
    {
        long? ID { get; set; }
        
        bool IsEnabled { get; set; }

        bool IsReadOnly { get; set; }

        string Content { get; set; }

        bool IsModified { get; set; }

        long ViewCount { get; set; }

        long ParentID { get; set; }

        long CreatedBy { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        long? ModifiedBy { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }



    }
}
