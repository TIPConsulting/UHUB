using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Authentication
{
    /// <summary>
    /// Status returned when token is validated
    /// </summary>
    public enum TokenValidationStatus
    {
        /// <summary>
        /// Token validation succeeded
        /// </summary>
        Success = 0,
        /// <summary>
        /// Token was processed, but returned an anonymous user
        /// </summary>
        AnonUser = 1,
        /// <summary>
        /// Client token not found
        /// </summary>
        TokenNotFound = 100,
        /// <summary>
        /// Client token could not be validated
        /// </summary>
        TokenInvalid = 101,
        /// <summary>
        /// Client token could not be decrypted
        /// </summary>
        TokenAESFailure = 102,
        /// <summary>
        /// Client token has expired
        /// </summary>
        TokenExpired = 103,
        /// <summary>
        /// Client token does not have current expected version
        /// </summary>
        TokenVersionMismatch = 104,
        /// <summary>
        /// Client token has been revoked
        /// </summary>
        TokenValidatorRevoked = 200,
        /// <summary>
        /// Client token does not have a DB validator
        /// </summary>
        TokenValidatorNotFound = 201,
        /// <summary>
        /// Client token does not match expected validator
        /// </summary>
        TokenValidatorMismatch = 202,
        /// <summary>
        /// Token validator could be validated
        /// </summary>
        TokenValidatorInvalid = 203,
        /// <summary>
        /// Client token does not match current client session
        /// </summary>
        TokenSessionMismatch = 300,
        /// <summary>
        /// The user encapsulated by the token could not be validated
        /// </summary>
        TokenUserError = 400,
        /// <summary>
        /// The user encapsulated by the token is not allowed to login
        /// </summary>
        TokenUserForbidden = 401,
        /// <summary>
        /// The user encapsulated by the token is not confirmed
        /// </summary>
        TokenUserNotConfirmed = 402,
        /// <summary>
        /// The user encapsulated by the token is not approved
        /// </summary>
        TokenUserNotApproved = 403,
        /// <summary>
        /// The user encapsulated by the token has been disabled
        /// </summary>
        TokenUserDisabled = 404
    }
}
