using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.Users.Interfaces;

namespace UHub.CoreLib.Entities.Users.DTOs
{
    [DtoClass(typeof(User))]
    public sealed partial class User_U_PrivateDTO : DtoEntityBase, IUser_U_Private
    {
        public long? ID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Major { get; set; }
        public string Year { get; set; }
        public string ExpectedGradDate { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }

    }
}
