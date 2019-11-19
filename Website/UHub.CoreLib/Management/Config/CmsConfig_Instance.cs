using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_Instance
    {
        /// <summary>
        /// Website friendly name that can be used in the UI and emails
        /// <para></para>
        /// Default: null
        /// </summary>
        public string SiteFriendlyName { get; set; } = null;
        /// <summary>
        /// Public site root address including protocol (ex: https://test.test)
        /// <para></para>
        /// Default: null
        /// </summary>
        public string CmsPublicBaseURL { get; set; } = null;
        /// <summary>
        /// Public site static resource address including protocol (ex: https://test.test)
        /// <para></para>
        /// Default: null
        /// </summary>
        public string CmsStaticResourceURL { get; set; } = null;

        /// <summary>
        /// Name of the cookie that stores the user Session ID
        /// <para></para>
        /// Default: TIPCCMS_Session
        /// </summary>
        public string SessionIDCookieName { get; set; } = "UHUB_Session";

    }
}
