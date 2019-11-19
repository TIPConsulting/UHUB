using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UHub.Database.Automation.Templates;
using UHub.Database.Automation.Templates.Views;
using UHub.Database.Management;
using UHub.Database.UI.Designer.Main.Models;
using UHub.CoreLib.DataInterop;
using System.Data;
using UHub.Database.Automation.Templates.Sprocs;

namespace UHub.Database.UI.Designer.DBNavigator.Views
{
    /// <summary>
    /// Interaction logic for DBNavigatorPage.xaml
    /// </summary>
    public partial class DBNavigatorPage : Page
    {
        public DBNavigatorPage()
        {
            InitializeComponent();
        }


        
        private async void btn_GenViews_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(GenerateViewWorker);
        }

        private async Task GenerateViewWorker()
        {
            var conn = SystemStateTracker.SqlConn;

            DBManager dbm = new DBManager(conn);

            var entTypes = await dbm.EntTypeReader.GetEntityTypesAsync();


            List<Task<ViewCompiler>> viewCompilerTasks = new List<Task<ViewCompiler>>();

            foreach (var x in entTypes)
            {
                viewCompilerTasks.Add(ViewCompilerFactory.Create(conn, x.ID));
            }

            await Task.WhenAll(viewCompilerTasks);


            Directory.CreateDirectory(".\\Views\\");
            Directory.CreateDirectory(".\\ErrorLogs\\");
            //var t = Parallel.ForEach(viewCompilerTasks, (x) =>
            //{
            //    var tmp = x.Result.Compile();

            //    File.WriteAllText($".\\Views\\{x.Result.EntType.ID}_{x.Result.EntType.Name}", tmp);
            //});

            List<Task> dbDeployTasks = new List<Task>();

            viewCompilerTasks.ForEach(x =>
            {


                /*
                var tmp = x.Result.CompileView(CompileOptions.CreateOrUpdate);
                File.WriteAllText($".\\Views\\{x.Result.EntType.ID}_{x.Result.EntType.Name}", tmp);
                /*/
                dbDeployTasks.Add(x.Result.DeployViewAsync(CompileOptions.CreateOrUpdate));
                //*/

            });


            await Task.WhenAll(dbDeployTasks);


            MessageBox.Show("Task Completed");

        }




        private async void btn_GenRevisionViews_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(GenerateRevisionViewWorker);
        }
        private async Task GenerateRevisionViewWorker()
        {
            var conn = SystemStateTracker.SqlConn;

            DBManager dbm = new DBManager(conn);

            var entTypes = await dbm.EntTypeReader.GetEntityTypesAsync();


            List<Task<SprocCompiler>> viewCompilerTasks = new List<Task<SprocCompiler>>();

            foreach (var x in entTypes)
            {
                viewCompilerTasks.Add(RevisionSprocCompilerFactory.Create(conn, x.ID));
            }

            await Task.WhenAll(viewCompilerTasks);


            Directory.CreateDirectory(".\\Views\\");
            Directory.CreateDirectory(".\\ErrorLogs\\");
            //var t = Parallel.ForEach(viewCompilerTasks, (x) =>
            //{
            //    var tmp = x.Result.Compile();

            //    File.WriteAllText($".\\Views\\{x.Result.EntType.ID}_{x.Result.EntType.Name}", tmp);
            //});

            List<Task> dbDeployTasks = new List<Task>();

            viewCompilerTasks.ForEach(x =>
            {


                /*
                var tmp = x.Result.CompileSproc(CompileOptions.CreateOrUpdate);
                File.WriteAllText($".\\Views\\{x.Result.EntType.ID}_{x.Result.EntType.Name}", tmp);
                /*/
                dbDeployTasks.Add(x.Result.DeploySproc(CompileOptions.CreateOrUpdate));
                //*/

            });


            await Task.WhenAll(dbDeployTasks);


            MessageBox.Show("Task Completed");

        }

    }
}
