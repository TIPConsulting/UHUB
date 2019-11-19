using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Security.Accounts.Interfaces;

namespace UHub.CoreLib.Security.Accounts
{
    [DataClass]
    public sealed partial class AccountConfirmToken : DBEntityBase, IUserConfirmToken
    {
        
        [DataProperty]
        public long UserID { get; set; }

        [DataProperty]
        public string RefUID { get; set; }

        [DataProperty]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty]
        public DateTimeOffset? ConfirmedDate{ get; set; }

        [DataProperty]
        public bool IsConfirmed { get; set; }

        [DataProperty]
        public bool IsDeleted { get; set; }

    }
}
