using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// DateTimeOffset extension methods
    /// </summary>
    public static class DateTimeOffsetExtensions
    {

        /// <summary>
        /// Get the beginning of any date (truncate to date portion)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTimeOffset StartOfDay(this DateTimeOffset dt)
        {
            return new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, dt.Offset);
        }

        /// <summary>
        /// Get the end of any date (last possible point before midnight)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTimeOffset EndOfDay(this DateTimeOffset dt)
        {
            return new DateTimeOffset(dt.Date.AddDays(1).AddTicks(-1), dt.Offset);
        }

        /// <summary>
        /// Format as "yyyy-MM-dd"
        /// </summary>
        public static string ISO8601(this DateTimeOffset dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Format as "yyyy-MM-dd'T'HH:mm:ss"
        /// </summary>
        public static string ISO8601_T(this DateTimeOffset dt)
        {
            return dt.ToString("yyyy-MM-dd'T'HH:mm:ss");
        }

        /// <summary>
        /// Format as "o"
        /// </summary>
        public static string ISO8601_Full(this DateTimeOffset dt)
        {
            return dt.ToString("o");
        }

    }
}
