using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Logging.Util
{
    internal static class LoggingHelpers
    {
        public static UsageLogData GetUserClientData(long? UserID)
        {
            var Request = HttpContext.Current.Request;

            var logData = new UsageLogData();



            if (Request != null)
            {

                logData.UserAgent = UserID?.ToString() ?? null;
                logData.ClientSessionID = HttpContext.Current?.Session?.SessionID ?? null;
                logData.HostIP = Request.UserHostAddress.TrimToLength(50);
                logData.ClientIP = Request.GetIPAddress().TrimToLength(50);
                logData.Accept = Request.Headers["Accept"].TrimToLength(200);
                logData.Encodings = Request.Headers["Accept-Encoding"].TrimToLength(50);
                logData.Languages = Request.Headers["Accept-Language"].TrimToLength(50);
                logData.UrlReferrer = Request.UrlReferrer?.ToString().TrimToLength(500);
                logData.UserAgent = Request.UserAgent.TrimToLength(500);
                logData.BrowserName = Request.Browser.Browser.TrimToLength(100);
                logData.BrowserVersion = Request.Browser.Version.TrimToLength(50);
                logData.Platform = Request.Browser.Platform.TrimToLength(100);
                logData.IsCrawler = Request.Browser.Crawler.ToString();
                logData.IsMobile = Request.Browser.IsMobileDevice.ToString();
                logData.IsBeta = Request.Browser.Beta.ToString();
                logData.SupportsJSVersion = Request.Browser.EcmaScriptVersion.ToString().TrimToLength(50);
                logData.SupportsVBS = Request.Browser.VBScript.ToString();
                logData.SupportsJava = Request.Browser.JavaApplets.ToString();
                logData.SupportsTables = Request.Browser.Tables.ToString();
                logData.SupportsFrames = Request.Browser.Frames.ToString();
                logData.SupportsCookies = Request.Browser.Cookies.ToString();
                logData.SupportsActiveX = Request.Browser.ActiveXControls.ToString();
                logData.DomVersion = Request.Browser.W3CDomVersion.ToString().TrimToLength(50);
                logData.InputType = Request.Browser.InputType.TrimToLength(50);
                logData.ColorDepth = Request.Browser.ScreenBitDepth.ToString();
                logData.ScreenHeight = Request.Browser.ScreenPixelsHeight.ToString();
                logData.ScreenWidth = Request.Browser.ScreenPixelsWidth.ToString();
            }
            else
            {
                logData.UserAgent = null;
                logData.ClientSessionID = null;
                logData.HostIP = null;
                logData.ClientIP = null;
                logData.Accept = null;
                logData.Encodings = null;
                logData.Languages = null;
                logData.UrlReferrer = null;
                logData.UserAgent = null;
                logData.BrowserName = null;
                logData.BrowserVersion = null;
                logData.Platform = null;
                logData.IsCrawler = null;
                logData.IsMobile = null;
                logData.IsBeta = null;
                logData.SupportsJSVersion = null;
                logData.SupportsVBS = null;
                logData.SupportsJava = null;
                logData.SupportsTables = null;
                logData.SupportsFrames = null;
                logData.SupportsCookies = null;
                logData.SupportsActiveX = null;
                logData.DomVersion = null;
                logData.InputType = null;
                logData.ColorDepth = null;
                logData.ScreenHeight = null;
                logData.ScreenWidth = null;
            }



            return logData;

        }

    }
}
