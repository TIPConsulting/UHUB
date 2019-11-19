using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Comments.Interfaces
{
    public interface IComment : IComment_R_Public, IComment_C_Public
    {

        bool IsDeleted { get; set; }

    }
}
