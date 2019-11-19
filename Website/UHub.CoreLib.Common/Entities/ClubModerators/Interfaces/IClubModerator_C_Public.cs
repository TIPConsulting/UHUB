using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.ClubModerators.Interfaces
{
    public interface IClubModerator_C_Public
    {
        long UserID { get; set; }
        bool IsOwner { get; set; }
        bool IsEnabled { get; set; }
    }
}
