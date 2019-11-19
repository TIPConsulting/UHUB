using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.Database.Management.DataInterop;

namespace UHub.Database.Management
{
    public sealed class DBManager
    {

        public EntChildMapReader ChildMapReader { get; }
        public EntDataTypeReader DataTypeReader { get; }
        public EntPropMapReader PropMapReader { get; }
        public EntPropReader PropReader { get; }
        public EntPropRevisionMapReader PropRevReader { get; }
        public EntTypeReader EntTypeReader { get; }



        public DBManager(SqlConfig Config)
        {
            ChildMapReader = new EntChildMapReader(Config);
            DataTypeReader = new EntDataTypeReader(Config);
            PropMapReader = new EntPropMapReader(Config);
            PropReader = new EntPropReader(Config);
            PropRevReader = new EntPropRevisionMapReader(Config);
            EntTypeReader = new EntTypeReader(Config);

        }



    }
}
