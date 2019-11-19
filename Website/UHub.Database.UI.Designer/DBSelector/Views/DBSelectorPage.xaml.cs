using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UHub.Database.UI.Designer.Main.Views;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.Database.UI.Designer.DBNavigator.Views;
using UHub.Database.UI.Designer.Main.Models;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UHub.Database.UI.Designer.DBSelector.Models;
using System.Data.SqlClient;

namespace UHub.Database.UI.Designer.DBSelector.Views
{
    /// <summary>
    /// Interaction logic for DBSelectorPage.xaml
    /// </summary>
    public partial class DBSelectorPage : Page
    {
        private SqlConfigBuilder ConnBuilder;

        //private static DependencyProperty DepHelper(Type PropType, [CallerMemberName] string CallerName = "")
        //{
        //    string CallerNameAdj = CallerName.RgxReplace("Property$", "", RegexOptions.IgnoreCase);
        //    return DependencyProperty.Register(CallerNameAdj, PropType, typeof(DBSelectorPage));
        //}
        //public DependencyProperty ServerDbListProperty = DepHelper(typeof(IEnumerable<string>));
        //public IEnumerable<string> ServerDbList
        //{
        //    get { return (IEnumerable<string>)GetValue(ServerDbListProperty); }
        //    set { SetValue(ServerDbListProperty, value); }
        //}


        public DBSelectorModel model { get; set; }


        public DBSelectorPage()
        {
            InitializeComponent();
            DataContext = this;

            model = new DBSelectorModel();

            ConnBuilder = new SqlConfigBuilder()
            {
                Server = model.CurrentSearch,
                EnableIntegratedSecurity = true,
                EnableAsync = true,
                ConnectionTimeout = 5
            };
        }



        private async void btn_DbSearch_Click(object sender, RoutedEventArgs e)
        {
            var server = txt_ServerName.Text;

            if (server.IsEmpty())
            {
                return;
            }

            model.CurrentSearch = server;
            btn_DbSearch.IsEnabled = false;


            ConnBuilder.Server = model.CurrentSearch;
            var pswd = psd_UserPswd.SecurePassword;
            pswd.MakeReadOnly();


            if (chk_EnableSqlAuth.IsChecked == true)
            {
                var username = txt_UserName.Text;
                //Domain account
                if (username.Contains("\\"))
                {
                    var parts = username.Split('\\');
                    SystemStateTracker.ImpersonationContext = ImpersonationContext.CreateContext(parts[0], parts[1], pswd, LogonType.Interactive);
                }
                //SQL account
                else
                {
                    SystemStateTracker.ImpersonationContext = null;

                    ConnBuilder.EnableIntegratedSecurity = false;
                    ConnBuilder.UserCredential = new SqlCredential(txt_UserName.Text, pswd);
                }
            }
            else
            {
                ConnBuilder.EnableIntegratedSecurity = true;
                ConnBuilder.UserCredential = null;
            }


            async Task SearchWorker()
            {
                SqlConfig conn = new SqlConfig(ConnBuilder);
                var result = await SqlHelper.GetDatabaseListAsync(conn);
                result = result.OrderBy(x => x).ToList();
                model.ServerDbList = result;
            }


            try
            {
                await Task.Run(SearchWorker);
            }
            catch (SqlException ex) when (ex.Server.IsEmpty())
            {
                psd_UserPswd.Clear();
                model.ServerDbList = Enumerable.Empty<string>();
                MessageBox.Show("Server not found");
            }
            catch (SqlException ex)
            {
                psd_UserPswd.Clear();
                model.ServerDbList = Enumerable.Empty<string>();
                MessageBox.Show(ex.Message);
            }
            catch
            {
                psd_UserPswd.Clear();
                model.ServerDbList = Enumerable.Empty<string>();
                MessageBox.Show("Unknown Error");
            }
            finally
            {
                btn_DbSearch.IsEnabled = true;
            }

        }




        private void btn_SelectDb_Click(object sender, RoutedEventArgs e)
        {
            if (model.SelectedDb.IsEmpty())
            {
                return;
            }

            ConnBuilder.Database = model.SelectedDb;
            var conn = new SqlConfig(ConnBuilder);

            if (!conn.ValidateConnection())
            {
                MessageBox.Show("Unable to connect to specified DB");
                return;
            }

            SystemStateTracker.SqlConn = conn;
            SystemStateTracker.FrameSource = new DBNavigatorPage();

        }

    }
}
