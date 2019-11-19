using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.RegExp;

namespace UHub.CoreLib.EmailInterop.Templates
{

    /// <summary>
    /// User contact form with user info and message that is sent to site admin
    /// </summary>

    public sealed class EmailMessage_ContactForm : EmailMessage
    {
        private const int MinMsgLength = 10;
        private const int MaxMsgLength = 1000;

        private const string Template =
            @"<!DOCTYPE html>
            <html>
                <head>
                    <title>Contact Form Message</title>
                    <meta charset=""utf-8"" />
                </head>
                <body>
                    <h3>User Info</h3>
                    <ul>
                        <li>Name: <strong>{LambdaVar:name}</strong></li>
                        <li>Email: <strong>{LambdaVar:email}</strong></li>
                        <li>Phone Number: <strong>{LambdaVar:phoneNum}</strong></li>
                    </ul>
                    <p></p>
                    <h3>Message</h3>
                    <div>
                        {LambdaVar:message}
                    </div>
                </body>
            </html>";

        /// <summary>
        /// Contact form user name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Contact form user email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Contact form user phone number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Contact form message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Create a contact form message to send to the system admins or help team
        /// </summary>
        /// <param name="Subject">Email subject</param>
        /// <param name="ContactFormRecipient">Contact form recipient email address</param>
        public EmailMessage_ContactForm(string Subject, string ContactFormRecipient) : base(Subject, ContactFormRecipient)
        {

        }

        protected override bool ValidateInner()
        {

            if (!this.Name.RgxIsMatch($@"^{RgxPtrns.EntUser.NAME}$"))
            {
                throw new ArgumentException("Name is invalid");
            }
            if (!Validators.IsValidEmail(this.Email))
            {
                throw new ArgumentException("Email is invalid");
            }
            if (!this.PhoneNumber.RgxIsMatch($@"^{RgxPtrns.EntUser.PHONE}$"))
            {
                throw new ArgumentException("Phone number is invalid");
            }
            if (this.Message.Length < MinMsgLength || this.Message.Length > MaxMsgLength)
            {
                throw new ArgumentException("Message length is invalid");
            }

            return true;
        }

        protected override string RenderMessage()
        {
            var output = Template
                .Replace("{LambdaVar:name}", this.Name.HtmlEncode())
                .Replace("{LambdaVar:email}", this.Email)
                .Replace("{LambdaVar:phoneNum}", this.PhoneNumber)
                .Replace("{LambdaVar:message}", this.Message.HtmlEncode());

            return output;
        }

    }
}
