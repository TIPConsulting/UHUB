using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace UHub
{
    public static class ConfigHelper
    {
        public static T GetWebConfVar<T>(string VarName)
        {
            var conf = WebConfigurationManager.AppSettings[VarName];

            return (T)Convert.ChangeType(conf, typeof(T));
        }

    }
}