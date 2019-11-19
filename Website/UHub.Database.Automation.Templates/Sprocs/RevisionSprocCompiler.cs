using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.Database.Management;

namespace UHub.Database.Automation.Templates.Sprocs
{
    internal sealed class RevisionSprocCompiler : SprocCompiler
    {
        private static string InnerTemplate_Set1 = @"
                --
			    case when eprx.PropID = {0}
			    then PropValue
			    else NULL
			    end [{1}],";


        private static string InnerTemplate_ColumnJoin = @"
        {0} {1} {2}
	    on
		    {2}.{3} = @{4}";

        private static string InnerTemplate_CustomJoin = @"
        {0} {1} {2}
	    on
		    {3}";


        private static string GetColumnJoin(string JoinType, string FkTable, string FkAlias, string FkColumn, string PkColumn)
        {
            string tmp = string.Format(InnerTemplate_ColumnJoin, JoinType, FkTable, FkAlias, FkColumn, PkColumn);
            return tmp;
        }

        private static string GetColumnJoin(string JoinType, string FkTable, string FkAlias, string CustomComparer)
        {
            string CustomComparerAdj = CustomComparer
                                        .Replace("{FK}", FkAlias)
                                        .Replace("{PK}.", "@");


            string tmp = string.Format(InnerTemplate_CustomJoin, JoinType, FkTable, FkAlias, CustomComparerAdj);
            return tmp;
        }


        private static string InnerTemplate_ParentBlock =
@"
	declare @parentID bigint
	select
	    @parentID = ParentEntID
	from EntChildXRef
	where
		ChildEntID = @EntID;";




        private static string MasterQueryTemplate = @"

{InnerTemplate_ProcMode} proc {InnerTemplate_ProcName}

    @EntID bigint

as
begin

	declare @EntTypeID smallint = {TemplateInsert_EntTypeID}

	declare @ID bigint
	declare @IsEnabled bit
	declare @IsReadOnly bit
	declare @IsDeleted bit
	declare @deletedBy bigint
	declare @deletedDate datetimeoffset(7)
	declare @CreatedBy bigint
	declare @CreatedDate datetimeoffset(7)
	declare @ModifiedDate datetimeoffset(7)
	select
		@ID = ID,
		@IsEnabled = IsEnabled,
		@IsReadOnly = IsReadOnly,
		@IsDeleted = IsDeleted,
		@deletedBy = DeletedBy,
		@deletedDate = DeletedDate,
		@CreatedBy = CreatedBy,
		@CreatedDate = CreatedDate,
		@ModifiedDate = ModifiedDate
	from dbo.Entities
	where
		ID = @EntID;

{TemplateInsert_ParentBlock}



	with DynamicSet1 as
	(
		select 
			EntID,
{TemplateInsert_Set1}
			--
			CreatedBy,
			CreatedDate
		from
			EntPropertyRevisionXRef eprx
		where
			eprx.EntID = @EntID
			and eprx.EntTypeID = @EntTypeID
			and (CreatedDate != @ModifiedDate or @IsDeleted = 1)
	),
	DynamicSet2 as
	(
		select
			EntID,
{TemplateInsert_Set2}

		from DynamicSet1
		group by
			EntID,
			CreatedDate
	),	
	RecurseSet as
	(
		select
			prs.EntID,
{TemplateInsert_RS1}
			prs.ModifiedBy,
			prs.ModifiedDate,
			prs.RowNum
		from DynamicSet2 prs
		where
			RowNum = 1


		UNION ALL


		select
			rs.EntID,
{TemplateInsert_RS2}
			
		from DynamicSet2 prs

		inner join RecurseSet rs
		on 
			prs.EntID = rs.EntID
			and prs.RowNum = rs.RowNum + 1

	)


	select
{TemplateInsert_ViewSelect}
    from {TemplateInsert_ViewSelectName}
	where 
		ID = @EntID

	UNION ALL


	select
		@ID			    as ID,
		@EntTypeID		as EntTypeID,
		@IsEnabled		as IsEnabled,
		@IsReadOnly		as IsReadOnly,
		@IsDeleted		as IsDeleted,
		@CreatedBy		as CreatedBy,
		@CreatedDate	as CreatedDate,
		rs.ModifiedBy   as ModifiedBy,
		rs.ModifiedDate as ModifiedDate,
		@deletedBy		as DeletedBy,
		@deletedDate	as DeletedDate,
{TemplateInsert_RsOuterSelect}

