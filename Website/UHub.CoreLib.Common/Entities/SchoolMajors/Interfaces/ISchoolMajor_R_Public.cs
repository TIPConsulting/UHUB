using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.SchoolMajors.Interfaces
{
    public interface ISchoolMajor_R_Public
    {
        string Name { get; set; }

        bool IsEnabled { get; set; }

        string Description { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

    }
}
