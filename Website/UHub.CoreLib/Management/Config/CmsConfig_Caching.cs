using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Management.Config
{
    public sealed class CmsConfig_Caching
    {
        /// <summary>
        /// Populate caches (where available) on CMS instantiation
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool EnableStartupCachePopulation { get; set; } = true;
        /// <summary>
        /// Enable content caching for client navigation bar
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableNavBarCaching { get; set; } = false;
        /// <summary>
        /// Enable caching for content pages that are read from the database
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableDBPageCaching { get; set; } = false;
        /// <summary>
        /// Lifespan for database entity caches
        /// Each entity stored in cache will be expired and refetched after the lifespan expires
        /// <para></para>
        /// Default: 6 hours
        /// </summary>
        public TimeSpan MaxDBCacheAge { get; set; } = new TimeSpan(0, 6, 0, 0);
        /// <summary>
        /// Enable IIS dynamic page caching to reduce processing time
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool EnableIISPageCaching { get; set; } = false;
        /// <summary>
        /// Lifespan for IIS dynamic page caches
        /// <para></para>
        /// Default: 6 hours
        /// </summary>
        public TimeSpan MaxDynamicCacheAge { get; set; } = new TimeSpan(0, 6, 0, 0);
        /// <summary>
        /// Lifespan for IIS static page caches
        /// <para></para>
        /// Default: 1 day
        /// </summary>
        public TimeSpan MaxStaticCacheAge { get; set; } = new TimeSpan(1, 0, 0, 0);
    }
}