	from RecurseSet rs

{TemplateInsert_Joins}

end

";



        internal RevisionSprocCompiler(SqlConfig DbConfig)
        {
            this.DbConfig = DbConfig;
            this.DBManager = new DBManager(this.DbConfig);
        }


        public override string GetSprocName()
        {
            return $"[{this.EntType.AutoViewSchema}].[{this.EntType.Name}_GetRevisions]";
        }

        public override string CompileSproc(CompileOptions CompileOpt)
        {
            int maxMarker = 0;
            StringBuilder masterBuilder = new StringBuilder(MasterQueryTemplate);


            if ((CompileOpt & CompileOptions.Create) != 0)
            {
                masterBuilder.Replace("{InnerTemplate_ProcMode}", "create");
            }
            else if ((CompileOpt & CompileOptions.Update) != 0)
            {
                masterBuilder.Replace("{InnerTemplate_ProcMode}", "alter");
            }

            masterBuilder.Replace("{InnerTemplate_ProcName}", GetSprocName());
            masterBuilder.Replace("{TemplateInsert_EntTypeID}", EntTypeID.ToString());


            masterBuilder.Replace("{TemplateInsert_ViewSelectName}", $"[{this.EntType.AutoViewSchema}].[{this.EntType.AutoViewName}]");



            string templateInsert_Set1;
            List<string> templateExpansion_Set1 = new List<string>();
            EntProperties
                .ForEach(prop =>
                {
                    string tmp = string.Format(InnerTemplate_Set1, prop.ID, prop.PropName);
                    templateExpansion_Set1.Add(tmp);
                });
            templateInsert_Set1 = string.Join(Environment.NewLine, templateExpansion_Set1);

            masterBuilder.Replace("{TemplateInsert_Set1}", templateInsert_Set1);





            string templateInsert_Set2;
            List<string> templateExpansion_Set2 = new List<string>();
            EntProperties
                .ForEach(prop =>
                {
                    if (prop.DataType.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string frmt = "			    min([{0}]) |as [{0}]";
                        string tmp = string.Format(frmt, prop.PropName);
                        templateExpansion_Set2.Add(tmp);
                    }
                    else
                    {
                        string frmt = "			    cast(min([{0}]) as {1}) |as [{0}]";
                        string tmp = string.Format(frmt, prop.PropName, prop.DataType);
                        templateExpansion_Set2.Add(tmp);
                    }

                });
            templateExpansion_Set2.Add("			    min(CreatedBy) |as ModifiedBy");
            templateExpansion_Set2.Add("			    min(CreatedDate) |as ModifiedDate");
            templateExpansion_Set2.Add("			    ROW_NUMBER() over (order by CreatedDate) |as RowNum");


            maxMarker = templateExpansion_Set2.Max(x => x.IndexOf('|'));

            for (int i = 0; i < templateExpansion_Set2.Count; i++)
            {
                var marker = templateExpansion_Set2[i].IndexOf('|');
                var internalPad = new string(' ', (maxMarker - marker) + 4);
                templateExpansion_Set2[i] = templateExpansion_Set2[i].Replace("|", internalPad);
            }

            templateInsert_Set2 = string.Join("," + Environment.NewLine, templateExpansion_Set2);
            masterBuilder.Replace("{TemplateInsert_Set2}", templateInsert_Set2);





            string templateInsert_RS1;
            List<string> templateExpansion_RS1 = new List<string>();
            EntProperties
                .ForEach(prop =>
                {
                    string frmt = "			    prs.[{0}],";
                    string tmp = string.Format(frmt, prop.PropName);
                    templateExpansion_RS1.Add(tmp);

                });
            templateInsert_RS1 = string.Join(Environment.NewLine, templateExpansion_RS1);
            masterBuilder.Replace("{TemplateInsert_RS1}", templateInsert_RS1);





            string templateInsert_RS2;
            List<string> templateExpansion_RS2 = new List<string>();
            EntProperties
                .ForEach(prop =>
                {
                    string frmt = "			    coalesce(prs.[{0}], rs.[{0}]) |as [{0}]";
                    string tmp = string.Format(frmt, prop.PropName);
                    templateExpansion_RS2.Add(tmp);

                });
            templateExpansion_RS2.Add("			    coalesce(prs.ModifiedBy, rs.ModifiedBy) |as ModifiedBy");
            templateExpansion_RS2.Add("			    coalesce(prs.ModifiedDate, rs.ModifiedDate)	|as ModifiedDate");
            templateExpansion_RS2.Add("			    prs.RowNum |as RowNum");


            maxMarker = templateExpansion_RS2.Max(x => x.IndexOf('|'));

            for (int i = 0; i < templateExpansion_RS2.Count; i++)
            {
                var marker = templateExpansion_RS2[i].IndexOf('|');
                var internalPad = new string(' ', (maxMarker - marker) + 4);
                templateExpansion_RS2[i] = templateExpansion_RS2[i].Replace("|", internalPad);
            }

            templateInsert_RS2 = string.Join("," + Environment.NewLine, templateExpansion_RS2);
            masterBuilder.Replace("{TemplateInsert_RS2}", templateInsert_RS2);










            ///////////////////////////
            int joinCounter = 0;
            string templateInsert_Joins;
            List<string> templateExpansion_ViewSelect = new List<string>();
            List<string> templateExpansion_RsOuterSelect = new List<string>();
            List<string> templateExpansion_Joins = new List<string>();





            if (HasParentage)
            {
                masterBuilder.Replace("{TemplateInsert_ParentBlock}", InnerTemplate_ParentBlock);
                var parentProp = "		    @parentID		as ParentID";

                templateExpansion_RsOuterSelect.Add(parentProp);
            }
            else
            {
                masterBuilder.Replace("{TemplateInsert_ParentBlock}", "");
            }



            var viewColumns = SqlHelper.GetTableColumnInfo(DbConfig, EntType.AutoViewSchema, EntType.AutoViewName)
                                .OrderBy(x => x.OrdinalPosition)
                                .ToList();


            string templateInsert_ViewSelect;
            viewColumns
                .ForEach(prop =>
                {
                    string frmt = "			[{0}]";
                    string tmp = string.Format(frmt, prop.ColumnName);
                    templateExpansion_ViewSelect.Add(tmp);
                });
            templateInsert_ViewSelect = string.Join("," + Environment.NewLine, templateExpansion_ViewSelect);
            masterBuilder.Replace("{TemplateInsert_ViewSelect}", templateInsert_ViewSelect);




            EntProperties
                .ForEach(prop =>
                {
                    string frmt = "            rs.[{0}] |as [{0}]";
                    string tmp = string.Format(frmt, prop.PropName);
                    templateExpansion_RsOuterSelect.Add(tmp);
                });


            EntBreakoutTables
                .ForEach((brkTbl) =>
                {
                    string fkJoinAlias = "fkJoin_" + joinCounter;
                    joinCounter++;

                    var FkSchema = brkTbl.TableSchema;
                    var FkTable = brkTbl.TableName;
                    var PkTable = "Entities";

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


                        var tmp = $"            {fkJoinAlias}.[{col}] |as [{col}]";
                        templateExpansion_RsOuterSelect.Add(tmp);
                    });

                });


            templateInsert_Joins = string.Join(Environment.NewLine, templateExpansion_Joins);
            masterBuilder.Replace("{TemplateInsert_Joins}", templateInsert_Joins);




            string templateInsert_RsOuterSelect;

            maxMarker = templateExpansion_RsOuterSelect.Max(x => x.IndexOf('|'));

            for (int i = 0; i < templateExpansion_RsOuterSelect.Count; i++)
            {
                var marker = templateExpansion_RsOuterSelect[i].IndexOf('|');
                var internalPad = new string(' ', (maxMarker - marker) + 4);
                templateExpansion_RsOuterSelect[i] = templateExpansion_RsOuterSelect[i].Replace("|", internalPad);
            }


            templateInsert_RsOuterSelect = string.Join("," + Environment.NewLine, templateExpansion_RsOuterSelect);
            masterBuilder.Replace("{TemplateInsert_RsOuterSelect}", templateInsert_RsOuterSelect);



            return masterBuilder.ToString();
        }
    }
}
