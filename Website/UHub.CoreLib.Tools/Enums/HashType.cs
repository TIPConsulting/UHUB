using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    /// <summary>
    /// Supported hashing algorithms
    /// </summary>
    public enum HashType
    {
        MD5,
        SHA1,
        SHA256,
        HMACSHA256,
        SHA512,
        HMACSHA512
    }
}
