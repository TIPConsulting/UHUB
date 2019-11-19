using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// TimeSpan extension methods
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// The number of ticks per microsecond.
        /// </summary>
        public const int TicksPerMicrosecond = 10;
        /// <summary>
        /// The number of ticks per Nanosecond.
        /// </summary>
        public const int NanosecondsPerTick = 100;

        /// <summary>
        /// Gets the microsecond fraction of a DateTime.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Microseconds(this TimeSpan self)
        {
            return (int)Math.Floor(
               (self.Ticks
               % TimeSpan.TicksPerMillisecond)
               / (double)TicksPerMicrosecond);
        }
        /// <summary>
        /// Gets the Nanosecond fraction of a DateTime.  Note that the DateTime
        /// object can only store nanoseconds at resolution of 100 nanoseconds.
        /// </summary>
        /// <param name="self">The DateTime object.</param>
        /// <returns>the number of Nanoseconds.</returns>
        public static int Nanoseconds(this TimeSpan self)
        {
            return (int)(self.Ticks % TimeSpan.TicksPerMillisecond % TicksPerMicrosecond)
               * NanosecondsPerTick;
        }

    }
}
