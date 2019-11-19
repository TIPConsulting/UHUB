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
    internal sealed class SelfJoinViewCompiler : ViewCompiler
    {
        private static string InnerTemplate_ParentJoin = @"
    inner join dbo.EntChildXRef {0}
	on
		{0}.ChildEntID = ent.ID";


        private static string InnerTemplate_SelfJoin = @"
    inner join dbo.EntPropertyXRef {0}
	on
		{0}.EntID = ent.ID
        and {0}.PropID = {1}";


        private static string GetSelfJoin(string JoinAlias, string PropID)
        {
            string tmp = string.Format(InnerTemplate_SelfJoin, JoinAlias, PropID);
            return tmp;
        }


        private static string MasterQueryTemplate = @"
    select
{TemplateInsert_Select}

	from 
		dbo.Entities ent


{TemplateInsert_Joins}


	where
		ent.EntTypeID = {TemplateInsert_EntTypeID}
		AND ent.IsDeleted = 0;";





        internal SelfJoinViewCompiler(SqlConfig DbConfig)
        {
            this.DbConfig = DbConfig;
            this.DBManager = new DBManager(this.DbConfig);
        }






        public override string CompileSelect()
        {
            int maxMarker = 0;

            StringBuilder masterBuilder = new StringBuilder(MasterQueryTemplate);

            masterBuilder.Replace("{TemplateInsert_EntTypeID}", EntTypeID.ToString());



            int joinCounter = 0;
            string templateInsert_Joins;
            List<string> templateExpansion_Joins = new List<string>();
            List<string> templateExpansion_Select = new List<string>();
            List<string> allColumns = new List<string>();


            DefaultEntColumns
                .ForEach((x) =>
                {
                    allColumns.Add(x);
                    var tmp = $"        ent.{x} |as [{x}]";
                    templateExpansion_Select.Add(tmp);
                });


            if (HasParentage)
            {
                string fkJoinAlias = "fkJoin_" + joinCounter;
                joinCounter++;

                var tmp = string.Format(InnerTemplate_ParentJoin, fkJoinAlias);

                templateExpansion_Joins.Add(tmp);


                allColumns.Add("ParentID");
                var tmp2 = $"        {fkJoinAlias}.ParentEntID |as [ParentID]";
                templateExpansion_Select.Add(tmp2);
            }



            EntProperties
                .ForEach(x =>
                {
                    string fkJoinAlias = "fkJoin_" + joinCounter;
                    joinCounter++;

                    string tmp = GetSelfJoin(fkJoinAlias, x.ID.ToString());
                    templateExpansion_Joins.Add(tmp);



                    var tmp2 = $"        {fkJoinAlias}.PropValue |as [{x.PropName}]";
                    templateExpansion_Select.Add(tmp2);

                    allColumns.Add(x.PropName);
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
