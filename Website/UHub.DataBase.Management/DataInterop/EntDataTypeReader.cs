using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.Database.Management.DataInterop
{
    public sealed class EntDataTypeReader
    {
        private SqlConfig Config;

        internal EntDataTypeReader(SqlConfig Config)
        {
            this.Config = Config;
        }


        public IEnumerable<EntDataType> GetDataTypes()
        {
            return SqlWorker.ExecEntityQuery<EntDataType>(Config, "[dbo].[_Ent_GetDataTypes]");
        }

        public async Task<IEnumerable<EntDataType>> GetDataTypesAsync()
        {
            return await SqlWorker.ExecEntityQueryAsync<EntDataType>(Config, "[dbo].[_Ent_GetDataTypes]");
        }

    }
}
