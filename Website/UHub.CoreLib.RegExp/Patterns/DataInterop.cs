using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UHub.CoreLib.RegExp.Attributes;

namespace UHub.CoreLib.RegExp
{
    partial class RgxPtrns
    {
        public class DataInterop
        {
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string INVALID_ARGUMENT = @"Invalid [a-zA-Z ]+ argument";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string INVALID_ARGUMENT_B = @"^Invalid [a-zA-Z ]+ argument$";


        }
    }
}
