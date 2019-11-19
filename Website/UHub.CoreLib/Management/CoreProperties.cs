using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using UHub.CoreLib.Management.Config;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Security;
using UHub.CoreLib.EmailInterop;
using UHub.CoreLib.EmailInterop.Templates;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Util;
using UHub.CoreLib.EmailInterop.Providers;

namespace UHub.CoreLib.Management
{
    /// <summary>
    /// CMS configuration store.  Readonly copy of the CMS config initializer
    /// </summary>
    public sealed class CoreProperties
    {

        /// <summary>
        /// Create readonly copy of a config module
        /// </summary>
        /// <param name="cmsConfig"></param>
        public static explicit operator CoreProperties(CmsConfiguration_Grouped cmsConfig)
        {
            cmsConfig.Validate();



            var Properties = new CoreProperties();

            //INSTANCE
            Properties.SiteFriendlyName = cmsConfig.Instance.SiteFriendlyName;
            Properties.SessionIDCookieName = cmsConfig.Instance.SessionIDCookieName;
            //DB CONNECTIONS
            Properties.CmsDBConfig = new SqlConfig(cmsConfig.DB.CmsDBConfig);
            Properties.EnableDBMultithreading = cmsConfig.DB.EnableDBMultithreading;
            //STORAGE CONNECTIONS
            Properties.FileStoreDirectory = Converters.GetAbsFileDirectory(cmsConfig.Storage.FileStoreDirectory);
            Properties.ImageStoreDirectory = Converters.GetAbsFileDirectory(cmsConfig.Storage.ImageStoreDirectory);
            Properties.TempCacheDirectory = Converters.GetAbsFileDirectory(cmsConfig.Storage.TempCacheDirectory);
            //MAIL CONNECTIONS
            //TODO: finish deep copy constructor
            Properties.MailProvider = cmsConfig.Mail.MailProvider;
            Properties.ContactFormRecipientAddress = cmsConfig.Mail.ContactFormRecipientAddress;
            //SECURITY
            Properties.PswdHashType = cmsConfig.Security.PswdHashType;
            Properties.MaxPswdAge = new TimeSpan(cmsConfig.Security.MaxPswdAge.Ticks);
            Properties.MaxPswdAttempts = cmsConfig.Security.MaxPswdAttempts;
            Properties.PswdAttemptPeriod = new TimeSpan(cmsConfig.Security.PswdAttemptPeriod.Ticks);
            Properties.PswdLockResetPeriod = cmsConfig.Security.PswdLockResetPeriod;
            Properties.EnablePswdRecovery = cmsConfig.Security.EnablePswdRecovery;
            Properties.AcctPswdRecoveryURL = Converters.GetAbsURL(cmsConfig.Instance.CmsPublicBaseURL, cmsConfig.Security.AcctPswdRecoveryURL);
            Properties.AcctPswdUpdateURL = Converters.GetAbsURL(cmsConfig.Instance.CmsPublicBaseURL, cmsConfig.Security.AcctPswdUpdateURL);
            Properties.AcctPswdRecoveryLifespan = cmsConfig.Security.AcctPswdRecoveryLifespan;
            Properties.ForceHTTPS = cmsConfig.Security.ForceHTTPS;
            Properties.ForceSecureCookies = cmsConfig.Security.ForceSecureCookies;
            Properties.CookieDomain = cmsConfig.Security.CookieDomain;
            Properties.CookieSameSiteMode = cmsConfig.Security.CookieSameSiteMode;
            Properties.AuthTokenTimeout = new TimeSpan(cmsConfig.Security.AuthTokenTimeout.Ticks);
            Properties.EnableAuthTokenSlidingExpiration = cmsConfig.Security.EnableAuthTokenSlidingExpiration;
            Properties.MaxAuthTokenLifespan = new TimeSpan(cmsConfig.Security.MaxAuthTokenLifespan.Ticks);
            Properties.EnablePersistentAuthTokens = cmsConfig.Security.EnablePersistentAuthTokens;
            Properties.LoginURL = Converters.GetAbsURL(cmsConfig.Instance.CmsPublicBaseURL, cmsConfig.Security.LoginURL);
            Properties.DefaultAuthFwdURL = Converters.GetAbsURL(cmsConfig.Instance.CmsPublicBaseURL, cmsConfig.Security.DefaultAuthFwdURL);
            Properties.AcctConfirmURL = Converters.GetAbsURL(cmsConfig.Instance.CmsPublicBaseURL, cmsConfig.Security.AcctConfirmURL);
            Properties.AcctConfirmLifespan = cmsConfig.Security.AcctConfirmLifespan;
            Properties.EnableRecaptcha = cmsConfig.Security.EnableRecaptcha;
            Properties.RecaptchaPrivateKey = cmsConfig.Security.RecaptchaPrivateKey;
            Properties.RecaptchaPublicKey = cmsConfig.Security.RecaptchaPublicKey;
            Properties.AutoApproveNewAccounts = cmsConfig.Security.AutoApproveNewAccounts;
            Properties.AutoConfirmNewAccounts = cmsConfig.Security.AutoConfirmNewAccounts;
            Properties.EnableTokenVersioning = cmsConfig.Security.EnableTokenVersioning;
            Properties.HtmlSanitizerMode = cmsConfig.Security.HtmlSanitizerMode;
            //ERRORS
            Properties.EnableCustomErrorCodes = cmsConfig.Errors.EnableCustomErrorCodes;
            //API
            Properties.EnableDetailedAPIErrors = cmsConfig.API.EnableDetailedAPIErrors;
            Properties.EnableInternalAPIErrors = cmsConfig.API.EnableInternalAPIErrors;
            Properties.EnableAPIFileUploads = cmsConfig.API.EnableAPIFileUploads;
            Properties.MaxFileUploadSize = new FileSize(FileSizeUnit.Bytes, cmsConfig.API.MaxFileUploadSize.GetAsBytes());
            Properties.AllowedFileUploadTypes = cmsConfig.API.AllowedFileUploadTypes.ToList().ToHashSet();

            return Properties;
        }


