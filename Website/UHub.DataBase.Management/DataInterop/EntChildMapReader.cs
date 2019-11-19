using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.Database.Management.DataInterop
{
    public sealed class EntChildMapReader
    {
        private SqlConfig Config;

        internal EntChildMapReader(SqlConfig Config)
        {
            this.Config = Config;
        }


        public IEnumerable<EntChildMap> GetMaps()
        {
            return SqlWorker.ExecEntityQuery<EntChildMap>(Config, "[dbo].[_Ent_GetEntChildMaps]");
        }

        public async Task<IEnumerable<EntChildMap>> GetMapsAsync()
        {
            return await SqlWorker.ExecEntityQueryAsync<EntChildMap>(Config, "[dbo].[_Ent_GetEntChildMaps]");
        }


    }
}
