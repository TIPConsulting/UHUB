using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.Database.Management.DataInterop
{
    public sealed class EntPropReader
    {
        private SqlConfig Config { get; }

        internal EntPropReader(SqlConfig Config)
        {
            this.Config = Config;
        }


        public IEnumerable<EntProperty> GetProperties()
        {
            return SqlWorker.ExecEntityQuery<EntProperty>(Config, "[dbo].[_Ent_GetEntProperties]");
        }

        public async Task<IEnumerable<EntProperty>> GetPropertiesAsync()
        {
            return await SqlWorker.ExecEntityQueryAsync<EntProperty>(Config, "[dbo].[_Ent_GetEntProperties]");
        }






        public IEnumerable<EntProperty> GetProperties(short EntTypeID)
        {
            return SqlWorker.ExecEntityQuery<EntProperty>(Config,
                "[dbo].[_Ent_GetEntPropertiesByEntType]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });
        }

        public async Task<IEnumerable<EntProperty>> GetPropertiesAsync(short EntTypeID)
        {
            return await SqlWorker.ExecEntityQueryAsync<EntProperty>(Config,
                "[dbo].[_Ent_GetEntPropertiesByEntType]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });
        }






        public EntProperty GetProperty(int PropID)
        {
            return SqlWorker.ExecEntityQuery<EntProperty>(
                Config,
                "[dbo].[_Ent_GetEntPropertyByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@PropID", SqlDbType.Int).Value = PropID;
                }).SingleOrDefault();
        }

        public async Task<EntProperty> GetPropertyAsync(int PropID)
        {
            var temp = await SqlWorker.ExecEntityQueryAsync<EntProperty>(
                Config,
                "[dbo].[_Ent_GetEntPropertyByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@PropID", SqlDbType.Int).Value = PropID;
                });

            return temp.SingleOrDefault();
        }

    }
}
