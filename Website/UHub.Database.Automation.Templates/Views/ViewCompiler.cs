using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.Database.Management;

namespace UHub.Database.Automation.Templates.Views
{
    public abstract class ViewCompiler
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






        public abstract string CompileSelect();


        public virtual string CompileView(CompileOptions CompileOpt)
        {
            string ViewNameFull = $"[{this.EntType.AutoViewSchema}].[{this.EntType.AutoViewName}]";

            StringBuilder builder = new StringBuilder();

            if ((CompileOpt & CompileOptions.Create) != 0)
            {

                if ((CompileOpt & CompileOptions.Update) != 0)
                {
                    var deletePart =
$@"if(OBJECT_ID('{ViewNameFull}', 'V') IS NOT NULL)
begin
    drop view {ViewNameFull}
end
go";

                    builder.Append(deletePart);
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.AppendLine();
                }

                builder.AppendFormat("create view {0}", ViewNameFull);
                builder.AppendLine();
                builder.AppendLine("as");
                builder.AppendLine();
                builder.AppendLine();
                builder.Append(this.CompileSelect());

            }
            else if ((CompileOpt & CompileOptions.Update) != 0)
            {
                builder.AppendFormat("alter view {0}", ViewNameFull);
                builder.AppendLine();
                builder.AppendLine("as");
                builder.AppendLine();
                builder.AppendLine();
                builder.Append(this.CompileSelect());
            }


            return builder.ToString();
        }




        public virtual async Task DeployViewAsync(CompileOptions CompileOpt)
        {
            string ViewNameFull = $"[{this.EntType.AutoViewSchema}].[{this.EntType.AutoViewName}]";

            StringBuilder builder = new StringBuilder();

            if ((CompileOpt & CompileOptions.Create) != 0)
            {

                if ((CompileOpt & CompileOptions.Update) != 0)
                {
                    var deletePart =
$@"if(OBJECT_ID('{ViewNameFull}', 'V') IS NOT NULL)
begin
    drop view {ViewNameFull}
end";
                    await SqlWorker.ExecNonQueryAsync(DbConfig, deletePart, (cmd) =>
                    {
                        cmd.CommandType = CommandType.Text;
                    });
                }


                builder.AppendFormat("create view {0}", ViewNameFull);
                builder.AppendLine();
                builder.AppendLine("as");
                builder.AppendLine();
                builder.Append(this.CompileSelect());


                var createPart = builder.ToString();

                await SqlWorker.ExecNonQueryAsync(DbConfig, createPart, (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                });

            }
            else if ((CompileOpt & CompileOptions.Update) != 0)
            {
                builder.AppendFormat("alter view {0}", ViewNameFull);
                builder.AppendLine();
                builder.AppendLine("as");
                builder.AppendLine();
                builder.Append(this.CompileSelect());

                var updatePart = builder.ToString();

                await SqlWorker.ExecNonQueryAsync(DbConfig, updatePart, (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                });
            }
        }


    }
}
