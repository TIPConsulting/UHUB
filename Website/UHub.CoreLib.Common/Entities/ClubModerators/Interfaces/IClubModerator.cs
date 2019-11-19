using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.ClubModerators.Interfaces
{
    public interface IClubModerator : IClubModerator_C_Public, IClubModerator_R_Public
    {
        long? ID { get; set; }

        bool IsReadOnly { get; set; }

        bool IsValid { get; set; }


        bool IsDeleted { get; set; }


        long CreatedBy { get; set; }


        long? ModifiedBy { get; set; }


        DateTimeOffset? ModifiedDate { get; set; }

    }
}
