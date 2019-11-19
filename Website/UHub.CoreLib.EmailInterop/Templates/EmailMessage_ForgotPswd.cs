using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.EmailInterop.Templates
{
    /// <summary>
    /// Auto forgot password message with recovery link
    /// </summary>
    public sealed class EmailMessage_ForgotPswd : EmailMessage
    {

        private const string Template =
            @"<!DOCTYPE html>
            <html>
                <head>
                    <title>Forgot Password</title>
                    <meta charset=""utf-8"" />
                </head>
                <body>
                    <h3>Password Reset</h3>
                    <p>
                        You have received this message because a password reset was requested for your {LambdaVar:siteName} account.  If you did not request this reset, please ignore this message.
                        If you did request this message, please continue to the recovery link.  You will need the provided recovery key in order to reset your password
                    <p>
                    <strong><a href=""{LambdaVar:recoveryLink}"" rel=""notrack"">Recovery Link</a></strong>
                    <p></p>
                    <strong>Recovery Key:</strong></br>
                    <p>
                        {LambdaVar:recoveryKey}
                    </p>
                </body>
            </html>";


        /// <summary>
        /// Client website name
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// Password recovery URL including user specific ID
        /// </summary>
        public string RecoveryURL { get; set; }
        /// <summary>
        /// Password recovery key specific to user
        /// </summary>
        public string RecoveryKey { get; set; }


        /// <summary>
        /// Create email to send user password recovery information
        /// </summary>
        /// <param name="Subject">Email subject</param>
        /// <param name="SiteName">Client website name</param>
        /// <param name="PasswordResetRecipient">Password recovery recipient email address</param>
        public EmailMessage_ForgotPswd(string Subject, string SiteName, string PasswordResetRecipient) : base(Subject, PasswordResetRecipient)
        {
            this.SiteName = SiteName;
        }

        protected override bool ValidateInner()
        {            
            if(SiteName.IsEmpty())
            {
                throw new ArgumentException("SiteName cannot be null or empty");
            }
            if(RecoveryURL.IsEmpty())
            {
                throw new ArgumentException("RecoveryLink cannot be null or empty");
            }
            if (RecoveryKey.IsEmpty())
            {
                throw new ArgumentException("RecoveryKey cannot be null or empty");
            }

            return true;
        }

        protected override string RenderMessage()
        {
            var output = Template
                .Replace("{LambdaVar:siteName}", this.SiteName.HtmlEncode())
                .Replace("{LambdaVar:recoveryLink}", this.RecoveryURL)
                .Replace("{LambdaVar:recoveryKey}", this.RecoveryKey);

            return output;
        }

    }
}
