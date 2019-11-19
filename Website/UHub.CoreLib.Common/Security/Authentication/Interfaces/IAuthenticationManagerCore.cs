using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;

namespace UHub.CoreLib.Security.Authentication.Interfaces
{
    /// <summary>
    /// Authentication manager interface to control method exposure
    /// </summary>
    public partial interface IAuthenticationManager
    {

        /// <summary>
        /// Log out of all devices by changing user version
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ExcludeCurrent"></param>
        void LogoutOfAllDevices(long UserID, HttpContext Context, bool ExcludeCurrent = false);
        /// <summary>
        /// Log out of all devices by changing user version
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="ExcludeCurrent"></param>
        void LogoutOfAllDevices(string Email, HttpContext Context, bool ExcludeCurrent = false);
        /// <summary>
        /// Log out of all devices by changing user version
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Domain"></param>
        /// <param name="ExcludeCurrent"></param>
        void LogoutOfAllDevices(string Username, string Domain, HttpContext Context, bool ExcludeCurrent = false);



        /// <summary>
        /// Remove persistent cookies from request/response
        /// </summary>
        void TryLogOut(HttpContext Context);

    }
}
