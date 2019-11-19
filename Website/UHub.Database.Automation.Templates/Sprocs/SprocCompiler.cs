using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.Database.Management;

namespace UHub.Database.Automation.Templates.Sprocs
{
    public abstract class SprocCompiler
    {
        private protected DBManager DBManager { get; set; }
        private protected SqlConfig DbConfig { get; set; }



        public EntType EntType { get; internal set; }
        public short EntTypeID { get; internal set; }
        public bool HasParentage { get; internal set; }
        internal List<EntProperty> EntProperties = new List<EntProperty>();
        internal List<EntTypeBreakoutXRef> EntBreakoutTables = new List<EntTypeBreakoutXRef>();
        internal List<TableRelationshipInfo> TableRelationships = new List<TableRelationshipInfo>();


        internal static readonly List<string> DefaultEntColumns = new List<string>
            {
                "ID",
                "EntTypeID",
                "IsEnabled",
                "IsReadonly",
                "IsDeleted",
                "CreatedBy",
                "CreatedDate",
                "ModifiedBy",
                "ModifiedDate",
                "DeletedBy",
                "DeletedDate"
            };



        internal async Task Initialize(short EntTypeID)
        {
            var taskEntType = DBManager.EntTypeReader.GetEntTypeAsync(EntTypeID);
            var taskChildMap = DBManager.ChildMapReader.GetMapsAsync();
            var taskEntProps = DBManager.PropReader.GetPropertiesAsync(EntTypeID);
            var taskEntTypeBreakout = DBManager.EntTypeReader.GetEntTypeBreakoutsAsync(EntTypeID);
            var taskTableRelationships = SqlHelper.GetTableRelationshipsAsync(DbConfig);

            await Task.WhenAll(taskEntType, taskChildMap, taskEntProps, taskEntTypeBreakout, taskTableRelationships);



            this.EntType = taskEntType.Result;
            this.EntTypeID = EntTypeID;
            this.HasParentage = taskChildMap.Result.Any(x => x.ChildEntType == EntTypeID);


            this.EntProperties = taskEntProps.Result.OrderBy(x => x.ID).ToList();


            this.EntBreakoutTables = taskEntTypeBreakout.Result.ToList();
            this.TableRelationships = taskTableRelationships.Result.ToList();


        }


        public abstract string GetSprocName();
        public abstract string CompileSproc(CompileOptions CompileOpt);



        public virtual async Task DeploySproc(CompileOptions CompileOpt)
        {
            var sproc = CompileSproc(CompileOpt);


            string SprocNameFull = GetSprocName();



            if (CompileOpt == CompileOptions.CreateOrUpdate)
            {
                var deletePart =
$@"if(OBJECT_ID('{SprocNameFull}', 'P') IS NOT NULL)
begin
    drop proc {SprocNameFull}
end";
                await SqlWorker.ExecNonQueryAsync(DbConfig, deletePart, (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                });
            }



            await SqlWorker.ExecNonQueryAsync(DbConfig, sproc, (cmd) =>
            {
                cmd.CommandType = CommandType.Text;
            });

        }

    }
}
