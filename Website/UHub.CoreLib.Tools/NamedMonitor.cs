using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    public class NamedMonitor
    {
        private readonly ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();

        public object this[string name] => _dictionary.GetOrAdd(name, _ => new object());
    }


    public class NamedMonitor<T_Key>
    {
        private readonly ConcurrentDictionary<T_Key, object> _dictionary = new ConcurrentDictionary<T_Key, object>();

        public object this[T_Key name] => _dictionary.GetOrAdd(name, _ => new object());
    }

}
