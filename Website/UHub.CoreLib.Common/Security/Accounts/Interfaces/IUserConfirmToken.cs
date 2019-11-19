using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Accounts.Interfaces
{
    public interface IUserConfirmToken
    {
        long UserID { get; set; }

        string RefUID { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        DateTimeOffset? ConfirmedDate { get; set; }

        bool IsConfirmed { get; set; }

        bool IsDeleted { get; set; }

    }
}
