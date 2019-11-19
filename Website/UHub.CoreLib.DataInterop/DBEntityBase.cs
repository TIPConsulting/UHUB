using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.DataInterop
{
    /// <summary>
    /// Base class for all custom DB interface objects.
    /// </summary>
    public abstract class DBEntityBase
    {
        public T_OUT ToDto<T_OUT>() where T_OUT : DtoEntityBase
        {
            return (T_OUT)(dynamic)this;
        }


        public virtual void LoadDataRow(DataRow DataRow) => throw new NotImplementedException();


        public virtual void LoadDataReader(SqlDataReader DataReader) => throw new NotImplementedException();
    }
}
