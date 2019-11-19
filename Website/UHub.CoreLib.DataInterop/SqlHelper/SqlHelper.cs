using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.DataInterop
{
    public static partial class SqlHelper
    {
        public static IEnumerable<string> GetDatabaseList(SqlConfig SqlConn)
        {
            var query = "SELECT name FROM sys.databases";

            return SqlWorker.ExecBasicQuery<string>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                });
        }


        public static IEnumerable<string> GetTableColumns(SqlConfig SqlConn, string Schema, string TableName)
        {
            string query = "select COLUMN_NAME from information_schema.columns where table_schema = @Schema and table_name = @TableName";

            return SqlWorker.ExecBasicQuery<string>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@Schema", SqlDbType.NVarChar).Value = Schema;
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = TableName;
                });
        }


        public static IEnumerable<TableColumnInfo> GetTableColumnInfo(SqlConfig SqlConn, string Schema, string TableName)
        {
            string query = "select * from information_schema.columns where table_schema = @Schema and table_name = @TableName";

            return SqlWorker.ExecEntityQuery<TableColumnInfo>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@Schema", SqlDbType.NVarChar).Value = Schema;
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = TableName;
                });
        }




        public static IEnumerable<TableRelationshipInfo> GetTableRelationships(SqlConfig SqlConn)
        {
            return SqlWorker.ExecEntityQuery<TableRelationshipInfo>(
                SqlConn,
                TableRelationshipQuery,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                });

        }








        public static bool DoesTableExist(SqlConfig SqlConn, string TableName)
        {
            return DoesObjectExist(SqlConn, TableName, SqlObjectType.UserTable);
        }
        public static bool DoesViewExist(SqlConfig SqlConn, string ViewName)
        {
            return DoesObjectExist(SqlConn, ViewName, SqlObjectType.View);
        }
        public static bool DoesProcExist(SqlConfig SqlConn, string SprocName)
        {
            return DoesObjectExist(SqlConn, SprocName, SqlObjectType.Sproc);
        }




        public static bool DoesObjectExist(SqlConfig SqlConn, string ObjectName, SqlObjectType ObjectType)
        {

            var objTypeStr = SqlObjectTypeMap[(int)ObjectType];


            var query = "select case when OBJECT_ID(@ObjectName, @ObjectType) IS NOT NULL then cast(1 as bit) else cast(0 as bit) end";

            return SqlWorker.ExecScalar<bool>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@ObjectName", SqlDbType.NVarChar).Value = ObjectName;
                    cmd.Parameters.Add("@ObjectType", SqlDbType.NVarChar).Value = objTypeStr;

                });

        }


        public static bool DoesObjectExist(SqlConfig SqlConn, string ObjectName, string ObjectType)
        {

            var query = "select case when OBJECT_ID(@ObjectName, @ObjectType) IS NOT NULL then cast(1 as bit) else cast(0 as bit) end";


            return SqlWorker.ExecScalar<bool>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@ObjectName", SqlDbType.NVarChar).Value = ObjectName;
                    cmd.Parameters.Add("@ObjectType", SqlDbType.NVarChar).Value = ObjectType;
                });

        }

    }
}
