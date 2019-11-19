using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Security.Authentication.Management
{
    internal static partial class TokenManager
    {


        internal static TokenValidator GetValidator(AuthenticationToken token)
        {
            TokenValidator validator = null;

            try
            {
                validator = DataInterop.SqlWorker.ExecEntityQuery<TokenValidator>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[User_GetTokenValidator]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@IssueDate", SqlDbType.DateTimeOffset).Value = token.IssueDate.UtcDateTime;
                        cmd.Parameters.Add("@TokenID", SqlDbType.NVarChar).Value = token.TokenID;
                    })
                    .SingleOrDefault();
            }
            catch (Exception ex)
            {
                var exID = new Guid("14C7A3B5-9D4B-4FCA-B8E4-0D68192A78B8");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }


            return validator;
        }

        internal static void SaveTokenValidatorToDB(AuthenticationToken token)
        {
            DateTimeOffset issue = token.IssueDate.UtcDateTime;
            DateTimeOffset expiration;

            var maxLifeSpan = CoreFactory.Singleton.Properties.MaxAuthTokenLifespan;

            if (maxLifeSpan.Ticks == 0)
            {
                expiration = DateTimeOffset.MaxValue;
            }
            else
            {
                expiration = issue.Add(maxLifeSpan).UtcDateTime;
            }


            DataInterop.SqlWorker.ExecNonQuery(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_CreateTokenValidator]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@IssueDate", SqlDbType.DateTimeOffset).Value = issue;
                    cmd.Parameters.Add("@MaxExpirationDate", SqlDbType.DateTimeOffset).Value = expiration;
                    cmd.Parameters.Add("@TokenID", SqlDbType.NVarChar).Value = token.TokenID;
                    cmd.Parameters.Add("@IsPersistent", SqlDbType.Bit).Value = token.IsPersistent;
                    cmd.Parameters.Add("@TokenHash", SqlDbType.NVarChar).Value = token.GetTokenHash();
                    cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = token.SessionID ?? "";
                    cmd.Parameters.Add("@RequestID", SqlDbType.NVarChar).Value = "";    //NOT USED
                });
        }

        internal static void RevokeTokenValidator(AuthenticationToken token)
        {
            DataInterop.SqlWorker.ExecNonQuery(
                CoreFactory.Singleton.Properties.CmsDBConfig,
                "[dbo].[User_RevokeTokenValidator]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@IssueDate", SqlDbType.DateTimeOffset).Value = token.IssueDate;
                    cmd.Parameters.Add("@TokenID", SqlDbType.NVarChar).Value = token.TokenID;
                });
        }


        internal static void PurgeExpiredTokenValidators()
        {
            DataInterop.SqlWorker.ExecNonQuery(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[Users_PurgeExpiredTokenValidators]", 
               null);
        }

        /// <summary>
        /// Checks token against DB to ensure that the given token was issued by the security system and has not been revoked.  Also checks Date, SessionID, and RequestToken
        /// </summary>
        /// <param name="token"></param>
        /// <param name="SessionID"></param>
        /// <param name="tokenStatus"></param>
        /// <returns></returns>
        internal static TokenValidationStatus IsTokenValid(AuthenticationToken token, string SessionID)
        {

            var now = FailoverDateTimeOffset.UtcNow;

            TokenValidator validator = GetValidator(token);

            if (validator == null)
            {
                return TokenValidationStatus.TokenValidatorNotFound;
            }
            //check revocation
            if (!validator.IsValid)
            {
                return TokenValidationStatus.TokenValidatorRevoked;
            }
            //check for date cohesion
            if (token.IssueDate != validator.IssueDate)
            {
                return TokenValidationStatus.TokenValidatorInvalid;
            }
            //ensure that the issue date has already passed
            if (token.IssueDate.UtcDateTime > now)
            {
                return TokenValidationStatus.TokenInvalid;
            }
            //ensure that the expiration date is in the future
            //ensure that the MaxAge is in the future
            if (token.ExpirationDate.UtcDateTime < now || validator.MaxExpirationDate < now)
            {
                return TokenValidationStatus.TokenExpired;
            }
            //ensure token/validator cohesion
            //hash
            //sessionID
            //persistence
            //requestToken
            if (token.GetTokenHash() != validator.TokenHash || token.SessionID != validator.SessionID || token.IsPersistent != validator.IsPersistent)
            {
                return TokenValidationStatus.TokenValidatorMismatch;
            }
            //ensure that the token is still pinned to the proper client session
            //only important is token is not persistent

            if (!token.IsPersistent && token.SessionID != SessionID)
            {
                return TokenValidationStatus.TokenSessionMismatch;
            }


            return TokenValidationStatus.Success;
        }


    }
}
