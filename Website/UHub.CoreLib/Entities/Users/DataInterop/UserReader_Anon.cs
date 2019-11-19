using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users.Interfaces;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    public static partial class UserReader
    {

        public static User GetAnonymousUser()
        {

            return new User()
            {
                ID = null,
                Username = "Anon"

            };


        }

    }
}
