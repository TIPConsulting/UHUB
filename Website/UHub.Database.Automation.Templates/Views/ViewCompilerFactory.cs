using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.Database.Management;

namespace UHub.Database.Automation.Templates.Views
{
    public class ViewCompilerFactory
    {

        public static async Task<ViewCompiler> Create(SqlConfig DbConfig, short EntTypeID)
        {
            ViewCompiler compiler;

            var DBManager = new DBManager(DbConfig);
            var taskEntProps = DBManager.PropReader.GetPropertiesAsync(EntTypeID);


            if (taskEntProps.Result.Count() > 3)
            {
                compiler = new CteViewCompiler(DbConfig);
            }
            else
            {
                compiler = new SelfJoinViewCompiler(DbConfig);
            }


            await compiler.Initialize(EntTypeID);

            return compiler;
        }

    }
}
