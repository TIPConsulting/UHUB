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

        public static class EntPost
        {
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string NAME = @".{1,100}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string NAME_B = @"^.{1,100}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Multiline | RegexOptions.IgnoreCase))]
            public const string CONTENT = @".{10,10000}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Multiline | RegexOptions.IgnoreCase))]
            public const string CONTENT_B = @"^.{10,10000}$";


        }
    }
}
