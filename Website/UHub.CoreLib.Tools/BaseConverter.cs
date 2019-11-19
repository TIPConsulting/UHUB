using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    internal class BaseConverter
    {
        const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private int radix = 2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        internal BaseConverter(int radix)
        {
            if (radix < 2 || radix > Digits.Length)
            {
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());
            }

            this.radix = radix;
        }

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <returns></returns>
        internal string DecimalToArbitrarySystem(long decimalNumber)
        {
            const int BitsInLong = 64;


            if (decimalNumber == 0)
            {
                return "0";
            }

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }


        /// <summary>
        /// Converts the given number from the numeral system with the specified
        /// radix (in the range [2, 36]) to decimal numeral system.
        /// </summary>
        /// <param name="number">The arbitrary numeral system number to convert.</param>
        /// is in (in the range [2, 36]).</param>
        /// <returns></returns>
        internal long ArbitraryToDecimalSystem(string number)
        {

            if (String.IsNullOrEmpty(number))
            {
                return 0;
            }

            // Make sure the arbitrary numeral system number is in upper case
            number = number.ToUpperInvariant();

            long result = 0;
            long multiplier = 1;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char c = number[i];
                if (i == 0 && c == '-')
                {
                    // This is the negative sign symbol
                    result = -result;
                    break;
                }

                int digit = Digits.IndexOf(c);
                if (digit == -1)
                {
                    throw new ArgumentException("Invalid character in the arbitrary numeral system number", nameof(number));
                }

                result += digit * multiplier;
                multiplier *= radix;
            }

            return result;
        }
    }
}
