using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Logging.Management
{

    public static class LoggingFactory
    {
        static object singletonLock = new object();
        static bool isInstantiated = false;
        static LoggingManager _singleton = null;



        public static void Initialize(LoggingConfig Config)
        {
            lock (singletonLock)
            {
                if (isInstantiated)
                {
                    throw new InvalidOperationException("LoggingManager already initialized");
                }


                _singleton = new LoggingManager(Config);
                isInstantiated = true;
            }
        }


        /// <summary>
        /// Gets singleton instance if initialized, otherwise returns null
        /// </summary>
        public static LoggingManager Singleton
        {
            get
            {
                if (isInstantiated)
                {
                    return _singleton;
                }

                return null;
            }
        }


        public static bool IsInitialized()
        {
            return isInstantiated;
        }

    }
}
