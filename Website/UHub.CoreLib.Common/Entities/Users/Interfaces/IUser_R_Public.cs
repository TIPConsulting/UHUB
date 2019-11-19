using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Users.Interfaces
{
    /// <summary>
    /// CMS User publically exposable interface
    /// </summary>
    public interface IUser_R_Public
    {
        long? ID { get; set; }
        string Username { get; }
        string Major { get; set; }
        string Year { get; set; }
        string ExpectedGradDate { get; set; }
        string Company { get; set; }
        string JobTitle { get; set; }


    }
}
