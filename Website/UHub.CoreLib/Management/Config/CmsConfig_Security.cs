using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Security;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_Security
    {
        /// <summary>
        /// Hash algorithm used by password manager
        /// <para></para>
        /// Default: BCrypt
        /// </summary>
        public CryptoHashType PswdHashType { get; set; } = CryptoHashType.HMACSHA512;
        /// <summary>
        /// Maximum age that a password is valid before expiration (0 is infinite)
        /// <para></para>
        /// Default: 0
        /// </summary>
        public TimeSpan MaxPswdAge { get; private set; } = new TimeSpan(0, 0, 0);
        /// <summary>
        /// Maximum number of invalid password attempts before user account is locked
        /// <para></para>
        /// Default: 10
        /// </summary>
        public byte MaxPswdAttempts { get; set; } = 10;
        /// <summary>
        /// Period of time in which the maximum number of invalid password attempts must occur
        /// <para></para>
        /// Default: 5 minutes
        /// </summary>
        public TimeSpan PswdAttemptPeriod { get; set; } = new TimeSpan(0, 5, 0);
        /// <summary>
        /// Period of time after which an account is automatically unlocked
        /// <para></para>
        /// Default: 5 minutes
        /// </summary>
        public TimeSpan PswdLockResetPeriod { get; set; } = new TimeSpan(0, 5, 0);
        /// <summary>
        /// Flag for whether or not users/admins can generate a password reset email with temp psd
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool EnablePswdRecovery { get; set; } = true;
        /// <summary>
        /// Account password reset proxy
        /// Users will use this link + RecoveryID to reset their passwords
        /// </summary>
        public string AcctPswdRecoveryURL { get; set; }
        /// <summary>
        /// Account password update proxy
        /// Users will use this link to update their passwords (after logging in)
        /// </summary>
        public string AcctPswdUpdateURL { get; set; }
        /// <summary>
        /// Account password recovery link expiration length
        /// Specify how long users will have to change their passwords after a recovery context is created (0 is infinite)
        /// <para></para>
        /// Default: 1 day
        /// </summary>
        public TimeSpan AcctPswdRecoveryLifespan { get; set; } = new TimeSpan(1, 0, 0, 0);
        /// <summary>
        /// Force all site content to use secure SSL protected connections.
        /// An SSL certificate must be configured through IIS in order for this setting to work properly.
        /// Must be enabled if <see cref="ForceSecureCookies"/> is enabled
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool ForceHTTPS { get; set; } = true;
        /// <summary>
        /// Force HTTP response headers that help protect resource access and conceal hosting configurations
        /// Must be enabled if <see cref="ForceHTTPS"/> is enabled
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool ForceSecureResponseHeaders { get; set; } = true;
        /// <summary>
        /// Force cookies to use HTTPS_ONLY mode.
        /// Must be enabled if <see cref="ForceHTTPS"/> is enabled
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool ForceSecureCookies { get; set; } = true;
        /// <summary>
        /// Explicit cookie domain for any CMS cookies (including authentication cookie)
        /// <para></para>
        /// Default: null
        /// </summary>
        public string CookieDomain { get; set; } = null;
        /// <summary>
        /// Set cookie SameSite attribute value
        /// <para></para>
        /// Default: Off
        /// </summary>
        public CookieSameSiteModes CookieSameSiteMode { get; set; } = CookieSameSiteModes.Off;
        /// <summary>
        /// Authentication token lifespan.
        /// If a user is inactive for the entire timeout length, then the session will be expired
        /// <para></para>
        /// Default: 3 hours
        /// </summary>
        public TimeSpan AuthTokenTimeout { get; set; } = new TimeSpan(0, 3, 0, 0);
        /// <summary>
        /// Allow user authentication tokens to renew as long as a user is active
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool EnableAuthTokenSlidingExpiration { get; set; } = true;
        /// <summary>
        /// The longest that an authorization token can be valid even with a sliding expiration
        /// <para></para>
        /// Default: 1 day
        /// </summary>
        public TimeSpan MaxAuthTokenLifespan { get; set; } = new TimeSpan(1, 0, 0, 0);
        /// <summary>
        /// Enable auth tokens to persist on user machines.  Reduces overall system security
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnablePersistentAuthTokens { get; set; } = false;
        /// <summary>
        /// Default user login page.
        /// Unauthenticated users will be forwarded to this page when credentials are required
        /// </summary>
        public string LoginURL { get; set; }
        /// <summary>
        /// Default page for authenticated users.
        /// Users will be forwarded to this page after logging in if a specific page wasnt requested
        /// </summary>
        public string DefaultAuthFwdURL { get; set; }
        /// <summary>
        /// Confirmation proxy for new accounts
        /// Users will use this link + RefID to confirm their accounts
        /// </summary>
        public string AcctConfirmURL { get; set; }
        /// <summary>
        /// The longest that a confirmation token can be valid (0 is infinite)
        /// <para></para>
        /// Default: 5 days
        /// </summary>
        public TimeSpan AcctConfirmLifespan { get; set; } = new TimeSpan(5, 0, 0, 0);
        /// <summary>
        /// Flag for whether or not ReCaptcha should be checked at various user auth pages.
        /// Also requires client-side recaptcha configuration
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableRecaptcha { get; set; } = false;
        /// <summary>
        /// ReCaptcha private key (required if <see cref="EnableRecaptcha"/> is enabled)
        /// <para></para>
        /// Default: null
        /// </summary>
        public string RecaptchaPrivateKey { get; set; } = null;
        /// <summary>
        /// ReCaptcha public key (required if <see cref="EnableRecaptcha"/> is enabled)
        /// <para></para>
        /// Default: null
        /// </summary>
        public string RecaptchaPublicKey { get; set; } = null;
        /// <summary>
        /// Automatically approve new user accounts instead of waiting for admin approval
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool AutoApproveNewAccounts { get; set; } = false;
        /// <summary>
        /// Automatically confirm new user accounts instead of sending a confirmation email
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool AutoConfirmNewAccounts { get; set; } = false;
        /// <summary>
        /// Allow login tokens to to be invalidated by changing the source version number
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool EnableTokenVersioning { get; set; } = true;
        /// <summary>
        /// Determine how when the HtmlSanitizer module is called during the CMS pipeline
        /// </summary>
        public HtmlSanitizerMode HtmlSanitizerMode { get; set; } = HtmlSanitizerMode.Both;
    }
}
