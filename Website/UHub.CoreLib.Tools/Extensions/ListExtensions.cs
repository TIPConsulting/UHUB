using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// List extension methods
    /// </summary>
    public static class ListExtensions
    {
        private static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static Random Local;
            public static Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }

        public class DuplicateInfo<T>
        {
            public T Value { get; set; }
            public long Count { get; set; }
        }



        /// <summary>
        /// Randomize order of objects in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Randomize<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }




        public static IEnumerable<DuplicateInfo<T>> DuplicateCounts<T>(this List<T> list)
        {
            Dictionary<T, DuplicateInfo<T>> dupeInfo = new Dictionary<T, DuplicateInfo<T>>();

            for (int i = 0; i < list.Count; i++)
            {
                DuplicateInfo<T> info;
                if (!dupeInfo.TryGetValue(list[i], out info))
                {
                    info = new DuplicateInfo<T>
                    {
                        Value = list[i],
                        Count = 0
                    };
                    dupeInfo.Add(list[i], info);
                }


                info.Count++;
            }

            return dupeInfo.Values;
        }


        public static IEnumerable<T> Duplicates<T>(this List<T> list)
        {
            var counts = DuplicateCounts(list).ToList();

            for (int i = 0; i < counts.Count; i++)
            {
                if (counts[i].Count > 1)
                {
                    yield return counts[i].Value;
                }
            }

        }

    }
}
