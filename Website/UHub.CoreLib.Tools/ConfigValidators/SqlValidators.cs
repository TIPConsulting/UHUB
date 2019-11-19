using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Tools
{
    public static partial class ConfigValidators
    {

        /// <summary>
        /// Ensure that the system can connect to the specified DB with the provided credentials
        /// </summary>
        /// <param name="config">SqlConfig to test</param>
        /// <param name="argName">Name of the config variable</param>
        public static void ValidateSqlConfig(SqlConfig config, string argName)
        {
            bool isValid = false;
            try
            {
                isValid = config.ValidateConnection();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(argName + ": " + ex.Message);
            }

            if (!isValid)
            {
                throw new ArgumentException(argName + $" is not valid.  Cannot connect to DB with the specified connection string.");
            }

        }

    }
}
