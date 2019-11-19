using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib
{
    public static class Constants
    {
        /// <summary>
        /// Name of the Web.Server Application variable name to be used when referencing the global TIPCCMS instance
        /// </summary>
        public const string APP_VAR_NAME = "UHUB_INSTANCE";

        /// <summary>
        /// Primary URL prefix for WebAPI Service endpoints
        /// </summary>
        public const string API_ROUTE_PREFIX = "uhubapi";

        /// <summary>
        /// Header used to supply client machine key to authentication system
        /// </summary> 
        public const string AUTH_HEADER_MACHINE_KEY = "client-machine-key";

        /// <summary>
        /// Header used to supply client auth token
        /// </summary>
        public const string AUTH_HEADER_TOKEN = "uhub-auth-token";

        /// <summary>
        /// ID of the primary system level user in the DB
        /// </summary>
        public const long SYSTEM_USER_ID = 0L;

    }
}
