using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Security.Accounts;

namespace UHub.CoreLib.Entities.Users.Interfaces
{
    /// <summary>
    /// CMS user account recovery context
    /// </summary>
    public partial interface IUserRecoveryContext
    {
        
        /// <summary>
        /// User int ID
        /// </summary>
        [DataProperty]
        long UserID { get; set; }
        /// <summary>
        /// User recovery context ID
        /// </summary>
        [DataProperty]
        string RecoveryID { get; set; }
        /// <summary>
        /// User recovery key (temp password needed to update password)
        /// </summary>
        [DataProperty]
        string RecoveryKey { get; set; }
        /// <summary>
        /// Recovery period eff start date/time
        /// </summary>
        [DataProperty]
        DateTimeOffset EffFromDate { get; set; }
        /// <summary>
        /// Recovery period experiation date/time
        /// </summary>
        [DataProperty]
        DateTimeOffset EffToDate { get; set; }
        /// <summary>
        /// Is recovery context enabled (can user use this context to recover their account)
        /// </summary>
        [DataProperty]
        bool IsEnabled { get; set; }
        /// <summary>
        /// Recovery context attempt count. Context can be invalidated after a specified number of failed attempts (failed: bad recovery key)
        /// </summary>
        [DataProperty]
        byte AttemptCount { get; set; }
        /// <summary>
        /// Flag to mark the recovery as mandatory or optional
        /// </summary>
        [DataProperty]
        bool IsOptional { get; set; }
        
        

    }
}
