using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security
{
    [Flags]
    public enum HtmlSanitizerMode : byte
    {
        Off = 0,
        OnWrite = 1,
        OnRead = 2,
        Both = OnWrite | OnRead
    }
}
