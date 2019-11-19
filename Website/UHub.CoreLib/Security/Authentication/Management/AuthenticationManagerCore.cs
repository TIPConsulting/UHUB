using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security.Authentication.Interfaces;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Security.Authentication.Providers;
using UHub.CoreLib.Security.Authentication.Providers.Forms;

namespace UHub.CoreLib.Security.Authentication.Management
{

    internal sealed partial class AuthenticationManager : IAuthenticationManager
    {
        /// <summary>
        /// Request variable to store the current user.  Used as temp caching to prevent extra lookups
        /// </summary>
        internal const string REQUEST_CURRENT_USER = "Global_CurrentUser";


        private AuthenticationProvider authWorker;
        internal AuthenticationManager()
        {
            authWorker = new FormsAuthProvider();
        }




        /// <summary>
        /// Logout of all devices by incrementing user version
        /// </summary>
        public void LogoutOfAllDevices(long UserID, HttpContext Context, bool ExcludeCurrent = false)
        {
            var modUser = UserReader.GetUser(UserID);
            if (modUser == null || modUser.ID == null)
            {
                return;
            }

            CoreFactory.Singleton.Accounts.TryUpdateUserVersion(modUser.ID.Value);
            if (ExcludeCurrent)
            {
                try
                {
                    authWorker.SetCurrentUser_ClientToken(false, modUser, Context);
                }
                catch
                {
                    //might throw SqlException for unique key violation
                    //no need to to process error
                    //user will simply be logged out
                }

            }
        }
        /// <summary>
        /// Logout of all devices by incrementing user version
        /// </summary>
        public void LogoutOfAllDevices(string Email, HttpContext Context, bool ExcludeCurrent = false)
        {
            var modUser = UserReader.GetUser(Email);
            if (modUser == null || modUser.ID == null)
            {
                return;
            }

            CoreFactory.Singleton.Accounts.TryUpdateUserVersion(modUser.ID.Value);
            if (ExcludeCurrent)
            {
                try
                {
                    authWorker.SetCurrentUser_ClientToken(false, modUser, Context);
                }
                catch
                {
                    //might throw SqlException for unique key violation
                    //no need to to process error
                    //user will simply be logged out
                }
            }
        }
        /// <summary>
        /// Logout of all devices by incrementing user version
        /// </summary>
        public void LogoutOfAllDevices(string Username, string Domain, HttpContext Context, bool ExcludeCurrent = false)
        {
            var modUser = UserReader.GetUser(Username, Domain);
            if (modUser == null || modUser.ID == null)
            {
                return;
            }

            CoreFactory.Singleton.Accounts.TryUpdateUserVersion(modUser.ID.Value);
            if (ExcludeCurrent)
            {
                try
                {
                    authWorker.SetCurrentUser_ClientToken(false, modUser, Context);
                }
                catch
                {
                    //might throw SqlException for unique key violation
                    //no need to to process error
                    //user will simply be logged out
                }
            }
        }


        /// <summary>
        /// Remove persistent cookies from request/response
        /// </summary>
        public void TryLogOut(HttpContext Context)
        {
            TryLogOut(Context, -1);
        }
        /// <summary>
        /// Remove persistent cookies from request/response
        /// </summary>
        internal void TryLogOut(HttpContext Context, int ErrorCode = -1, [CallerMemberName] string key = null)
        {
            bool DEBUG = false;
            if (ErrorCode != -1 && DEBUG)
            {
                CoreFactory.Singleton.Logging.CreateInfoLog(ErrorCode.ToString());
            }
            if (key != null && DEBUG)
            {
                CoreFactory.Singleton.Logging.CreateInfoLog(key.ToString());
            }

            try
            {

                //get cookie
                var authCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;
                HttpCookie authCookie = Context.Request.Cookies[authCookieName] ?? Context.Response.Cookies[authCookieName];


                if (authCookie != null && authCookie.Value.IsNotEmpty())
                {
                    AuthenticationToken authToken = null;
                    try
                    {
                        authToken = AuthenticationToken.Decrypt(authCookie.Value);
                        if (authToken != null)
                        {
                            TokenManager.RevokeTokenValidator(authToken);
                        }
                    }
                    catch
                    {
                        //Do nothing
                    }
                }


                Context.Items[REQUEST_CURRENT_USER] = null;
                if (Context.Request.Cookies[authCookieName] != null && Context.Request.Cookies[authCookieName].Value.IsNotEmpty())
                {
                    Context.Request.Cookies.Remove(authCookieName);
                    Context.Response.Cookies.Remove(authCookieName);
                    Context.Request.Cookies[authCookieName]?.Expire();
                }
                if (Context.Response.Cookies[authCookieName] != null && Context.Response.Cookies[authCookieName].Value.IsNotEmpty())
                {
                    Context.Request.Cookies.Remove(authCookieName);
                    Context.Response.Cookies.Remove(authCookieName);
                    Context.Response.Cookies[authCookieName]?.Expire();
                }


            }
            catch { }

        }

    }
}
