using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    /// <summary>
    /// Base36 number<>string converter
    /// </summary>
    public static class Base36
    {
        private static BaseConverter converter = new BaseConverter(36);

        /// <summary>
        /// Convert b36 string to int
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int StringToInt(string input)
        {
            return (int)converter.ArbitraryToDecimalSystem(input);
        }

        /// <summary>
        /// Convert b36 string to long
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long StringToLong(string input)
        {
            return converter.ArbitraryToDecimalSystem(input);
        }



        /// <summary>
        /// Convert int to b36 string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string IntToString(int input)
        {
            return converter.DecimalToArbitrarySystem(input);
        }

        /// <summary>
        /// Convert long to b36 string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string LongToString(long input)
        {
            return converter.DecimalToArbitrarySystem(input);
        }

    }
}
