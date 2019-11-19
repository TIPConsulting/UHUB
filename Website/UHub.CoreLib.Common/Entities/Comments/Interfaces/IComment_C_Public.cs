using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Comments.Interfaces
{
    public interface IComment_C_Public
    {
        string Content { get; set; }

        long ParentID { get; set; }


    }
}
