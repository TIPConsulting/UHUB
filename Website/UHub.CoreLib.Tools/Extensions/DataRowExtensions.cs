using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Tools.Extensions
{
    public static class DataRowExtensions
    {
        /// <summary>
        /// Convert SQL DataRow to <see cref="DBEntityBase"/> via autoloader
        /// </summary>
        /// <typeparam name="T"><see cref="DBEntityBase"/> return type</typeparam>
        /// <param name="row">SQL DataRow</param>
        /// <returns></returns>
        public static T ToCustomDBType<T>(this DataRow row) where T : DBEntityBase, new()
        {
            if (row == null)
            {
                return null;
            }

            var newEnt = new T();
            newEnt.LoadDataRow(row);

            return newEnt;
        }

        /// <summary>
        /// Check if a dataRow has a specific column value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool HasColumn(this DataRow row, string column)
        {
            return row.Table.Columns.Contains(column);
        }

    }
}
