using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Security.Accounts;
using UHub.CoreLib.Security.Authentication;
using UHub.CoreLib.EmailInterop;
using UHub.CoreLib.EmailInterop.Templates;
using UHub.CoreLib.Security.Recovery.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.Controllers
{
    public class AccountController : Controller
    {
        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult Index()
        {
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;

            var userName = cmsUser.Username;

            ViewBag.Username = userName.HtmlEncode();

            return View();
        }


        [System.Web.Mvc.HttpGet]
        public ActionResult Create()
        {
            return View();
        }



        [System.Web.Mvc.HttpGet]
        public ActionResult Login()
        {
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;
            if (cmsUser != null && cmsUser.ID != null)
            {
                var url = CoreFactory.Singleton.Properties.DefaultAuthFwdURL;
                return Redirect(url);
            }

            return View();
        }


        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Login([FromBody] User_CredentialDTO creds)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var context = System.Web.HttpContext.Current;
            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(context);
                if (!isCaptchaValid)
                {
                    ViewBag.ErrorMsg = "Captcha is not valid";
                    return View();
                }
            }



            var email = creds.Email;
            var pswd = creds.Password;

            var statusMsg = "An unknown authentication error has occured";
            var ResultCode = CoreFactory.Singleton.Auth.TrySetClientAuthToken(
                email,
                pswd,
                false);

            var isValid = (ResultCode == AuthResultCode.Success);

            switch (ResultCode)
            {
                case AuthResultCode.EmailEmpty:
                    {
                        statusMsg = "Email cannot be Empty";
                        break;
                    }
                case AuthResultCode.EmailInvalid:
                    {
                        statusMsg = "Email address is invalid";
                        break;
                    }
                case AuthResultCode.PswdEmpty:
                    {
                        statusMsg = "Password cannot be empty";
                        break;
                    }
                case AuthResultCode.PswdInvalid:
                    {
                        statusMsg = "Password is invalid";
                        break;
                    }
                case AuthResultCode.PswdExpired:
                    {
                        statusMsg = "Password has expired - please reset and try again";
                        break;
                    }
                case AuthResultCode.UserInvalid:
                    {
                        statusMsg = "User account is invalid";
                        break;
                    }
                case AuthResultCode.UserLocked:
                    {
                        statusMsg = "User account is currently locked - please try again later";
                        break;
                    }
                case AuthResultCode.UserDisabled:
                    {
                        statusMsg = "User account is disabled";
                        break;
                    }
                case AuthResultCode.PendingApproval:
                    {
                        statusMsg = "User account is pending admin approval";
                        break;
                    }
                case AuthResultCode.PendingConfirmation:
                    {
                        statusMsg = "User account is pending email confirmation";
                        break;
                    }
                case AuthResultCode.CredentialsInvalid:
                    {
                        statusMsg = "User credentials are invalid";
                        break;
                    }
                case AuthResultCode.Success:
                    {
                        statusMsg = "Success";
                        break;
                    }
            }



            if (isValid)
            {
                var url = CoreFactory.Singleton.Auth.GetAuthForwardUrl();

                var cookieName = CoreFactory.Singleton.Properties.PostAuthCookieName;
                Request.Cookies[cookieName]?.Expire();
                Response.Cookies[cookieName]?.Expire();

                return Redirect(url);
            }
            else
            {
                ViewBag.ErrorMsg = statusMsg;

                return View();
            }
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult Logout()
        {
            var context = System.Web.HttpContext.Current;
            CoreFactory.Singleton.Auth.TryLogOut(context);


            return View();
        }


        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Confirm()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            if (idObj == null)
            {
                ViewBag.Message = "Unable to complete operation - must provide confirmation key";
                return View();
            }
            var idStr = idObj.ToString();


            if (idStr.ToLower() == "new")
            {
                ViewBag.Message = "Your new account has been created, please check your email and follow the confirmation instructions";
                return View();
            }



            var confResult = await CoreFactory.Singleton.Accounts.TryConfirmUserAsync(idStr);
            if (confResult == 0)
            {
                ViewBag.Message = "User account has been successfully confirmed.  Please return home to log in";
            }
            else
            {
                ViewBag.Message = "User confirmation failed - " + confResult.ToString();
            }

            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult UpdatePassword()
        {
            ViewBag.CanForward = false;
            return View();
        }


        [System.Web.Mvc.HttpPost]
        [MvcAuthControl]
        public async Task<ActionResult> UpdatePassword(string txt_CurrentPswd, string txt_NewPswd, string txt_ConfirmPswd, bool chk_DeviceLogout = false)
        {
            ViewBag.CanForward = false;


            var context = System.Web.HttpContext.Current;
            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(context);
                if (!isCaptchaValid)
                {
                    ViewBag.Message = "Captcha is not valid";
                    return View();
                }
            }


            if (txt_NewPswd != txt_ConfirmPswd)
            {
                ViewBag.Message = "Passwords must match";
                return View();
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;

            var result = await CoreFactory.Singleton.Accounts.TryUpdatePasswordAsync(
                cmsUser.ID.Value,
                txt_CurrentPswd,
                txt_NewPswd,
                chk_DeviceLogout,
                context);


            ViewBag.CanForward = (result == 0);
            ViewBag.Message = result.ToString();

            return View();

        }



        [System.Web.Mvc.HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }



        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> ForgotPassword(string txt_Email)
        {
            if (!Validators.IsValidEmail(txt_Email))
            {
                ViewBag.Message = "Email is not valid";
                return View();
            }


            var context = System.Web.HttpContext.Current;
            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(context);
                if (!isCaptchaValid)
                {
                    ViewBag.ErrorMsg = "Captcha is not valid";
                    return View();
                }
            }


            var data = await CoreFactory.Singleton.Accounts
                .TryCreateUserRecoveryContextAsync(
                    UserEmail: txt_Email,
                    IsOptional: true);


            if (data.ResultCode != AcctRecoveryResultCode.Success)
            {
                //*/
                ViewBag.Message = data.ResultCode.ToString();
                /*/
                ViewBag.Message = "Recovery email sent, please check your inbox";
                //*/

                //"security" measure
                //make it more difficult to discover valid email addresses
                //Only works if there is no error message presented to user (ie, pointless now)
                Random rnd = new Random();
                var fluff = rnd.Next(1900, 2400);
                await Task.Delay(fluff);
                return View();
            }


            var path = RecoveryManager.GetRecoveryURL(data.RecoveryContext);

            var recoveryMessage = new EmailMessage_ForgotPswd("UHub Account Recovery", "UHub", txt_Email)
            {
                RecoveryURL = path,
                RecoveryKey = data.RecoveryKey
            };


            var mailResult = await CoreFactory.Singleton.Mail.TrySendMessageAsync(recoveryMessage);

            if (mailResult == EmailResultCode.Success)
            {
                ViewBag.Message = "Recovery email sent, please check your inbox";
                return View();
            }
            else
            {
                ViewBag.Message = mailResult.ToString();
                return View();
            }
        }



        [System.Web.Mvc.HttpGet]
        public ActionResult Recover()
        {
            ViewBag.CanForward = false;
            return View();
        }


        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Recover(string txt_RecoveryKey, string txt_NewPswd, string txt_ConfirmPswd)
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var recoveryID = idObj?.ToString() ?? "";
            ViewBag.CanForward = false;


            if (recoveryID.IsEmpty() || recoveryID.Length > 200)
            {
                ViewBag.Message = "Invalid Recovery Key";
                return View();
            }

            if (txt_RecoveryKey.Length > 200)
            {
                ViewBag.Message = "Invalid Recovery Key";
                return View();
            }

            if (txt_NewPswd != txt_ConfirmPswd)
            {
                ViewBag.Message = "Passwords must match";
                return View();
            }


            var context = System.Web.HttpContext.Current;
            if (CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                var isCaptchaValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(context);
                if (!isCaptchaValid)
                {
                    ViewBag.Message = "Captcha is not valid";
                    return View();
                }
            }



            var result = await CoreFactory.Singleton.Accounts.TryRecoverPasswordAsync(
                recoveryID,
                txt_RecoveryKey,
                txt_NewPswd,
                true,
                context);


            ViewBag.CanForward = (result == 0);
            ViewBag.Message = result.ToString();


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult Find()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = int.TryParse(idStr, out var userId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            return View();
        }
    }
}