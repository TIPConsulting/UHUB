using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.Hosting;
using System.Data;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Util;
using UHub.CoreLib.RegExp;


namespace UHub.CoreLib.Management.Config
{
    /// <summary>
    /// Public CMS configuration module.  Client applications must properly configure the system for use
    /// </summary>
    public sealed class CmsConfiguration_Grouped
    {

        public CmsConfig_Instance Instance = null;
        public CmsConfig_DB DB = null;
        public CmsConfig_Storage Storage = null;
        public CmsConfig_Mail Mail = null;
        public CmsConfig_Security Security = null;
        public CmsConfig_Logging Logging = null;
        public CmsConfig_Errors Errors = null;
        public CmsConfig_Caching Caching = null;
        public CmsConfig_API API = null;


        public CmsConfiguration_Grouped()
        {

        }


        /// <summary>
        /// Basic validation for configuration settings
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public bool Validate()
        {
            if (Instance == null)
            {
                string err = $"{nameof(CmsConfig_Instance)} cannot be null";
                throw new ArgumentException(err);
            }
            if (DB == null)
            {
                string err = $"{nameof(CmsConfig_DB)} cannot be null";
                throw new ArgumentException(err);
            }
            if (Storage == null)
            {
                string err = $"{nameof(CmsConfig_Storage)} cannot be null";
                throw new ArgumentException(err);
            }
            if (Mail == null)
            {
                string err = $"{nameof(CmsConfig_Mail)} cannot be null";
                throw new ArgumentException(err);
            }
            if (Security == null)
            {
                string err = $"{nameof(CmsConfig_Security)} cannot be null";
                throw new ArgumentException();
            }
            if (Logging == null)
            {
                string err = $"{nameof(CmsConfig_Logging)} cannot be null";
                throw new ArgumentException(err);
            }
            if (Errors == null)
            {
                string err = $"{nameof(CmsConfig_Errors)} cannot be null";
                throw new ArgumentException(err);
            }
            if (Caching == null)
            {
                string err = $"{nameof(CmsConfig_Caching)} cannot be null";
                throw new ArgumentException(err);
            }
            if (API == null)
            {
                string err = $"{nameof(CmsConfig_API)} cannot be null";
                throw new ArgumentException(err);
            }


            //friendly name
            ConfigValidators.ValidateString(Instance.SiteFriendlyName, nameof(Instance.SiteFriendlyName));
            //base url (only URL that cant be virtual)
            ConfigValidators.ValidateUrl(Instance.CmsPublicBaseURL, nameof(Instance.CmsPublicBaseURL), Security.ForceHTTPS, EnableVirtual: false);

            //resource url
            ConfigValidators.ValidateUrl(Instance.CmsStaticResourceURL, nameof(Instance.CmsStaticResourceURL), Security.ForceHTTPS);
            ConfigValidators.ValidateString(Instance.SessionIDCookieName, nameof(Instance.SessionIDCookieName));

            //DB CONNECTIONS
            ConfigValidators.ValidateSqlConfig(DB.CmsDBConfig, nameof(DB.CmsDBConfig));
            if (!ValidateCmsDB())
            {
                string err = $"{nameof(DB.CmsDBConfig)} is not properly configured.  Please ensure that the TIPC CMS module is installed and your username/password are accurate.";
                throw new ArgumentException(err);
            }

            //STORAGE CONNECTIONS
            ConfigValidators.ValidateDirectory(Storage.FileStoreDirectory, nameof(Storage.FileStoreDirectory), Storage.CreateMissingStorageDirectories);
            ConfigValidators.ValidateDirectory(Storage.ImageStoreDirectory, nameof(Storage.ImageStoreDirectory), Storage.CreateMissingStorageDirectories);
            ConfigValidators.ValidateDirectory(Storage.TempCacheDirectory, nameof(Storage.TempCacheDirectory), Storage.CreateMissingStorageDirectories);


            //LOGGING
            if ((Logging.UsageLogMode & UsageLoggingMode.GoogleAnalytics) != 0)
            {
                ConfigValidators.ValidateString(Logging.GoogleAnalyticsKey, nameof(Logging.GoogleAnalyticsKey));
            }



            //SECURITY
            ConfigValidators.ValidateTimeSpan_NonNeg(Security.AcctPswdRecoveryLifespan, nameof(Security.AcctPswdRecoveryLifespan));
            ConfigValidators.ValidateTimeSpan_NonNeg(Security.AcctConfirmLifespan, nameof(Security.AcctConfirmLifespan));
            ConfigValidators.ValidateTimeSpan_NonNeg(Security.MaxPswdAge, nameof(Security.MaxPswdAge));
            ConfigValidators.ValidateTimeSpan_NonNeg(Security.MaxAuthTokenLifespan, nameof(Security.MaxAuthTokenLifespan));
            ConfigValidators.ValidateTimeSpan_Pos(Security.AuthTokenTimeout, nameof(Security.AuthTokenTimeout));


            if (Security.MaxPswdAge != null && Security.MaxPswdAge.Ticks > 0)
            {
                if (!Security.EnablePswdRecovery)
                {
                    string err = $"{nameof(Security.EnablePswdRecovery)} must be set if {nameof(Security.MaxPswdAge)} is set.";
                    throw new ArgumentException(err);
                }
            }
            if (Security.ForceHTTPS != Security.ForceSecureCookies)
            {
                string err = $"Security Mismatch - {nameof(Security.ForceHTTPS)} and {nameof(Security.ForceSecureCookies)} must be set to the same value.";
                throw new ArgumentException(err);
            }
            if (Security.CookieDomain.IsNotEmpty() && !Security.CookieDomain.Split(',').All(dmn => dmn.RgxIsMatch($@"^{RgxPtrns.Cookie.DOMAIN}$", RegexOptions.IgnoreCase)))
            {
                string err = "Invalid cookie domain format.";
                throw new ArgumentException(err);
            }
            if (Security.EnableRecaptcha)
            {
                ConfigValidators.ValidateString(Security.RecaptchaPrivateKey, nameof(Security.RecaptchaPrivateKey));
                ConfigValidators.ValidateString(Security.RecaptchaPublicKey, nameof(Security.RecaptchaPublicKey));
            }
            //check if the token timeout is longer than the token max age
            //but ensure max lifespan is not infinite
            if (Security.AuthTokenTimeout.Ticks > Security.MaxAuthTokenLifespan.Ticks && Security.MaxAuthTokenLifespan.Ticks > 0)
            {
                string err = nameof(Security.AuthTokenTimeout) + " must be greater than " + nameof(Security.MaxAuthTokenLifespan);
                throw new ArgumentException(err);
            }

            //--LOGIN URL
            ConfigValidators.ValidateUrl(Security.LoginURL, nameof(Security.LoginURL), Security.ForceHTTPS);
            //--DEFAULT URL
            ConfigValidators.ValidateUrl(Security.DefaultAuthFwdURL, nameof(Security.DefaultAuthFwdURL), Security.ForceHTTPS);
            //--ACCT CONFIRMATION
            if (!Security.AutoConfirmNewAccounts)
            {
                ConfigValidators.ValidateUrl(Security.AcctConfirmURL, nameof(Security.AcctConfirmURL), Security.ForceHTTPS);
            }

            //PASSWORD UPDATE
            ConfigValidators.ValidateUrl(Security.AcctPswdUpdateURL, nameof(Security.AcctPswdUpdateURL), Security.ForceHTTPS);


            //PASSWORD RESET
            if (Security.EnablePswdRecovery)
            {
                ConfigValidators.ValidateUrl(Security.AcctPswdRecoveryURL, nameof(Security.AcctPswdRecoveryURL), Security.ForceHTTPS);

                //make sure that MailClient/Pswd reset meta is set if the proxy address is set
                ConfigValidators.ValidateTimeSpan_Pos(Security.PswdAttemptPeriod, nameof(Security.PswdAttemptPeriod));
                ConfigValidators.ValidateTimeSpan_Pos(Security.PswdLockResetPeriod, nameof(Security.PswdLockResetPeriod));
            }


            //VALIDATE EMAIL PROVIDER IF REQUIRED
            if (!Security.AutoConfirmNewAccounts || Security.EnablePswdRecovery || Mail.ContactFormRecipientAddress.IsNotEmpty())
            {
                ConfigValidators.ValidateObject(Mail.MailProvider, nameof(Mail.MailProvider));

                Mail.MailProvider.Validate();
                ConfigValidators.ValidateEmail(Mail.ContactFormRecipientAddress, nameof(Mail.ContactFormRecipientAddress));
            }


            //CACHING
            if (Caching.EnableDBPageCaching)
            {
                ConfigValidators.ValidateTimeSpan_Pos(Caching.MaxDBCacheAge, nameof(Caching.MaxDBCacheAge));
            }
            if (Caching.EnableIISPageCaching)
            {
                ConfigValidators.ValidateTimeSpan_Pos(Caching.MaxStaticCacheAge, nameof(Caching.MaxStaticCacheAge));
                ConfigValidators.ValidateTimeSpan_Pos(Caching.MaxDynamicCacheAge, nameof(Caching.MaxDynamicCacheAge));
            }

            //API
            //DISABLE ALL API FUNCTIONS IF ROUTES ARE OFF
            if (!API.RegisterAPIRoutes)
            {
                API.EnableAPIFileUploads = false;
                API.EnableAPIAuthService = false;
            }
            if (API.EnableAPIFileUploads)
            {
                ConfigValidators.ValidateObject(API.MaxFileUploadSize, nameof(API.MaxFileUploadSize));
            }
            else
            {
                API.AllowedFileUploadTypes = new List<FileCategory>();
                API.MaxFileUploadSize = new FileSize(FileSizeUnit.Bytes, 1);
            }

            ConfigValidators.ValidateObject(API.AllowedFileUploadTypes, nameof(API.AllowedFileUploadTypes));


            return true;
        }




        /// <summary>
        /// Check DB for test table/view
        /// </summary>
        /// <returns></returns>
        private bool ValidateCmsDB()
        {

            var result = true;
            result = result && SqlHelper.DoesTableExist(DB.CmsDBConfig, "[dbo].[Users]");
            result = result && SqlHelper.DoesViewExist(DB.CmsDBConfig, "[dbo].[vSchools]");



            return result;

        }
    }
}
