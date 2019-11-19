using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Security.Authentication
{
    [DataClass]
    public sealed partial class TokenValidator : DBEntityBase
    {

        
        [DataProperty]
        public DateTimeOffset IssueDate { get; private set; }
        [DataProperty]
        public DateTimeOffset MaxExpirationDate { get; private set; }
        [DataProperty]
        public string TokenID { get; private set; }
        [DataProperty]
        public bool IsPersistent { get; private set; }
        [DataProperty]
        public string TokenHash { get; private set; }
        [DataProperty]
        public string RequestID { get; private set; }
        [DataProperty]
        public string SessionID { get; private set; }
        [DataProperty]
        public bool IsValid { get; private set; }
        

    }
}
