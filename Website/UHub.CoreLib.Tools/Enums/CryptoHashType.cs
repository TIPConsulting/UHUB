using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    public enum CryptoHashType
    {
        /// <summary>
        /// Keyed hash
        /// </summary>
        HMACSHA256,
        /// <summary>
        /// Unkeyed hash
        /// </summary>
        SHA512,
        /// <summary>
        /// Keyed hash
        /// </summary>
        HMACSHA512,
        /// <summary>
        /// Unkeyed hash
        /// </summary>
        Bcrypt,
        /// <summary>
        /// Keyed hash - only use on high performance servers
        /// </summary>
        Argon2,
        /// <summary>
        /// Keyed hash
        /// </summary>
        HMACBlake2
    }
}
