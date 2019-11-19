using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.EmailInterop.Templates;

namespace UHub.CoreLib.EmailInterop.Providers.SMTP
{
    /// <summary>
    /// CMS SMTP (email) manager to send messages from the site
    /// </summary>
    public sealed partial class SmtpProvider : EmailProvider
    {
        private SmtpConfig _config = null;
        private bool hasPerformedValidation = false;
        private bool isValid = false;


        public SmtpProvider(SmtpConfig config)
        {
            _config = config;
        }


        /// <summary>
        /// Generate SmtpClient object from configuration
        /// </summary>
        /// <returns></returns>
        private SmtpClient GetSmtpClient()
        {
            var smtpClient = new SmtpClient(_config.Host, _config.Port);

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = _config.UseDefaultCredentials;
            smtpClient.EnableSsl = _config.EnableSSL;
            smtpClient.TargetName = $"STARTTLS/{_config.Host}";

            smtpClient.Credentials = new NetworkCredential(_config.UserName, _config.Password);

            return smtpClient;
        }



        public override void Validate()
        {
            //check for cached value
            if(hasPerformedValidation)
            {
                if(!isValid)
                {
                    string err = $"MailProvider is invalid.";
                    throw new ArgumentException(err);
                }
                return;
            }


            hasPerformedValidation = true;


            if (_config == null)
            {
                string err = $"MailProvider Config must be set.";
                throw new ArgumentException(err);
            }


            try
            {
                if (!_config.Validate())
                {
                    string err = $"MailProvider config is invalid.";
                    throw new ArgumentException(err);
                }
            }
            catch (Exception ex)
            {
                string err = $"MailProvider config: {ex.Message}";
                throw new ArgumentException(err);
            }


            isValid = true;
            hasPerformedValidation = true;

        }

    }
}
