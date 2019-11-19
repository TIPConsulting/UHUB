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

        public static class Entity
        {
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string ENT_NAME = @".{2,200}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string ENT_NAME_B = @"^.{2,200}$";
        }
    }
}
