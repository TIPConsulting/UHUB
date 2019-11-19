using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging
{
    /// <summary>
    /// Location lookup methods
    /// </summary>
    enum LocationLogType
    {
        /// <summary>
        /// Location sourced from IP lookup service
        /// </summary>
        IPLookup = 1,
        /// <summary>
        /// Location sourced from HTML5 GeoLocation API
        /// </summary>
        HtmlGeoAPI = 2
    }
}
