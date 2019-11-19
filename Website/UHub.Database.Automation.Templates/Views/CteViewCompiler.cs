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
    internal sealed class CteViewCompiler : ViewCompiler
    {
        private static string InnerTemplate_Set1 = @"
                    --
			        case when epx.PropID = {0}
			        then PropValue
			        else NULL
			        end [{1}]";


        private static string InnerTemplate_ColumnJoin = @"
            {0} {1} {2}
	        on
		        {2}.{3} = {4}.{5}";

        private static string InnerTemplate_CustomJoin = @"
            {0} {1} {2}
	        on
		        {3}";


        private static string GetColumnJoin(string JoinType, string FkTable, string FkAlias, string FkColumn, string PkAlias, string PkColumn)
        {
            string tmp = string.Format(InnerTemplate_ColumnJoin, JoinType, FkTable, FkAlias, FkColumn, PkAlias, PkColumn);
            return tmp;
        }

        private static string GetColumnJoin(string JoinType, string FkTable, string FkAlias, string PkAlias, string CustomComparer)
        {
            string CustomComparerAdj = CustomComparer
                                        .Replace("{FK}", FkAlias)
                                        .Replace("{PK}", PkAlias);


            string tmp = string.Format(InnerTemplate_CustomJoin, JoinType, FkTable, FkAlias, CustomComparerAdj);
            return tmp;
        }



        private static string MasterQueryTemplate = @"
            with set1 as
	        (
		        select 
			        EntID,
                    {TemplateInsert_Set1}

		        from
			        EntPropertyXRef epx
		        where
			        epx.EntTypeID = {TemplateInsert_EntTypeID}
	        ),
	        set2 as
	        (
		        select
			        EntID,
{TemplateInsert_Set2}
		        from set1
		        group by
			        EntID
	        )

	        select
{TemplateInsert_Select}
	        from Entities ent

	        inner join set2 s2
	        on
		        ent.ID = s2.EntID

{TemplateInsert_Joins}

	        where
		        ent.IsDeleted = 0";





        internal CteViewCompiler(SqlConfig DbConfig)
        {
            this.DbConfig = DbConfig;
            this.DBManager = new DBManager(this.DbConfig);
        }





        public override string CompileSelect()
        {
            int maxMarker = 0;
            StringBuilder masterBuilder = new StringBuilder(MasterQueryTemplate);

            masterBuilder.Replace("{TemplateInsert_EntTypeID}", EntTypeID.ToString());




            string templateInsert_Set1;
            List<string> templateExpansion_Set1 = new List<string>();
            EntProperties
                .ForEach(prop =>
                {
                    string tmp = string.Format(InnerTemplate_Set1, prop.ID, prop.PropName);
                    templateExpansion_Set1.Add(tmp);
                });
            templateInsert_Set1 = string.Join(",", templateExpansion_Set1);


            masterBuilder.Replace("{TemplateInsert_Set1}", templateInsert_Set1);




            string templateInsert_Set2;
            List<string> templateExpansion_Set2 = new List<string>();
            EntProperties
                .ForEach(prop =>
                {
                    if (prop.DataType.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string frmt = "			        min([{0}]) |as [{0}]";
                        string tmp = string.Format(frmt, prop.PropName);
                        templateExpansion_Set2.Add(tmp);
                    }
                    else
                    {
                        string frmt = "			        cast(min([{0}]) as {1}) |as [{0}]";
                        string tmp = string.Format(frmt, prop.PropName, prop.DataType);
                        templateExpansion_Set2.Add(tmp);
                    }

                });
            maxMarker = templateExpansion_Set2.Max(x => x.IndexOf('|'));

            for (int i = 0; i < templateExpansion_Set2.Count; i++)
            {
                var marker = templateExpansion_Set2[i].IndexOf('|');
                var internalPad = new string(' ', (maxMarker - marker) + 4);
                templateExpansion_Set2[i] = templateExpansion_Set2[i].Replace("|", internalPad);
            }

            templateInsert_Set2 = string.Join("," + Environment.NewLine, templateExpansion_Set2);
            masterBuilder.Replace("{TemplateInsert_Set2}", templateInsert_Set2);





            int joinCounter = 0;
            string templateInsert_Joins;
            List<string> templateExpansion_Joins = new List<string>();
            List<string> templateExpansion_Select = new List<string>();
            List<string> allColumns = new List<string>();


            DefaultEntColumns
                .ForEach((x) =>
                {
                    allColumns.Add(x);
                    var tmp = $"		        ent.{x} |as [{x}]";
                    templateExpansion_Select.Add(tmp);
                });



            if (HasParentage)
            {
                string fkJoinAlias = "fkJoin_" + joinCounter;
                joinCounter++;

                var tmp = GetColumnJoin(
                            JoinType: "INNER JOIN",
                            FkTable: $"dbo.EntChildXRef",
                            FkAlias: fkJoinAlias,
                            PkAlias: "ent",
                            CustomComparer: "{FK}.ChildEntID = {PK}.ID");

                templateExpansion_Joins.Add(tmp);


                allColumns.Add("ParentID");
                var tmp2 = $"		        {fkJoinAlias}.ParentEntID |as [ParentID]";
                templateExpansion_Select.Add(tmp2);
            }


            EntProperties
                .ForEach(prop =>
                {
                    string frmt = "		        s2.[{0}] |as [{0}]";
                    string tmp = string.Format(frmt, prop.PropName);
                    templateExpansion_Select.Add(tmp);
                });


            EntBreakoutTables
                .ForEach((brkTbl) =>
                {
                    string fkJoinAlias = "fkJoin_" + joinCounter;
                    joinCounter++;

                    var FkSchema = brkTbl.TableSchema;
                    var FkTable = brkTbl.TableName;
                    var PkTable = "Entities";
                    var PkAlias = "ent";

                    var columnWhitelist = brkTbl.ColumnWhiteList?.Split(',').Select(x => x.ToLower().Trim()).ToHashSet() ?? new HashSet<string>();
                    var columnBlacklist = brkTbl.ColumnBlackList?.Split(',').Select(x => x.ToLower().Trim()).ToHashSet() ?? new HashSet<string>();


                    var columns = SqlHelper.GetTableColumns(this.DbConfig, FkSchema, FkTable).ToList();

                    if (columns.Count == 0)
                    {
                        throw new Exception($"Foreign Breakout Table \"{FkSchema}.{FkTable}\" cannot be found");
                    }

                    if (brkTbl.OverrideComparer.IsNotEmpty())
                    {

                        var tmp = GetColumnJoin(
                            JoinType: brkTbl.JoinType,
                            FkTable: $"{FkSchema}.{FkTable}",
                            FkAlias: fkJoinAlias,
                            PkAlias: PkAlias,
                            CustomComparer: brkTbl.OverrideComparer);

                        templateExpansion_Joins.Add(tmp);


                    }
                    else
                    {
                        var relationship = TableRelationships
                                                .Where(x => x.FK_Table == FkTable && x.PK_Table == PkTable)
                                                .Single();


                        columnBlacklist.Add(relationship.FK_Column.ToLower());


                        var tmp = GetColumnJoin(
                                JoinType: brkTbl.JoinType,
                                FkTable: $"{FkSchema}.{FkTable}",
                                FkAlias: fkJoinAlias,
                                FkColumn: relationship.FK_Column,
                                PkAlias: PkAlias,
                                PkColumn: relationship.PK_Column);

                        templateExpansion_Joins.Add(tmp);
                    }


                    columns
                    .ForEach(col =>
                    {
                        if (columnWhitelist.Count != 0)
                        {
                            if (!columnWhitelist.Contains(col.ToLower()))
                            {
                                return;
                            }
                        }
                        if (columnBlacklist.Count != 0)
                        {
                            if (columnBlacklist.Contains(col.ToLower()))
                            {
                                return;
                            }
                        }

                        allColumns.Add(col);
                        var tmp = $"		        {fkJoinAlias}.[{col}] |as [{col}]";
                        templateExpansion_Select.Add(tmp);
                    });

                });


            templateInsert_Joins = string.Join(Environment.NewLine, templateExpansion_Joins);
            masterBuilder.Replace("{TemplateInsert_Joins}", templateInsert_Joins);







            string templateInsert_Select;

            maxMarker = templateExpansion_Select.Max(x => x.IndexOf('|'));

            for (int i = 0; i < templateExpansion_Select.Count; i++)
            {
                var marker = templateExpansion_Select[i].IndexOf('|');
                var internalPad = new string(' ', (maxMarker - marker) + 4);
                templateExpansion_Select[i] = templateExpansion_Select[i].Replace("|", internalPad);
            }


            templateInsert_Select = string.Join("," + Environment.NewLine, templateExpansion_Select);
            masterBuilder.Replace("{TemplateInsert_Select}", templateInsert_Select);



            var allColumnsDistinct = allColumns.Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();

            if (allColumns.Count != allColumnsDistinct.Count)
            {
                throw new Exception($"Duplicate column names exist in view \"{EntType.Name}\"" + Environment.NewLine + allColumns.Duplicates().ToFormattedJSON());
            }



            return masterBuilder.ToString();
        }



    }
}
