using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.SchoolClubs.Interfaces
{
    public interface ISchoolClub_R_Public
    {
        long? ID { get; set; }

        long SchoolID { get; set; }

        bool IsEnabled { get; set; }

        bool IsReadOnly { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

    }
}
