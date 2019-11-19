using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management.Config;
using UHub.CoreLib.Security.Accounts;

namespace UHub.CoreLib.Management
{
    public static class CoreFactory
    {

        static object singletonLock = new object();

        private static bool isInstantiated = false;
        private static CoreManager _singleton;



        public static bool Initialize(CmsConfiguration_Grouped config)
        {
            lock (singletonLock)
            {

                if (isInstantiated)
                {
                    throw new InvalidOperationException("CoreManager already initialized");
                }

                if(!config.Validate())
                {
                    throw new ArgumentException("Invalid Configuration");
                }

                _singleton = new CoreManager(config);
                isInstantiated = true;
                return true;
            }
        }


        public static CoreManager Singleton
        {
            get
            {
                if (isInstantiated)
                {
                    return _singleton;
                }

                throw new InvalidOperationException("CoreManager not initialized");
            }
        }


        public static bool IsInitialized()
        {
            return isInstantiated;
        }



    }
}
