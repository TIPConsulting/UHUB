using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.EmailInterop.Templates
{
    /// <summary>
    /// SMTP (email) message to base
    /// </summary>
    public abstract class EmailMessage
    {
        /// <summary>
        /// Message subject
        /// </summary>
        public string Subject { get; }
        /// <summary>
        /// Message recipient
        /// </summary>
        public string Recipient { get; }

        public EmailMessage(string Subject, string Recipient)
        {
            this.Subject = Subject;
            this.Recipient = Recipient;
        }

        /// <summary>
        /// Render full message from email template and supplied arguments
        /// </summary>
        /// <returns></returns>
        internal string GetMessage()
        {
            if(this.Validate() && this.ValidateInner())
            {
                return this.RenderMessage();
            }
            else
            {
                throw new UHub.CoreLib.ErrorHandling.Exceptions.ConfigurationException("Message cannot be compiled due to invalid arguments");
            }
        }

        /// <summary>
        /// Validate base email arguments
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {

            if (!Validators.IsValidEmail(Recipient))
            {
                throw new ArgumentException("Recipient email address is invalid");
            }

            return true;
        }

        /// <summary>
        /// Validate child email arguments
        /// </summary>
        /// <returns></returns>
        protected abstract bool ValidateInner();

        /// <summary>
        /// Render true message
        /// </summary>
        /// <returns></returns>
        protected abstract string RenderMessage();
    }
}
