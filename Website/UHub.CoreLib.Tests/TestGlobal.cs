using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Web.Http.Results;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using System.Web.Http.Routing;
using System.Web.Http.Filters;
using NSubstitute;
using UHub.ApiControllers.Security.Authentication;
using UHub.CoreLib.Management.Config;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.EmailInterop;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Util;
using UHub.CoreLib.Security.Authentication.APIControllers;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.EmailInterop.Providers.SMTP;

namespace UHub.CoreLib.Tests
{
    public class TestGlobal
    {

        public static void TestInit()
        {
            if (CoreFactory.IsInitialized())
            {
                return;
            }


            var configPath = @"\\thelairserv4\FileHosting\IISData\UHub\_configs\Test_Raw";
            string getVar(string name)
            {
                return File.ReadAllText(Path.Combine(configPath, name + ".txt"));
            }

            //DB
            var dbConn = getVar("DB_Conn");

            //MAIL
            var ContactFormRecipient = getVar("ContactFormRecipient");
            var mailFromAddr = getVar("MailFromAddress");
            var mailFromName = getVar("MailFromName");
            var mailHost = getVar("MailHost");
            var mailPort = int.Parse(getVar("MailPort"));
            var mailUsername = getVar("MailUsername");
            var mailPswd = getVar("MailPassword");


            //FILES
            var fileStoreDir = getVar("FileStoreDir");
            var imgStoreDir = getVar("ImageStoreDir");
            var tempCacheDir = getVar("TempCacheDir");
            var logStoreDir = getVar("LogStoreDir");


            //CAPTCHA
            var captchaPublicKey = getVar("RecaptchaPublicKey");
            var captchaPrivateKey = getVar("RecaptchaPrivateKey");


            var allowedFileTypes = new FileCategory[]
                    {
                        FileCategory.Image,
                        FileCategory.Document
                    };

            var mailConfig = new SmtpConfig(new MailAddress(mailFromAddr, mailFromName), false, true, mailHost, mailPort, mailUsername, mailPswd);

            var cmsConfig = new CmsConfiguration_Grouped()
            {
                Instance = new CmsConfig_Instance
                {
                    SiteFriendlyName = "UHUB",
                    CmsPublicBaseURL = "https://u-hub.life",
                    CmsStaticResourceURL = "https://static.u-hub.life",
                },
                DB = new CmsConfig_DB
                {
                    CmsDBConfig = new SqlConfig(dbConn),
                    EnableDBMultithreading = true
                },
                Storage = new CmsConfig_Storage
                {
                    FileStoreDirectory = fileStoreDir,
                    ImageStoreDirectory = imgStoreDir,
                    TempCacheDirectory = tempCacheDir,
                    CreateMissingStorageDirectories = true
                },
                Mail = new CmsConfig_Mail
                {
                    MailProvider = new SmtpProvider(mailConfig),
                    ContactFormRecipientAddress = ContactFormRecipient
                },
                Security = new CmsConfig_Security
                {
                    PswdHashType = CryptoHashType.HMACSHA512,
                    ForceHTTPS = true,
                    ForceSecureCookies = true,
                    CookieDomain = ".u-hub.life",
                    ForceSecureResponseHeaders = true,
                    AuthTokenTimeout = new TimeSpan(0, 6, 0, 0),
                    EnableAuthTokenSlidingExpiration = true,
                    LoginURL = "https://u-hub.life/Account/Login",
                    DefaultAuthFwdURL = "https://u-hub.life/Account",
                    AcctConfirmURL = "https://u-hub.life/Account/Confirm",
                    AcctPswdRecoveryURL = "https://u-hub.life/Account/ResetPassword",
                    AcctPswdUpdateURL = "https://u-hub.life/Account/UpdatePassword",
                    AcctPswdRecoveryLifespan = new TimeSpan(0, 0, 30, 0),
                    EnableRecaptcha = false,                //CAPTCHA
                    RecaptchaPublicKey = captchaPublicKey,
                    RecaptchaPrivateKey = captchaPrivateKey,
                    AutoConfirmNewAccounts = true,
                    AutoApproveNewAccounts = true,
                    EnableTokenVersioning = false,           //VERSION
                    CookieSameSiteMode = CookieSameSiteModes.Lax,
                    EnablePswdRecovery = true,
                    EnablePersistentAuthTokens = true,
                    HtmlSanitizerMode = HtmlSanitizerMode.OnWrite | HtmlSanitizerMode.OnRead
                },
                Logging = new CmsConfig_Logging
                {
                    //EnableUserSessionLogging = true,
                    //EnableUserActivityLogging = true,
                    EventLogMode = EventLoggingMode.File,
                    LoggingSource = LocalSysLoggingSource.UHUB_CMS,
                    LogFileDirectory = logStoreDir,
                },
                Errors = new CmsConfig_Errors
                {
                    EnableCustomErrorCodes = true
                },
                Caching = new CmsConfig_Caching
                {
                    EnableStartupCachePopulation = true,
                    EnableNavBarCaching = true,
                    EnableDBPageCaching = false,
                    MaxDBCacheAge = new TimeSpan(0, 6, 0, 0),
                    EnableIISPageCaching = false,
                    MaxDynamicCacheAge = new TimeSpan(0, 1, 0, 0),
                    MaxStaticCacheAge = new TimeSpan(1, 0, 0, 0),
                },
                API = new CmsConfig_API
                {
                    RegisterAPIRoutes = true,
                    EnableDetailedAPIErrors = true,
                    EnableAPIAuthService = true,
                    EnableAPIFileUploads = true,
                    MaxFileUploadSize = new FileSize(FileSizeUnit.Gibibyte, 1),
                    AllowedFileUploadTypes = allowedFileTypes
                },
            };



            CoreFactory.Initialize(cmsConfig);

        }


        public static T GetStdRequest<T>(T controllerArg) where T : ApiController
        {
            HttpContext.Current = FakeHttpContext();

            var controller = controllerArg;
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://u-hub.life");

            controllerContext.RequestContext = new HttpRequestContext();
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            controller.ActionContext = new HttpActionContext(controllerContext, ContextUtil.CreateActionDescriptor());


            return controller;
        }


        public static async Task<T> GetAuthRequest<T>(T controllerArg, bool useCookie = false, string email = null, string password = null) where T : ApiController
        {
            var authController = GetStdRequest(new AuthenticationController());

            email = email ?? "aual1780@colorado.edu";
            password = password ?? "testtest";


            User_CredentialDTO cred = new User_CredentialDTO()
            {
                Email = email,
                Password = password
            };

            var response = await authController.GetToken(cred);
            Assert.IsNotNull(response);

            var result = response as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);

            var token = result.Content;


            var controller = GetStdRequest(controllerArg);
            if (useCookie)
            {
                var cookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;

                //add cookie
                HttpCookie c = new HttpCookie(cookieName);
                c.Value = token;
                HttpContext.Current.Request.Cookies.Add(c);

                //add cookie again
                token = token.UrlEncode();
                controller.Request.Headers.Add("Cookie", $"{cookieName}=\"{token}\"");

                controller.Request.Properties.Add("Cookie1", "Version=1");
            }
            else
            {
                var tknName = UHub.CoreLib.Constants.AUTH_HEADER_TOKEN;
                controller.Request.Headers.Add(tknName, token);
            }

            return controller;
        }



        private static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "https://u-hub.life", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("asjkas", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }



    }
}
