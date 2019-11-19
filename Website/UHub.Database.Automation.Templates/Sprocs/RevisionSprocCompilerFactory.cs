using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.Database.Automation.Templates.Sprocs
{
    public sealed class RevisionSprocCompilerFactory
    {
        public static async Task<SprocCompiler> Create(SqlConfig DbConfig, short EntTypeID)
        {
            SprocCompiler compiler = new RevisionSprocCompiler(DbConfig);

            await compiler.Initialize(EntTypeID);

            return compiler;
        }
    }
}
