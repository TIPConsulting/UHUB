using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    /// <summary>
    /// Attempt to get HighResolutionDateTime with DateTime as a falback option
    /// </summary>
    public static class FailoverDateTimeOffset
    {
        private static readonly bool isHighRes;

        static FailoverDateTimeOffset()
        {
            isHighRes = HighResolutionDateTime.IsAvailable;
        }

        public static DateTimeOffset UtcNow
        {
            get
            {
                if (isHighRes)
                {
                    return HighResolutionDateTime.UtcNow;
                }
                else
                {
                    return DateTime.UtcNow;
                }
            }
        }

    }
}
