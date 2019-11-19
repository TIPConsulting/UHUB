using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.EmailInterop.Templates;

namespace UHub.CoreLib.EmailInterop.Providers.SMTP
{
    /// <summary>
    /// CMS SMTP (email) manager to send messages from the site
    /// </summary>
    public sealed partial class SmtpProvider
    {

        /// <summary>
        /// Attempt to send an email using the system configuration
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public override async Task<EmailResultCode> TrySendMessageAsync(EmailMessage Message)
        {
            if (!Message.Validate())
            {
                return EmailResultCode.ValidationError;
            }

            
            var from = _config.FromAddress;
            var to = Message.Recipient;
            var subj = Message.Subject;



            using (SmtpClient client = GetSmtpClient())
            {

                client.EnableSsl = true;


                using (MailMessage msgOut = new MailMessage(from, new MailAddress(to)))
                {


                    msgOut.Subject = subj;
                    msgOut.IsBodyHtml = true;
                    msgOut.Body = Message.GetMessage();

                    try
                    {
                        await client.SendMailAsync(msgOut);

                        return EmailResultCode.Success;
                    }
                    catch
                    {
                        return EmailResultCode.ValidationError;
                    }

                }
            }

        }

    }
}
