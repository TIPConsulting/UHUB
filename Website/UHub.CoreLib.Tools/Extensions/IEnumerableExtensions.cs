using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace UHub.CoreLib.Tools.Extensions
{
    public static class IEnumerableExtensions
    {
        public static double Median(this IEnumerable<byte> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<short> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<int> set)
        {
            var list = set.ToList();

            if(list.Count == 0)
            {
                return 0;
            }
            if(list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<long> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<float> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<double> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }





        /// <summary>
        /// Order list by direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static IOrderedEnumerable<TSource> orderByDirectionCore<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection dir = SortDirection.Ascending)
        {
            if (source.Count() > 0)
            {
                if (dir == SortDirection.Ascending)
                {
                    return source.OrderBy(keySelector);
                }
                else
                {
                    return source.OrderByDescending(keySelector);
                }
            }
            else
                return (IOrderedEnumerable<TSource>)Enumerable.Empty<TSource>();
        }

        /// <summary>
        /// Order list  with variable direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="useAsc"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool useAsc = true)
        {
            var dir = useAsc ? SortDirection.Ascending : SortDirection.Descending;
            return orderByDirectionCore(source, keySelector, dir);
        }

        /// <summary>
        /// Order list with variable direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection dir = SortDirection.Ascending)
        {
            return orderByDirectionCore(source, keySelector, dir);
        }

        /// <summary>
        /// Order list with variable direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="useAsc"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource>(this IEnumerable<TSource> source, string sortProperty, bool useAsc = true)
        {
            var dir = useAsc ? SortDirection.Ascending : SortDirection.Descending;

            Func<TSource, object> keySelector = (itm) => typeof(TSource).GetProperty(sortProperty).GetValue(itm, null);

            return orderByDirectionCore(source, keySelector, dir);
        }

        /// <summary>
        /// Order list with variable direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource>(this IEnumerable<TSource> source, string sortProperty, SortDirection dir = SortDirection.Ascending)
        {
            Func<TSource, object> keySelector = (itm) => typeof(TSource).GetProperty(sortProperty).GetValue(itm, null);

            return orderByDirectionCore(source, keySelector, dir);
        }



    }
}
