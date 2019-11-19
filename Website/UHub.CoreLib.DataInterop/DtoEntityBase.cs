using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.DataInterop
{
    /// <summary>
    /// Base class for all custom DTOs.
    /// </summary>
    public abstract class DtoEntityBase
    {
        public T_OUT ToInternal<T_OUT>() where T_OUT : DBEntityBase
        {
            return (T_OUT)(dynamic)this;
        }

    }
}
