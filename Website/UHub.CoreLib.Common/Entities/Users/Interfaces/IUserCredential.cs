using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Users.Interfaces
{
    public interface IUserCredential
    {
        string Email { get; set; }
        string Password { get; set; }

    }
}
