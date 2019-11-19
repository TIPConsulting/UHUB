using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.EmailInterop.Templates
{

    /// <summary>
    /// Confirm membership event registration after users have completed the form
    /// </summary>
    public sealed class EmailMessage_ConfirmEvent : EmailMessage
    {

        private const string Template =
            @"<!DOCTYPE html>
            <html>
                <head>
                    <title>Event Confirmation</title>
                    <meta charset=""utf-8"" />
                </head>
                <body>
                    <h3>Event Confirmation</h3>
                    
                    <p>
                        This is being sent to you to confirm your {LambdaVar:siteName} event registration for the following event: <strong>{LambdaVar:eventName}</strong>.
                    </p>

                    <hr/>

                    <h4 style=""margin-bottom:10px"">Event Details</h4>
                    

                    <table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""padding:0 0 0 30px"">
                        {LambdaVar:eventDescription}
                        <tr>
                            <td><strong>Event Date:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:eventDate}</td>
                        </tr>
                        <tr>
                            <td><strong>Event Time:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:eventTime}</td>
                        </tr>
                        <tr>
                            <td><strong>Event Location:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:eventLocation}</td>
                        </tr>
                    </table>
                    

                    <h4 style=""margin-bottom:10px"">Registration Details</h4>
                    
                    <table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""padding:0 0 0 30px"">

                        {LambdaVar:attendingLuncheon}
                        {LambdaVar:attendingBreakoutAM}
                        {LambdaVar:attendingBreakoutPM}
                        <tr>
                            <td><strong>Guest Count:</strong></td>
                        </tr>
                        <tr>
                            <td style="" padding:0 0 5px 50px"">{LambdaVar:guestCount}</td>
                        </tr>
                        <tr>
                            <td><strong>Dietary Requests:</strong></td>
                        </tr>
                        <tr>
                            <td style=""padding:0 0 5px 50px"">{LambdaVar:dietRequest}</td>
                        </tr>
                    </table>

                </body>
            </html>";

        /// <summary>
        /// Client website name
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// Member event name
        /// </summary>
        public string EventName { get; set; } = null;
        /// <summary>
        /// Member event description
        /// </summary>
        public string EventDescription { get; set; } = null;
        /// <summary>
        /// Member event date
        /// </summary>
        public string EventDate { get; set; }
        /// <summary>
        /// Member event start time
        /// </summary>
        public string EventTime { get; set; }
        /// <summary>
        /// Member event location
        /// </summary>
        public string EventLocation { get; set; }
        /// <summary>
        /// Does this event have a luncheon
        /// </summary>
        public bool HasLuncheon { get; set; }
        /// <summary>
        /// Does this event have a morning breakout session
        /// </summary>
        public bool HasBreakoutAM { get; set; }
        /// <summary>
        /// Does this event have an afternoon breakout session
        /// </summary>
        public bool HasBreakoutPM { get; set; }
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Event registration guest count
        /// </summary>
        public byte GuestCount { get; set; } = 0;
        /// <summary>
        /// Event registration dietary requests
        /// </summary>
        public string DietaryRequests { get; set; }
        /// <summary>
        /// Event registration luncheon status
        /// </summary>
        public bool IsAttendingLuncheon { get; set; }
        /// <summary>
        /// Event registrationm morning breakout status
        /// </summary>
        public bool IsAttendingBreakoutAM { get; set; }
        /// <summary>
        /// Event registration afternoon breakout status
        /// </summary>
        public bool IsAttendingBreakoutPM { get; set; }

        /// <summary>
        /// Create email to send event registration confirmation information
        /// </summary>
        /// <param name="Subject">Email subject</param>
        /// <param name="SiteName">Client website name</param>
        /// <param name="ConfirmationRecipient">Recipient email address</param>
        public EmailMessage_ConfirmEvent(string Subject, string SiteName, string ConfirmationRecipient) : base(Subject, ConfirmationRecipient)
        {
            this.SiteName = SiteName;
        }

        protected override bool ValidateInner()
        {
            if (SiteName.IsEmpty())
            {
                throw new ArgumentException("SiteName cannot be null or empty");
            }

            if (EventName.IsEmpty())
            {
                throw new ArgumentException("EventName cannot be null or empty");
            }
            if (EventDate.IsEmpty())
            {
                throw new ArgumentException("EventDate cannot be null or empty");
            }
            if (EventLocation.IsEmpty())
            {
                throw new ArgumentException("EventLocation cannot be null or empty");
            }


            return true;
        }

        protected override string RenderMessage()
        {

            var output = Template
                .Replace("{LambdaVar:siteName}", this.SiteName.HtmlEncode())
                .Replace("{LambdaVar:eventName}", this.EventName.HtmlEncode())
                .Replace("{LambdaVar:eventDate}", this.EventDate.HtmlEncode())
                .Replace("{LambdaVar:eventTime}", this.EventTime.HtmlEncode())
                .Replace("{LambdaVar:eventLocation}", this.EventLocation.HtmlEncode().Replace(Environment.NewLine, "<br/>"))
                //
                .Replace("{LambdaVar:guestCount}", this.GuestCount.ToString())

                ;

            //EVENT DESCRIPTION
            if (EventDescription.IsNotEmpty())
            {
                var description = @"<tr><td style=""padding:0 0 15px 30px"">" + EventDescription.HtmlEncode().Replace(Environment.NewLine, "<br/>") + "</td></tr>";

                output = output.Replace("{LambdaVar:eventDescription}", description);
            }
            else
            {
                output = output.Replace("{LambdaVar:eventDescription}", "");
            }

            //LUNCHEON
            if(HasLuncheon)
            {
                var temp1 = $@"<tr><td><strong>Attending Luncheon:</strong></td></tr><tr><td style="" padding:0 0 5px 50px"">{this.IsAttendingLuncheon.ToString()}</td></tr>";

                output = output.Replace("{LambdaVar:attendingLuncheon}", temp1);
            }
            else
            {
                output = output.Replace("{LambdaVar:attendingLuncheon}", "");
            }

            //BREAKOUT (AM)
            if (HasBreakoutPM)
            {
                var temp1 = $@"<tr><td><strong>Attending Morning Breakout Session:</strong></td></tr><tr><td style="" padding:0 0 5px 50px"">{this.IsAttendingBreakoutAM.ToString()}</td></tr>";

                output = output.Replace("{LambdaVar:attendingBreakoutAM}", temp1);
            }
            else
            {
                output = output.Replace("{LambdaVar:attendingBreakoutAM}", "");
            }

            //BREAKOUT (PM)
            if (HasBreakoutPM)
            {
                var temp1 = $@"<tr><td><strong>Attending Afternoon Breakout Session:</strong></td></tr><tr><td style="" padding:0 0 5px 50px"">{this.IsAttendingBreakoutPM.ToString()}</td></tr>";

                output = output.Replace("{LambdaVar:attendingBreakoutPM}", temp1);
            }
            else
            {
                output = output.Replace("{LambdaVar:attendingBreakoutPM}", "");
            }

            //DIETARY REQUESTS
            if (DietaryRequests.IsNotEmpty())
            {
                output = output.Replace("{LambdaVar:dietRequest}", this.DietaryRequests.HtmlEncode().Replace(Environment.NewLine, "<br/>"));
            }
            else
            {
                output = output.Replace("{LambdaVar:dietRequest}", "None");
            }

            return output;
        }

    }
}
