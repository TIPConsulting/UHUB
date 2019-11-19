using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.EmailInterop.Templates
{

    /// <summary>
    /// Automatic account confirmation with confirmation link
    /// </summary>

    public sealed class EmailMessage_ConfirmAcct : EmailMessage
    {
        private const int MinMsgLength = 10;
        private const int MaxMsgLength = 1000;

        private const string Template =
            @"<!DOCTYPE html>
            <html>
                <head>
                    <title>Confirm Account</title>
                    <meta charset=""utf-8"" />
                </head>
                <body>
                    <h3>Account Confirmation</h3>
                    <p>
                        You have received this message because you recently created an account with {LambdaVar:siteName}.  Please <a href=""{LambdaVar:confirmURL}"" rel=""notrack"">Click Here</a> to confirm your account in order to log in.
                    </p>
                    <p>
                        If you did not create an account, this email may be safely ignored.
                    </p>
                </body>
            </html>";

        /// <summary>
        /// Client website name
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// Full acount confirmation URL including user ID
        /// </summary>
        public string ConfirmationURL { get; set; }

        /// <summary>
        /// Create email to send user account confirmation information
        /// </summary>
        /// <param name="Subject">Email subject</param>
        /// <param name="SiteName">Client website name</param>
        /// <param name="ConfirmationRecipient">User email address</param>
        public EmailMessage_ConfirmAcct(string Subject, string SiteName, string ConfirmationRecipient) : base(Subject, ConfirmationRecipient)
        {
            this.SiteName = SiteName;
        }

        protected override bool ValidateInner()
        {
            if(SiteName.IsEmpty())
            {
                throw new ArgumentException("SiteName cannot be null or empty");
            }
            if (ConfirmationURL.IsEmpty())
            {
                throw new ArgumentException("ConfirmationURL cannot be null or empty");
            }

            return true;
        }

        protected override string RenderMessage()
        {
            var output = Template
                .Replace("{LambdaVar:siteName}", this.SiteName.HtmlEncode())
                .Replace("{LambdaVar:confirmURL}", this.ConfirmationURL);

            return output;
        }

    }
}
