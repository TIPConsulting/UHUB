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

        public static class FileUpload
        {
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string CHUNK_ID = @"[0-9a-f]{12}\-([0-9]{1,20})\-([0-9]{1,20})";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string CHUNK_ID_B = @"^[0-9a-f]{12}\-([0-9]{1,20})\-([0-9]{1,20})$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string FILE_NAME = @"[\w\-. ]{1,200}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string FILE_NAME_B = @"^[\w\-. ]{1,200}$";
        }
    }
}
