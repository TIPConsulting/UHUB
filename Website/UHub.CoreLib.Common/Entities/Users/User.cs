using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.Users.Interfaces;


namespace UHub.CoreLib.Entities.Users
{

    [DataClass]
    public sealed partial class User : DBEntityBase, IUserCredential, IUser_C_Public, IUser_R_Private, IUser_U_Private
    {
        private const short USER_VERSION_LENGTH = 10;


        [DataProperty]
        public long? ID { get; set; }

        //NOT A DATA PROP
        public string Password { get; set; }

        /// <summary>
        /// Simple DB attribute
        /// </summary>
        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadOnly { get; set; }

        [DataProperty]
        public bool IsConfirmed { get; set; }

        [DataProperty]
        public bool IsApproved { get; set; }

        [DataProperty]
        public string Version { get; set; }

        [DataProperty]
        public bool IsAdmin { get; private set; } = false;

        [DataProperty]
        public string Email { get; set; }

        [DataProperty]
        public string Username { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public string PhoneNumber { get; set; }

        [DataProperty]
        public string Major { get; set; }

        [DataProperty]
        public string Year { get; set; }

        [DataProperty]
        public string ExpectedGradDate { get; set; }

        [DataProperty]
        public string Company { get; set; }

        [DataProperty]
        public string JobTitle { get; set; }

        [DataProperty]
        public bool IsFinished { get; set; }

        [DataProperty(DBNativeName = "ParentID")]
        public long? SchoolID { get; set; }

        [DataProperty]
        public long CreatedBy { get; set; }

        [DataProperty]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty]
        public long? ModifiedBy { get; set; }

        [DataProperty]
        public DateTimeOffset? ModifiedDate { get; set; }


        /// <summary>
        /// Aggregate validation property
        /// </summary>
        public bool IsFullyEnabled
        {
            get
            {
                return IsConfirmed && IsApproved && IsEnabled;
            }
        }
    }
}
