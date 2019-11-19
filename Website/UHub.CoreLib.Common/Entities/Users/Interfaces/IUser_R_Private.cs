using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Users.Interfaces
{
    /// <summary>
    /// CMS User publically exposable interface for self-referencing access
    /// </summary>
    public interface IUser_R_Private : IUser_R_Public
    {
        string Email { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        long? SchoolID { get; set; }

    }
}
