using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.Database.Management.DataInterop
{
    public sealed class EntPropRevisionMapReader
    {
        private SqlConfig Config;

        internal EntPropRevisionMapReader(SqlConfig Config)
        {
            this.Config = Config;
        }


        public IEnumerable<EntPropertyRevisionMap> GetMaps()
        {
            return SqlWorker.ExecEntityQuery<EntPropertyRevisionMap>(Config, "[dbo].[_Ent_GetEntPropertyRevisionMaps]");
        }

        public async Task<IEnumerable<EntPropertyRevisionMap>> GetMapsAsync()
        {
            return await SqlWorker.ExecEntityQueryAsync<EntPropertyRevisionMap>(Config, "[dbo].[_Ent_GetEntPropertyRevisionMaps]");
        }











        public IEnumerable<EntPropertyRevisionMap> GetMaps(short EntTypeID)
        {
            return SqlWorker.ExecEntityQuery<EntPropertyRevisionMap>(
                Config,
                "[dbo].[_Ent_GetEntPropertyRevisionMapsByEntType]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });
        }

        public async Task<IEnumerable<EntPropertyRevisionMap>> GetMapsAsync(short EntTypeID)
        {
            return await SqlWorker.ExecEntityQueryAsync<EntPropertyRevisionMap>(
                Config,
                "[dbo].[_Ent_GetEntPropertyRevisionMapsByEntType]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });
        }
    }
}
