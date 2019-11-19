using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace UHub.CoreLib.Tools
{
    public sealed class MachineKeyHelper
    {
        private static byte[] _validationKey = null;
        //private static byte[] _decryptionKey = null;

        private static byte[] GetKey(object provider, string name)
        {
            var validationKey = provider.GetType().GetMethod(name).Invoke(provider, new object[0]);
            return (byte[])validationKey.GetType().GetMethod("GetKeyMaterial").Invoke(validationKey, new object[0]);
        }

        public static byte[] GetValidationKey()
        {
            if (_validationKey == null)
            {
                var machineKey = typeof(MachineKeySection).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(a => a.Name == "GetApplicationConfig").Invoke(null, new object[0]);

                var type = Assembly.Load("System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").GetTypes().Single(a => a.Name == "MachineKeyMasterKeyProvider");

                var instance = type.Assembly.CreateInstance(
                    type.FullName, false,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null, new object[] { machineKey, null, null, null, null }, null, null);

                var validationKey = type.GetMethod("GetValidationKey").Invoke(instance, new object[0]);
                var key = (byte[])validationKey.GetType().GetMethod("GetKeyMaterial").Invoke(validationKey, new object[0]);


                _validationKey = GetKey(instance, "GetValidationKey");
                //_decryptionKey = GetKey(instance, "GetEncryptionKey");
            }

            //return Encoding.Unicode.GetString(_validationKey);
            return _validationKey;
        }
    }
}
