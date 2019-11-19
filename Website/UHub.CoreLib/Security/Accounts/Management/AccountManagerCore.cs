using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SysSec = System.Web.Security;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Security.Accounts.Interfaces;

namespace UHub.CoreLib.Security.Accounts.Management
{
    public sealed partial class AccountManager : IAccountManager
    {

        private const short EMAIL_MIN_LEN = 3;
        private const short EMAIL_MAX_LEN = 250;
        private const short SALT_LEN = 50;
        private const short USER_VERSION_LEN = 10;
        private const short R_KEY_LEN = 20;
        private const string CONFIRMATION_URL_FORMAT = "{0}/{1}";



        public static string GetConfirmationURL(IUserConfirmToken ConfirmToken)
        {
            if (ConfirmToken == null)
            {
                return "";
            }
            else if (ConfirmToken.RefUID.IsEmpty())
            {
                return "";
            }
            else
            {

                var confirmUrlBase = CoreFactory.Singleton.Properties.AcctConfirmURL;
                var confirmUrlAdj = string.Format(CONFIRMATION_URL_FORMAT, confirmUrlBase, ConfirmToken.RefUID);

                return confirmUrlAdj;

            }
        }



    }
}
