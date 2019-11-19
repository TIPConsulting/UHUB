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

        public static class EntSchoolClub
        {
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string NAME = @"(([ \u00c0-\u01ffA-z0-9'\-])+){3,100}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string NAME_B = @"^(([ \u00c0-\u01ffA-z0-9'\-])+){3,100}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Multiline | RegexOptions.IgnoreCase))]
            public const string DESCRIPTION = @".{1,2000}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Multiline | RegexOptions.IgnoreCase))]
            public const string DESCRIPTION_B = @"^.{1,2000}$";
        }
    }
}
