using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    /// <summary>
    /// Advanced DateTime with higher resolution
    /// </summary>
    public static class HighResolutionDateTime
    {
        /// <summary>
        /// Determine if <see cref="HighResolutionDateTime"/> is available on the current environment
        /// </summary>
        public static bool IsAvailable { get; private set; }


        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        /// <summary>
        /// Get precise current UTC datetime
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException("High resolution clock isn't available.");
                }

                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);
                return DateTime.FromFileTimeUtc(filetime);
            }
        }

        /// <summary>
        /// Initialize <see cref="HighResolutionDateTime"/>
        /// </summary>
        static HighResolutionDateTime()
        {
            try
            {
                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);
                IsAvailable = true;
            }
            catch (EntryPointNotFoundException)
            {
                // Not running Windows 8 or higher.             
                IsAvailable = false;
            }
        }
    }
}
