using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Users.Interfaces
{
    public interface IUser_U_Private
    {
        long? ID { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        string Major { get; set; }
        string Year { get; set; }
        string ExpectedGradDate{ get; set; }
        string Company { get; set; }
        string JobTitle { get; set; }

    }
}
