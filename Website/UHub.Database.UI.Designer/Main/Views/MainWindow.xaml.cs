using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
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
using UHub.CoreLib.DataInterop;
using UHub.Database.UI.Designer.DBSelector.Views;
using UHub.Database.UI.Designer.Main.Models;

namespace UHub.Database.UI.Designer.Main.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public DependencyProperty FrameSourceProperty = DependencyProperty.Register("FrameSource", typeof(object), typeof(MainWindow));
        //public object FrameSource
        //{
        //    get { return (string)GetValue(FrameSourceProperty); }
        //    set { SetValue(FrameSourceProperty, value); }
        //}




        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        private void Window_Initialized(object sender, EventArgs e)
        {
            SystemStateTracker.FrameSource = new DBSelectorPage();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SystemStateTracker.ImpersonationContext?.Dispose();
        }
    }
}
