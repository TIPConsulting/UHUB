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
        public static async Task<IEnumerable<string>> GetDatabaseListAsync(SqlConfig SqlConn)
        {
            var query = "SELECT name FROM sys.databases";

            return await SqlWorker.ExecBasicQueryAsync<string>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 10;
                });
        }



        public static async Task<IEnumerable<string>> GetTableColumnsAsync(SqlConfig SqlConn, string Schema, string TableName)
        {
            string query = "select COLUMN_NAME from information_schema.columns where table_schema = @Schema and table_name = @TableName";

            return await SqlWorker.ExecBasicQueryAsync<string>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@Schema", SqlDbType.NVarChar).Value = Schema;
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = TableName;
                });
        }



        public static async Task<IEnumerable<TableColumnInfo>> GetTableColumnInfoAsync(SqlConfig SqlConn, string Schema, string TableName)
        {
            string query = "select * from information_schema.columns where table_schema = @Schema and table_name = @TableName";

            return await SqlWorker.ExecEntityQueryAsync<TableColumnInfo>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@Schema", SqlDbType.NVarChar).Value = Schema;
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = TableName;
                });
        }





        public static async Task<IEnumerable<TableRelationshipInfo>> GetTableRelationshipsAsync(SqlConfig SqlConn)
        {
            return await SqlWorker.ExecEntityQueryAsync<TableRelationshipInfo>(
                SqlConn,
                TableRelationshipQuery,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                });

        }







        public static async Task<bool> DoesTableExistAsync(SqlConfig SqlConn, string TableName)
        {
            return await DoesObjectExistAsync(SqlConn, TableName, SqlObjectType.UserTable);
        }
        public static async Task<bool> DoesViewExistAsync(SqlConfig SqlConn, string ViewName)
        {
            return await DoesObjectExistAsync(SqlConn, ViewName, SqlObjectType.View);
        }
        public static async Task<bool> DoesProcExistAsync(SqlConfig SqlConn, string SprocName)
        {
            return await DoesObjectExistAsync(SqlConn, SprocName, SqlObjectType.Sproc);
        }




        public static async Task<bool> DoesObjectExistAsync(SqlConfig SqlConn, string ObjectName, SqlObjectType ObjectType)
        {

            var objTypeStr = SqlObjectTypeMap[(int)ObjectType];


            var query = "select case when OBJECT_ID(@ObjectName, @ObjectType) IS NOT NULL then cast(1 as bit) else cast(0 as bit) end";

            return await SqlWorker.ExecScalarAsync<bool>(
                SqlConn,
                query,
                (cmd) =>
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@ObjectName", SqlDbType.NVarChar).Value = ObjectName;
                    cmd.Parameters.Add("@ObjectType", SqlDbType.NVarChar).Value = objTypeStr;

                });

        }


        public static async Task<bool> DoesObjectExistAsync(SqlConfig SqlConn, string ObjectName, string ObjectType)
        {

            var query = "select case when OBJECT_ID(@ObjectName, @ObjectType) IS NOT NULL then cast(1 as bit) else cast(0 as bit) end";


            return await SqlWorker.ExecScalarAsync<bool>(
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
