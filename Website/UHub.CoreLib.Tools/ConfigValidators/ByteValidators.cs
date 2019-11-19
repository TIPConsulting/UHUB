using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    public static partial class ConfigValidators
    {

        /// <summary>
        /// Ensure that byte is not 0
        /// </summary>
        /// <param name="byteVal">Byte to test</param>
        /// <param name="argName">Name of byte variable</param>
        public static void ValidateByte(byte byteVal, string argName)
        {
            if (byteVal == 0)
            {
                throw new ArgumentException(argName + " cannot be 0.");
            }
        }

    }
}
