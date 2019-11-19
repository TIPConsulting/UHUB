using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UHub.CoreLib.DataInterop
{
    public static class SqlConverters
    {
        /// <summary>
        /// Parse special DB null type into a form that can be handled by managed code
        /// </summary>
        public static object HandleDBNull(object obj)
        {
            return !(obj is DBNull) ? obj : null;
        }

        /// <summary>
        /// Process out null or empty parameters and replace with DBNULL
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static object HandleParamEmpty(object val)
        {
            switch (val)
            {
                case string valInner:
                    {
                        if (string.IsNullOrEmpty(valInner))
                            return DBNull.Value;
                        else
                            return val;
                    }
                case Guid valInner:
                    {
                        if (valInner == null)
                            return DBNull.Value;
                        else if (valInner == Guid.Empty)
                            return DBNull.Value;
                        else
                            return val;
                    }
                default:
                    {
                        return val ?? DBNull.Value;
                    }
            }
        }

        /// <summary>
        /// Get CLR type from SQL type
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static Type GetClrType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(long?);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool?);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(DateTime?);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal?);

                case SqlDbType.Float:
                    return typeof(double?);

                case SqlDbType.Int:
                    return typeof(int?);

                case SqlDbType.Real:
                    return typeof(float?);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid?);

                case SqlDbType.SmallInt:
                    return typeof(short?);

                case SqlDbType.TinyInt:
                    return typeof(byte?);

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);

                case SqlDbType.Structured:
                    return typeof(DataTable);

                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset?);

                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }
    }
}
