using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.ClientFriendly
{
    /// <summary>
    /// User friendly response strings. Used to normalize system success/error responses
    /// </summary>
    public static class ResponseStrings
    {
        /// <summary>
        /// Error strings for DB failures
        /// </summary>
        public static class DBError
        {
            /// <summary>
            /// Generic message used for unknown DB errors
            /// </summary>
            public const string GENERIC_UNKNOWN = "An unkown error occured while processing your request.  The database is currently not available.  If the problem persists please contact a site admin.";
            /// <summary>
            /// Generic message used for unknown DB errors while writing content
            /// </summary>
            public const string WRITE_UNKNOWN = "An unkown error occured while saving.  If the problem persists please contact a site admin.";
        }

        /// <summary>
        /// Error strings for SMTP failures
        /// </summary>
        public static class MailError
        {
            /// <summary>
            /// Generic message used for unknown SMTP errors
            /// </summary>
            public const string GENERIC_UNKNOWN = "An unkown error occured while processing your request. If the problem persists please contact a site admin.";
        }

        /// <summary>
        /// Error strings for CMS Config failures
        /// </summary>
        public static class ConfigError
        {
            /// <summary>
            /// Current page URL does not match the expected URL of the proxy page
            /// </summary>
            public const string PROXY_ADDRESS_MISMATCH = "The requested resource address does not match the configured proxy address";
        }

        /// <summary>
        /// Error strings for CMS User account errors
        /// </summary>
        public static class AccountError
        {
            /// <summary>
            /// Password not specified
            /// </summary>
            public const string PASSWORD_EMPTY = "Password cannot be empty";
            /// <summary>
            /// Confirmation password does not match original password
            /// </summary>
            public const string PASSWORD_MISMATCH = "Passwords do not match";
            /// <summary>
            /// Password does not adhere to specifications
            /// </summary>
            public const string PASSWORD_INVALID = "Password is not valid";
            /// <summary>
            /// New password is same as old
            /// </summary>
            public const string PASSWORD_NOT_CHANGED = "New password cannot be the same as the previous password";
            /// <summary>
            /// Email not specified
            /// </summary>
            public const string EMAIL_EMPTY = "Email cannot be empty";
            /// <summary>
            /// Email address does not adhere to specifications
            /// </summary>
            public const string EMAIL_INVALID = "Email address is not valid";
            /// <summary>
            /// Email already exists in CMS
            /// </summary>
            public const string EMAIL_DUPLICATE = "The specified email is already associated with an account";
            /// <summary>
            /// Unknown error occured while processing account creation
            /// </summary>
            public const string ACCOUNT_FAIL = "Unable to create user account. Please try again later";
        }

        /// <summary>
        /// Error strings for CMS User authentication errors
        /// </summary>
        public static class AuthenticationError
        {
            /// <summary>
            /// Email not specified
            /// </summary>
            public const string EMAIL_EMPTY = "Email cannot be empty";
            /// <summary>
            /// Email address does not adhere to specifications
            /// </summary>
            public const string EMAIL_INVALID = "Email address is not valid";
            /// <summary>
            /// Password not specified
            /// </summary>
            public const string PASSWORD_EMPTY = "Password cannot be empty";
            /// <summary>
            /// Password does not adhere to specifications
            /// </summary>
            public const string PASSWORD_INVALID = "Password is not valid";
            /// <summary>
            /// Password is no longer valid
            /// </summary>
            public const string PASSWORD_EXPIRED = "Password has expired";
            /// <summary>
            /// Specified account does not exist
            /// </summary>
            public const string ACCOUNT_DNE = "The specified user account does not exist";
            /// <summary>
            /// User not allowed to login
            /// </summary>
            public const string LOGIN_FORBIDDEN = "The specified user account cannot log in";
            /// <summary>
            /// Account is currently locked
            /// </summary>
            public const string ACCOUNT_LOCKOUT = "The specified user account is temporarily locked. Please try again in 5 minutes";
            /// <summary>
            /// Account has not been confirmed by Email
            /// </summary>
            public const string ACCOUNT_NOT_CONFIRMED = "The specified user account has not yet been confirmed. The associated email address must be confirmed before logging in";
            /// <summary>
            /// Account has not been approved by admins
            /// </summary>
            public const string ACCOUNT_NOT_APPROVED = "The specified user account has not yet been approved by an admin.  Please try again later";
            /// <summary>
            /// Account disabled for unknown reason
            /// </summary>
            public const string ACCOUNT_DISABLED = "The specified user account has been disabled";
            /// <summary>
            /// Unable to authenticate user
            /// </summary>
            public const string GENERIC_LOGIN_FAILURE = "Invalid email/password combination.";
        }

        /// <summary>
        /// Error strings for Recaptcha validation errors
        /// </summary>
        public static class RecaptchaError
        {
            /// <summary>
            /// Recaptcha key is not provided
            /// </summary>
            public const string RECAPTCHA_MISSING = "Recaptcha is required";
            /// <summary>
            /// Recaptcha key is not valid
            /// </summary>
            public const string RECAPTCHA_INVALID = "Recaptcha is invalid";
        }

        /// <summary>
        /// Error strings for CMS core entity errors
        /// </summary>
        public static class EntManagement
        {
            /// <summary>
            /// Generic splash message
            /// </summary>
            public const string DEFAULT_SPLASH = "This site is currently under construction.  Please check again soon for new content.";
        }
    }
}
