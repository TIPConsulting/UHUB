using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.EmailInterop;
using UHub.CoreLib.EmailInterop.Providers;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_Mail
    {
        /// <summary>
        /// Email provider for sending messages
        /// </summary>
        public EmailProvider MailProvider { get; set; } = null;
        /// <summary>
        /// The email address that Contact Us emails will be delivered to
        /// <para></para>
        /// Default: null
        /// </summary>
        public string ContactFormRecipientAddress { get; set; } = null;
    }
}
