using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;


namespace UHub.CoreLib.EmailInterop.Templates
{

    /// <summary>
    /// Confirm membership registration after users have completed their forms
    /// </summary>
    public sealed class SmtpMessage_ConfirmMembership : EmailMessage
    {

        private const string Template =
            @"<!DOCTYPE html>
            <html>
                <head>
                    <title>Membership Confirmation</title>
                    <meta charset=""utf-8"" />
                    <style>
                        ul{margin: 0 0 10px 0}
                    </style>
                </head>
                <body>
                    <h3>Membership Confirmation</h3>
                    
                    <p>
                        This is being sent to you to confirm your {LambdaVar:siteName} Membership registration for the following term: <strong>{LambdaVar:termName}</strong>.
                    </p>

                    <p>
                        <strong>To pay by credit card:</strong> use <a href=""{LambdaVar:ccPaymentURL}"">This Link</a>
                    </p>

                    <p>
                        <strong>To pay by check:</strong> Print a copy of this email and send it with a check or money order to<br/>
                        <span style=""margin-left:20px"">
                            {LambdaVar:companyAddress}
                        </span>
                    </p>

                    <p>
                        The amount due for your membership type is <strong>${LambdaVar:memPrice}</strong>
                    </p>

                    <p>
                        <strong>Please note that your membership will not be complete until payment has been received.</strong>
                    </p>


                    <hr/>
                    
                    <h4 style=""margin-bottom:10px"">Member Details</h4>

                    <table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""padding:0 0 0 30px"">
                        <tr>
                            <td><strong>Member Term:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:termName}</td>
                        </tr>
                        <tr>
                            <td><strong>Member Type:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:memType}</td>
                        </tr>
                        <tr>
                            <td><strong>Member ID:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:memRefUID}</td>
                        </tr>
                        <tr>
                            <td><strong>Name:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:memName}</td>
                        </tr>
                        <tr>
                            <td><strong>Email:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:memEmail}</td>
                        </tr>
                        <tr>
                            <td><strong>Phone:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:memPhone}</td>
                        </tr>
                        {LambdaVar:memStdntInfo}
                    </table>

                </body>
            </html>";

        /// <summary>
        /// Client website name
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// Company mailing address for users to send payment checks
        /// </summary>
        public string CompanyMailingAddress { get; set; } = null;
        /// <summary>
        /// URL for users to pay their membership invoice
        /// </summary>
        public string CreditCardPaymentURL { get; set; } = null;
        /// <summary>
        /// Membership registration price
        /// </summary>
        public decimal MemberPrice { get; set; }
        /// <summary>
        /// Term name
        /// </summary>
        public string TermName { get; set; } = null;
        /// <summary>
        /// Member registration type
        /// </summary>
        public string MemberType { get; set; } = null;
        /// <summary>
        /// Member registration reference UID
        /// </summary>
        public string MemberRegistryRefUID { get; set; } = null;
        /// <summary>
        /// Member name
        /// </summary>
        public string MemberName { get; set; } = null;
        /// <summary>
        /// Member email
        /// </summary>
        public string MemberEmail { get; set; } = null;
        /// <summary>
        /// Member phone number
        /// </summary>
        public string MemberPhone { get; set; } = null;
        /// <summary>
        /// Member university (for students)
        /// </summary>
        public string StudentSchool { get; set; } = null;
        /// <summary>
        /// Member university major (for students)
        /// </summary>
        public string StudentMajor { get; set; } = null;

        /// <summary>
        /// Create email for member term registration confirmation and payment information
        /// </summary>
        /// <param name="Subject">Email subject</param>
        /// <param name="SiteName">Client wesbite name</param>
        /// <param name="ConfirmationRecipient">Member email adress</param>
        public SmtpMessage_ConfirmMembership(string Subject, string SiteName, string ConfirmationRecipient) : base(Subject, ConfirmationRecipient)
        {
            this.SiteName = SiteName;
        }

        protected override bool ValidateInner()
        {
            if (SiteName.IsEmpty())
            {
                throw new ArgumentException("SiteName cannot be null or empty");
            }
            if (CompanyMailingAddress.IsEmpty())
            {
                throw new ArgumentException("CompanyAddress cannot be null or empty");
            }
            if (CreditCardPaymentURL.IsEmpty())
            {
                throw new ArgumentException("CreditCardPaymentURL cannot be null or empty");
            }
            if (TermName.IsEmpty())
            {
                throw new ArgumentException("TermName cannot be null or empty");
            }
            if (MemberType.IsEmpty())
            {
                throw new ArgumentException("MemberType cannot be null or empty");
            }
            if (MemberRegistryRefUID.IsEmpty())
            {
                throw new ArgumentException("MemberRegistryRefUID cannot be null or empty");
            }
            if (MemberName.IsEmpty())
            {
                throw new ArgumentException("MemberName cannot be null or empty");
            }
            if (MemberEmail.IsEmpty())
            {
                throw new ArgumentException("MemberEmail cannot be null or empty");
            }
            if (MemberPhone.IsEmpty())
            {
                throw new ArgumentException("MemberPhone cannot be null or empty");
            }
            if ((StudentSchool.IsNotEmpty() && StudentMajor.IsEmpty()) || (StudentMajor.IsNotEmpty() && StudentSchool.IsEmpty()))
            {
                throw new ArgumentException("All student information must be supplied");
            }

            return true;
        }

        protected override string RenderMessage()
        {

            var output = Template
                .Replace("{LambdaVar:siteName}", this.SiteName.HtmlEncode())
                .Replace("{LambdaVar:companyAddress}", this.CompanyMailingAddress.HtmlEncode().Replace(Environment.NewLine, "<br/>"))
                .Replace("{LambdaVar:mailingAddress}", this.SiteName.HtmlEncode())
                .Replace("{LambdaVar:ccPaymentURL}", this.CreditCardPaymentURL)
                .Replace("{LambdaVar:memPrice}", this.MemberPrice.ToString("0.00"))
                .Replace("{LambdaVar:termName}", this.TermName.HtmlEncode())
                .Replace("{LambdaVar:memType}", this.MemberType.HtmlEncode())
                .Replace("{LambdaVar:memName}", this.MemberName.HtmlEncode())
                .Replace("{LambdaVar:memEmail}", this.MemberEmail.HtmlEncode())
                .Replace("{LambdaVar:memPhone}", this.MemberPhone.HtmlEncode())
                .Replace("{LambdaVar:memRefUID}", this.MemberRegistryRefUID);

            if (StudentSchool.IsNotEmpty())
            {
                var studentUniv = $@"<tr><td><strong>University:</strong></td></tr><tr><td style=""padding:0 0 5px 40px"">{this.StudentSchool.HtmlEncode()}</td></tr>";
                var studentMajor = $@"<tr><td><strong>Major:</strong></td></tr><tr><td style=""padding:0 0 5px 40px"">{this.StudentMajor.HtmlEncode()}</td></tr>";

                output = output.Replace("{LambdaVar:memStdntInfo}", studentUniv + studentMajor);
            }
            else
            {
                output = output.Replace("{LambdaVar:memStdntInfo}", "");
            }

            return output;
        }

    }
}
