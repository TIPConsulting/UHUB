using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.EmailInterop.Providers.SMTP
{
    /// <summary>
    /// SMTP (email) server configuration 
    /// </summary>
    public sealed class SmtpConfig
    {
        /// <summary>
        /// Mail server from address
        /// </summary>
        public MailAddress FromAddress { get; }

        /// <summary>
        /// Use mail server default credentials
        /// </summary>
        public bool UseDefaultCredentials { get; }

        /// <summary>
        /// Enable SSL for the SMTP connection
        /// </summary>
        public bool EnableSSL { get; }

        /// <summary>
        /// SMTP host
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// SMTP host server port
        /// </summary>
        public int Port { get; } = -1;

        /// <summary>
        /// SMTP account username
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// SMTP account pasword
        /// </summary>
        public string Password { get; }


        public SmtpConfig(MailAddress FromAddress, bool UseDefaultCredentials, bool EnableSSL, string Host, int Port)
        {
            this.FromAddress = FromAddress;
            this.UseDefaultCredentials = UseDefaultCredentials;
            this.EnableSSL = EnableSSL;
            this.Host = Host;
            this.Port = Port;
            this.UserName = null;
            this.Password = null;
        }

        public SmtpConfig(MailAddress FromAddress, bool UseDefaultCredentials, bool EnableSSL, string Host, int Port, string UserName, string Password)
        {
            this.FromAddress = FromAddress;
            this.UseDefaultCredentials = UseDefaultCredentials;
            this.EnableSSL = EnableSSL;
            this.Host = Host;
            this.Port = Port;
            this.UserName = UserName;
            this.Password = Password;
        }

        public SmtpConfig(SmtpConfig config)
        {
            this.FromAddress = new MailAddress(config.FromAddress.Address, config.FromAddress.DisplayName);
            this.UseDefaultCredentials = config.UseDefaultCredentials;
            this.EnableSSL = config.EnableSSL;
            this.Host = config.Host;
            this.Port = config.Port;
            this.UserName = config.UserName;
            this.Password = config.Password;
        }


        /// <summary>
        /// Validate the configuration parameters
        /// </summary>
        /// <returns></returns>
        internal bool Validate()
        {
            if (FromAddress == null)
            {
                throw new ArgumentException("FromAddress cannot be null or empty");
            }
            if (FromAddress.Address.IsEmpty())
            {
                throw new ArgumentException("FromAddress cannot be null or empty");
            }

            if (Host.IsEmpty())
            {
                throw new ArgumentException("Host cannot be null or empty");
            }
            if (Host.StartsWith("http://") || Host.StartsWith("https://"))
            {
                throw new ArgumentException("Host name should not include protocol");
            }


            if (Port == -1)
            {
                throw new ArgumentException("Port cannot be null or empty");
            }

            if (!UseDefaultCredentials)
            {
                if (UserName.IsEmpty())
                    throw new ArgumentException("UserName cannot be null or empty");

                if (Password.IsEmpty())
                    throw new ArgumentException("Password cannot be null or empty");
            }

            return true;
        }

    }
}
