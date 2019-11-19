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
        /// Ensure that timespan is not null/empty
        /// </summary>
        /// <param name="tSpan">Timespan to test</param>
        /// <param name="argName">Name of timespan variable</param>
        public static void ValidateTimeSpan(TimeSpan tSpan, string argName)
        {
            if (tSpan == null)
            {
                throw new ArgumentException(argName + " cannot be null.");
            }
        }

        /// <summary>
        /// Ensure that timespan is not null/empty/less than 1
        /// </summary>
        /// <param name="tSpan">Timespan to test</param>
        /// <param name="argName">Name of timespan variable</param>
        public static void ValidateTimeSpan_Pos(TimeSpan tSpan, string argName)
        {
            if (tSpan == null || tSpan.Ticks < 1)
            {
                throw new ArgumentException(argName + " cannot be null or less than 1.");
            }
        }

        /// <summary>
        /// Ensure that timespan is not null/empty/negative
        /// </summary>
        /// <param name="tSpan">Timespan to test</param>
        /// <param name="argName">Name of timespan variable</param>
        public static void ValidateTimeSpan_NonNeg(TimeSpan tSpan, string argName)
        {
            if (tSpan == null || tSpan.Ticks < 0)
            {
                throw new ArgumentException(argName + " cannot be null or negative.");
            }
        }

    }
}
