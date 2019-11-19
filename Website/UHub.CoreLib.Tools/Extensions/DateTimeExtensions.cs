using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// DateTime extension methods
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Get the start of the month for any date
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        /// <summary>
        /// Get the end of the month for any date
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        }

        /// <summary>
        /// Get the first day of the week for any date
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="weekStart"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek weekStart = DayOfWeek.Sunday)
        {
            int diff = dt.DayOfWeek - weekStart;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Get the beginning of any date (truncate to date portion)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime dt)
        {
            return dt.Date;
        }

        /// <summary>
        /// Get the end of any date (last possible point before midnight)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime dt)
        {
            return dt.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Format as "yyyy-MM-dd"
        /// </summary>
        public static string ISO8601(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Format as "yyyy-MM-dd'T'HH:mm:ss"
        /// </summary>
        public static string ISO8601_T(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd'T'HH:mm:ss");
        }

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
        public static int Microseconds(this DateTime self)
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
        public static int Nanoseconds(this DateTime self)
        {
            return (int)(self.Ticks % TimeSpan.TicksPerMillisecond % TicksPerMicrosecond)
               * NanosecondsPerTick;
        }

        /// <summary>
        /// Adds a number of microseconds to this DateTime object.
        /// </summary>
        /// <param name="self">The DateTime object.</param>
        /// <param name="microseconds">The number of milliseconds to add.</param>
        public static DateTime AddMicroseconds(this DateTime self, int microseconds)
        {
            return self.AddTicks(microseconds * TicksPerMicrosecond);
        }

        /// <summary>
        /// Adds a number of nanoseconds to this DateTime object.  Note: this
        /// object only stores nanoseconds of resolutions of 100 seconds.
        /// Any nanoseconds passed in lower than that will be rounded using
        /// the default rounding algorithm in Math.Round().
        /// </summary>
        /// <param name="self">The DateTime object.</param>
        /// <param name="nanoseconds">The number of nanoseconds to add.</param>
        public static DateTime AddNanoseconds(this DateTime self, int nanoseconds)
        {
            return self.AddTicks((int)Math.Round(nanoseconds / (double)NanosecondsPerTick));
        }

    }
}
