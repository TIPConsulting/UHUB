using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.Database.Management.DataInterop
{
    public sealed class EntPropMapReader
    {
        private SqlConfig Config;

        internal EntPropMapReader(SqlConfig Config)
        {
            this.Config = Config;
        }


        public IEnumerable<EntPropertyMap> GetMaps()
        {
            return SqlWorker.ExecEntityQuery<EntPropertyMap>(Config, "[dbo].[_Ent_GetEntPropertyMaps]");
        }

        public async Task<IEnumerable<EntPropertyMap>> GetMapsAsync()
        {
            return await SqlWorker.ExecEntityQueryAsync<EntPropertyMap>(Config, "[dbo].[_Ent_GetEntPropertyMaps]");
        }





        public IEnumerable<EntPropertyMap> GetMaps(short EntTypeID)
        {
            return SqlWorker.ExecEntityQuery<EntPropertyMap>(
                Config,
                "[dbo].[_Ent_GetEntPropertyMapsByEntType]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });
        }

        public async Task<IEnumerable<EntPropertyMap>> GetMapsAsync(short EntTypeID)
        {
            return await SqlWorker.ExecEntityQueryAsync<EntPropertyMap>(
                Config,
                "[dbo].[_Ent_GetEntPropertyMapsByEntType]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });
        }

    }
}
