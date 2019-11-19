using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Schools.Interfaces
{
    public interface ISchool_R_Public
    {
        long? ID { get; set; }
        string Name { get; set; }
        string State { get; set; }
        string City { get; set; }
        bool IsEnabled { get; set; }
        bool IsReadOnly { get; set; }
        string Description { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        DateTimeOffset? ModifiedDate { get; set; }

    }
}
