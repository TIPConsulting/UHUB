using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.Database.Management.DataInterop
{
    public sealed class EntTypeReader
    {
        private SqlConfig Config;

        internal EntTypeReader(SqlConfig Config)
        {
            this.Config = Config;
        }


        public IEnumerable<EntType> GetEntTypes()
        {
            return SqlWorker.ExecEntityQuery<EntType>(Config, "[dbo].[_Ent_GetEntTypes]");
        }
        public async Task<IEnumerable<EntType>> GetEntityTypesAsync()
        {
            return await SqlWorker.ExecEntityQueryAsync<EntType>(Config, "[dbo].[_Ent_GetEntTypes]");
        }


        
        public EntType GetEntType(short EntTypeID)
        {
            return SqlWorker.ExecEntityQuery<EntType>(
                Config,
                "[dbo].[_Ent_GetEntTypeByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                }).SingleOrDefault();
        }
        public async Task<EntType> GetEntTypeAsync(short EntTypeID)
        {
            var temp = await SqlWorker.ExecEntityQueryAsync<EntType>(
                Config,
                "[dbo].[_Ent_GetEntTypeByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });

            return temp.SingleOrDefault();
        }






        public IEnumerable<EntTypeBreakoutXRef> GetEntTypeBreakouts()
        {
            return SqlWorker.ExecEntityQuery<EntTypeBreakoutXRef>(Config, "[dbo].[_Ent_GetEntTypeBreakouts]");
        }
        public async Task<IEnumerable<EntTypeBreakoutXRef>> GetEntTypeBreakoutsAsync()
        {
            return await SqlWorker.ExecEntityQueryAsync<EntTypeBreakoutXRef>(Config, "[dbo].[_Ent_GetEntTypeBreakouts]");
        }




        public IEnumerable<EntTypeBreakoutXRef> GetEntTypeBreakouts(short EntTypeID)
        {
            return SqlWorker.ExecEntityQuery<EntTypeBreakoutXRef>(
                Config,
                "[dbo].[_Ent_GetEntTypeBreakoutsByEntType]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });
        }
        public async Task<IEnumerable<EntTypeBreakoutXRef>> GetEntTypeBreakoutsAsync(short EntTypeID)
        {
            return await SqlWorker.ExecEntityQueryAsync<EntTypeBreakoutXRef>(
                Config,
                "[dbo].[_Ent_GetEntTypeBreakoutsByEntType]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@EntTypeID", SqlDbType.SmallInt).Value = EntTypeID;
                });
        }
    }
}
