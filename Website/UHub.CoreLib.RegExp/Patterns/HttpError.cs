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

        public static class HttpError
        {
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string ERROR_PAGE = @"\.[A-z]+\/Error\/[1-9][0-9]{2}(\?.*)?";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string ERROR_PAGE_B = @"^\.[A-z]+\/Error\/[1-9][0-9]{2}(\?.*)?$";
        }
    }
}
