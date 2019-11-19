using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    internal sealed class UsageLogData
    {
        public string UserID { get; set; } = null;
        public string ClientSessionID { get; set; } = null;
        public string HostIP { get; set; } = null;
        public string ClientIP { get; set; } = null;
        public string Accept { get; set; } = null;
        public string Encodings { get; set; } = null;
        public string Languages { get; set; } = null;
        public string UrlReferrer { get; set; } = null;
        public string UserAgent { get; set; } = null;
        public string BrowserName { get; set; } = null;
        public string BrowserVersion { get; set; } = null;
        public string Platform { get; set; } = null;
        public string IsCrawler { get; set; } = null;
        public string IsMobile { get; set; } = null;
        public string IsBeta { get; set; } = null;
        public string SupportsJSVersion { get; set; } = null;
        public string SupportsVBS { get; set; } = null;
        public string SupportsJava { get; set; } = null;
        public string SupportsTables { get; set; } = null;
        public string SupportsFrames { get; set; } = null;
        public string SupportsCookies { get; set; } = null;
        public string SupportsActiveX { get; set; } = null;
        public string DomVersion { get; set; } = null;
        public string InputType { get; set; } = null;
        public string ColorDepth { get; set; } = null;
        public string ScreenHeight { get; set; } = null;
        public string ScreenWidth { get; set; } = null;
    }
}