        public CoreProperties()
        {
            //Get random number for current token version - prevents old tokens from persisting after a system reset
            ResetCurrentAuthTknVersion();


            this.CmsVersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }


        //--------------------------------------------------------------------------------------------------
        //-------------------------------------------- INSTANCE --------------------------------------------
        //--------------------------------------------------------------------------------------------------
        public string CmsVersionNumber { get; } = "1.0.0.0";
        /// <summary>
        /// CMS DB schema version validation object
        /// </summary>
        public SchemaVersion CmsSchemaVersion { get; } = new SchemaVersion(0.1M, 0.1M, 0.1M, 0.1M);
        /// <summary>
        /// Website friendly name that can be used in the UI and emails
        /// </summary>
        public string SiteFriendlyName { get; private set; }
        /// <summary>
        /// Public site root address including protocol (ex: https://test.test)
        /// </summary>
        public string PublicBaseURL { get; private set; }
        /// <summary>
        /// Public site static resource address including protocol (ex: https://test.test)
        /// </summary>
        public string StaticResourceURL { get; private set; }
        /// <summary>
        /// Name of the cookie that stores the user Session ID
        /// </summary>
        public string SessionIDCookieName { get; private set; }
        //--------------------------------------------------------------------------------------------------
        //----------------------------------------- DB CONNECTIONS -----------------------------------------
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Connection information for default CMS DB
        /// </summary>
        public SqlConfig CmsDBConfig { get; private set; }
        /// <summary>
        /// Allow mutlithreaded DB processing. Might improve DB read times depending on server configuration
        /// </summary>
        public bool EnableDBMultithreading { get; private set; }
        //--------------------------------------------------------------------------------------------------
        //--------------------------------------- STORAGE CONNECTIONS --------------------------------------
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// URL for google document viewer - enables users to preview files in browser
        /// </summary>
        public string DocViewerURL => "https://docs.google.com/viewer?url=";
        /// <summary>
        /// File directory used for storing CMS files
        /// </summary>
        public string FileStoreDirectory { get; private set; }
        /// <summary>
        /// File directory used for storing CMS images
        /// </summary>
        public string ImageStoreDirectory { get; private set; }
        /// <summary>
        /// File directory used for storing temporary user file uploads
        /// </summary>
        public string TempCacheDirectory { get; private set; }
        /// <summary>
        /// File directory used for storing CMS log files
        /// </summary>
        public string LogStoreDirectory { get; private set; }
        //--------------------------------------------------------------------------------------------------
        //---------------------------------------- MAIL CONNECTIONS ----------------------------------------
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Email provider for sending messages
        /// </summary>
        public EmailProvider MailProvider { get; set; } = null;
        /// <summary>
        /// The email address that Contact Us emails will be delivered to
        /// </summary>
        public string ContactFormRecipientAddress { get; private set; }
        //--------------------------------------------------------------------------------------------------
        //-------------------------------------------- SECURITY --------------------------------------------
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Hash algorithm used by password manager
        /// </summary>
        public CryptoHashType PswdHashType { get; private set; } = CryptoHashType.HMACSHA512;
        /// <summary>
        /// Minimum length of user passwords
        /// </summary>
        public const int MinPswdLength = 8;
        /// <summary>
        /// Maximum length of user passwords
        /// </summary>
        public const int MaxPswdLength = 150;
        /// <summary>
        /// Maximum age that a password is valid before expiration (0 is infinite)
        /// </summary>
        public TimeSpan MaxPswdAge { get; private set; }
        //ASSOC: 5F6FC523-2852-4C5A-91A0-3A3F05556594
        /// <summary>
        /// Regular expression to validate password strength
        /// </summary>
        public string PswdStrengthRegex => $@"^.{{{MinPswdLength},{MaxPswdLength}}}$";
        /// <summary>
        /// Maximum number of invalid password attempts before user account is locked
        /// </summary>
        public byte MaxPswdAttempts { get; private set; }
        /// <summary>
        /// Period of time in which the maximum number of invalid password attempts must occur
        /// </summary>
        public TimeSpan PswdAttemptPeriod { get; private set; }
        /// <summary>
        /// Period of time after which an account is automatically unlocked
        /// </summary>
        public TimeSpan PswdLockResetPeriod { get; private set; }
        /// <summary>
        /// Flag for whether or not users/admins can generate a password reset email with temp psd
        /// </summary>
        public bool EnablePswdRecovery { get; private set; }
        /// <summary>
        /// Account password reset proxy
        /// Users will use this link + RecoveryID to reset their passwords
        /// </summary>
        public string AcctPswdRecoveryURL { get; private set; }
        /// <summary>
        /// Account password update proxy
        /// Users will use this link to update their passwords (after logging in)
        /// </summary>
        public string AcctPswdUpdateURL { get; private set; }
        /// <summary>
        /// The maximum number of times that a user can attempt to reset password before recovery context is invalidated
        /// </summary>
        public byte AcctPswdResetMaxAttemptCount => 10;
        /// <summary>
        /// Account password reset link expiration length
        /// Specify how long users will have to change their passwords after a reset context is created (0 is infinite)
        /// </summary>
        public TimeSpan AcctPswdRecoveryLifespan { get; private set; }
        /// <summary>
        /// Force all site content to use secure SSL protected connections.
        /// An SSL certificate must be configured through IIS in order for this setting to work properly.
        /// Must be enabled if <see cref="ForceSecureCookies"/> is enabled
        /// </summary>
        public bool ForceHTTPS { get; private set; }
        /// <summary>
        /// Force HTTP response headers that help protect resource access and conceal hosting configurations
        /// Must be enabled if <see cref="ForceHTTPS"/> is enabled
        /// </summary>
        public bool ForceSecureCookies { get; private set; }
        /// <summary>
        /// Explicit cookie domain for any CMS cookies (including authentication cookie)
        /// </summary>
        public string CookieDomain { get; private set; }
        /// <summary>
        /// Set cookie SameSite attribute value
        /// </summary>
        public CookieSameSiteModes CookieSameSiteMode { get; private set; }
        /// <summary>
        /// Current system level authentication token version
        /// </summary>
        public int CurrentAuthTknVersion { get; private set; } = 1;
        /// <summary>
        /// Authentication token activity timeout.
        /// If a user is inactive for the entire timeout length, then the session will be expired
        /// <para></para>
        /// Default: 3 hours
        /// </summary>
        public TimeSpan AuthTokenTimeout { get; private set; }
        /// <summary>
        /// Allow user authentication tokens to renew as long as a user is active
        /// </summary>
        public bool EnableAuthTokenSlidingExpiration { get; private set; }
        /// <summary>
        /// The longest that an authorization token can be valid even with a sliding expiration
        /// </summary>
        public TimeSpan MaxAuthTokenLifespan { get; private set; }
        /// <summary>
        /// Enable auth tokens to persist on user machines.  Reduces overall system security
        /// </summary>
        public bool EnablePersistentAuthTokens { get; private set; }
        /// <summary>
        /// Default user login page.
        /// Unauthenticated users will be forwarded to this page when credentials are required
        /// </summary>
        public string LoginURL { get; private set; }
        /// <summary>
        /// Default page for authenticated users.
        /// Users will be forwarded to this page after logging in if a specific page wasnt requested
        /// </summary>
        public string DefaultAuthFwdURL { get; private set; }
        /// <summary>
        /// Confirmation proxy for new accounts
        /// Users will use this link + RefID to confirm their accounts
        /// </summary>
        public string AcctConfirmURL { get; private set; }
        /// <summary>
        /// The longest that a confirmation token can be valid (0 is infinite)
        /// <para></para>
        /// Default: 5 days
        /// </summary>
        public TimeSpan AcctConfirmLifespan { get; set; }
        /// <summary>
        /// Flag for whether or not ReCaptcha should be checked at various user auth pages.
        /// Also requires client-side recaptcha configuration
        /// </summary>
        public bool EnableRecaptcha { get; private set; }
        /// <summary>
        /// ReCaptcha private key (required if <see cref="EnableRecaptcha"/> is enabled)
        /// </summary>
        public string RecaptchaPrivateKey { get; private set; }
        /// <summary>
        /// ReCaptcha public key (required if <see cref="EnableRecaptcha"/> is enabled)
        /// </summary>
        public string RecaptchaPublicKey { get; private set; }
        /// <summary>
        /// Cookie name for storing user authentication token
        /// </summary>
        public string AuthTknCookieName => "UHub_AuthTkn";
        /// <summary>
        /// Cookie name for storing authentication forward URL
        /// </summary>
        public string PostAuthCookieName => "UHub_PostAuthFwrdUrl";
        /// <summary>
        /// Automatically approve new user accounts instead of waiting for admin approval
        /// </summary>
        public bool AutoApproveNewAccounts { get; private set; }
        /// <summary>
        /// Automatically confirm new user accounts instead of sending a confirmation email
        /// </summary>
        public bool AutoConfirmNewAccounts { get; private set; }
        /// <summary>
        /// Allow login tokens to to be invalidated by changing the source version number
        /// </summary>
        public bool EnableTokenVersioning { get; private set; }
        /// <summary>
        /// Determine how when the HtmlSanitizer module is called during the CMS pipeline
        /// </summary>
        public HtmlSanitizerMode HtmlSanitizerMode { get; private set; }
        //--------------------------------------------------------------------------------------------------
        //--------------------------------------------- LOGGING --------------------------------------------
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Name of session variable that stores logging ID for this session
        /// </summary>
        public string SessionLogIDName => "UHub_SessionID";
        //--------------------------------------------------------------------------------------------------
        //--------------------------------------------- ERRORS ---------------------------------------------
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Allows error pages to show error status (otherwise all error pages return code 200)
        /// </summary>
        public bool EnableCustomErrorCodes { get; private set; }
        //--------------------------------------------------------------------------------------------------
        //----------------------------------------------- API ----------------------------------------------
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Allow API calls to display detailed error messages regarding failures
        /// </summary>
        public bool EnableDetailedAPIErrors { get; private set; }
        /// <summary>
        /// Allow API calls to display GUID error messages regarding failures
        /// </summary>
        public bool EnableInternalAPIErrors { get; private set; }
        /// <summary>
        /// Allow users to upload files via API
        /// </summary>
        public bool EnableAPIFileUploads { get; private set; }
        /// <summary>
        /// Maximum file size that can be uploaded by files (0 is infinite)
        /// </summary>
        public FileSize MaxFileUploadSize { get; private set; }
        /// <summary>
        /// List of allowable file categories. (Empty is all)
        /// </summary>
        public HashSet<FileCategory> AllowedFileUploadTypes { get; private set; }





        /// <summary>
        /// Change the system token version number to invalidate all current tokens
        /// </summary>
        internal void ResetCurrentAuthTknVersion()
        {
            //force a new positive, non-zero version number
            int oldAuthV = CurrentAuthTknVersion;
            int newAuthV = CurrentAuthTknVersion;
            do
            {
                newAuthV = new Random().Next(100) + 1;
            }
            while (oldAuthV == newAuthV);

            CurrentAuthTknVersion = newAuthV;
        }

    }

}
