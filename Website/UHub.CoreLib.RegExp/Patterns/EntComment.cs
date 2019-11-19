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

        public static class EntComment
        {
            [RegexOptionFlag(Value = (int)(RegexOptions.Multiline | RegexOptions.IgnoreCase))]
            public const string CONTENT = ".{1,1000}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Multiline | RegexOptions.IgnoreCase))]
            public const string CONTENT_B = "^.{1,1000}$";

        }
    }
}
