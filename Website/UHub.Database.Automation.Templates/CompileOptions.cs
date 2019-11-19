using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.Database.Automation.Templates
{
    [Flags]
    public enum CompileOptions
    {
        Create = 1,
        Update = 2,
        CreateOrUpdate = Create | Update
    }
}
