using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.Database.UI.Designer.Main.Models
{
    public static class SystemStateTracker
    {

        public static event EventHandler FrameSourceChanged = delegate { };
        private static object _frameSource = null;
        public static object FrameSource
        {
            get
            {
                return _frameSource;
            }
            set
            {
                _frameSource = value;
                FrameSourceChanged?.Invoke(null, EventArgs.Empty);
            }
        }


        public static event EventHandler SqlConnChanged = delegate { };
        private static SqlConfig _sqlConn = null;
        public static SqlConfig SqlConn
        {
            get
            {
                return _sqlConn;
            }
            set
            {
                _sqlConn = value;
                SqlConnChanged?.Invoke(null, EventArgs.Empty);
            }
        }


        private static ImpersonationContext _impersonationContext = null;
        public static ImpersonationContext ImpersonationContext
        {
            get => _impersonationContext;
            set
            {
                //dispose old context to ensure logout when user is changed
                _impersonationContext?.Dispose();
                _impersonationContext = value;
            }
        }
    }
}
