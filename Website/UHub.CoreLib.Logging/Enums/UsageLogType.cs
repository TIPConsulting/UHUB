using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    /// <summary>
    /// CMS DB activity log event types
    /// </summary>
    public enum UsageLogType
    {
        /// <summary>
        /// User log in event
        /// </summary>
        UserLogin = 1,
        /// <summary>
        /// User log out event
        /// </summary>
        UserLogOut = 2,
        /// <summary>
        /// User read entity event
        /// </summary>
        EntRead = 3,
        /// <summary>
        /// User download entity event
        /// </summary>
        EntDownload = 4,
        /// <summary>
        /// User access mamanaged page event
        /// </summary>
        PageRead = 5,
        /// <summary>
        /// Error event
        /// </summary>
        HttpError = 6,
        /// <summary>
        /// User views entity as resource
        /// </summary>
        EntServe = 7,
        /// <summary>
        /// User views entity through Google doc viewer
        /// </summary>
        EntPreview = 8,
        /// <summary>
        /// User enters bad password
        /// </summary>
        UserLoginFailure = 9,
        /// <summary>
        /// User resets password
        /// </summary>
        UserPasswordReset = 10,
        /// <summary>
        /// User updates email address
        /// </summary>
        UserEmailUpdate = 11

    }
}
