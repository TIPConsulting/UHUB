using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Web.Security;
using System.Web.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Web;
using Ganss.XSS;
using Konscious.Security.Cryptography;
using BCrypt.Net;
using UHub.CoreLib.RegExp;
using UHub.CoreLib.RegExp.Compiled;

namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// String extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Check if a string is null or empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        /// <summary>
        /// Check if a string is populated
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
        
        /// <summary>
        /// Get domain portion from email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetEmailDomain(this string email)
        {
            if (email.IsEmpty())
            {
                return "";
            }

            var idx = email.IndexOf("@");
            if (idx == -1)
            {
                return "";
            }


            try
            {
                return email.Substring(idx);
            }
            catch
            {
                return "";
            }
        }
      

        /// <summary>
        /// Removes extra spaces between words
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string InnerTrim(this string str)
        {
            return string.Join(" ", str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Truncate a string a specified length.  Does not trim whitespace
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TrimToLength(this string str, int length)
        {
            if (str.IsEmpty())
            {
                return null;
            }

            if (str.Length <= length)
            {
                return str;
            }

            return str.Substring(0, length);
        }

        /// <summary>
        /// Perform Regex match on a string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RgxIsMatch(this string str, string pattern)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// Perform Regex match on a string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RgxIsMatch(this string str, string pattern, RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, pattern, options);
        }

        /// <summary>
        /// Perform Regex replace on a string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RgxReplace(this string str, string pattern, string replacement)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, pattern, replacement);
        }

        /// <summary>
        /// Perform Regex replace on a string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RgxReplace(this string str, string pattern, string replacement, RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, pattern, replacement, options);
        }

        /// <summary>
        /// Convert special characters to ASCII equivalents
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToAscii(this string str)
        {
            var strMessage = str;

            strMessage = Regex.Replace(strMessage, "[éèëêð]", "e");
            strMessage = Regex.Replace(strMessage, "[ÉÈËÊ]", "E");
            strMessage = Regex.Replace(strMessage, "[àâä]", "a");
            strMessage = Regex.Replace(strMessage, "[ÀÁÂÃÄÅ]", "A");
            strMessage = Regex.Replace(strMessage, "[àáâãäå]", "a");
            strMessage = Regex.Replace(strMessage, "[ÙÚÛÜ]", "U");
            strMessage = Regex.Replace(strMessage, "[ùúûüµ]", "u");
            strMessage = Regex.Replace(strMessage, "[òóôõöø]", "o");
            strMessage = Regex.Replace(strMessage, "[ÒÓÔÕÖØ]", "O");
            strMessage = Regex.Replace(strMessage, "[ìíîï]", "i");
            strMessage = Regex.Replace(strMessage, "[ÌÍÎÏ]", "I");
            strMessage = Regex.Replace(strMessage, "[š]", "s");
            strMessage = Regex.Replace(strMessage, "[Š]", "S");
            strMessage = Regex.Replace(strMessage, "[ñ]", "n");
            strMessage = Regex.Replace(strMessage, "[Ñ]", "N");
            strMessage = Regex.Replace(strMessage, "[ç]", "c");
            strMessage = Regex.Replace(strMessage, "[Ç]", "C");
            strMessage = Regex.Replace(strMessage, "[ÿ]", "y");
            strMessage = Regex.Replace(strMessage, "[Ÿ]", "Y");
            strMessage = Regex.Replace(strMessage, "[ž]", "z");
            strMessage = Regex.Replace(strMessage, "[Ž]", "Z");
            strMessage = Regex.Replace(strMessage, "[Ð]", "D");
            strMessage = Regex.Replace(strMessage, "[œ]", "oe");
            strMessage = Regex.Replace(strMessage, "[Œ]", "Oe");
            strMessage = Regex.Replace(strMessage, "[«»\u201C\u201D\u201E\u201F\u2033\u2036]", "\"");
            strMessage = Regex.Replace(strMessage, "[\u2026]", "...");

            return strMessage;
        }

        /// <summary>
        /// Escape string and make it HTML friendly
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string XmlEncode(this string str)
        {
            if (str.IsEmpty())
                return str;

            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = str;
            return node.InnerXml;
        }

        /// <summary>
        /// Basic HTML escape that converts special characters to their HTML safe equivalents
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string HtmlEncode(this string str)
        {
            return WebUtility.HtmlEncode(str);
        }

        /// <summary>
        /// Basic HTML decode that converts HTML safe characters to their standard form
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string HtmlDecode(this string str)
        {
            return WebUtility.HtmlDecode(str);
        }

        /// <summary>
        /// Basic URL escape that converts special characters to their URL safe equivalents
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UrlEncode(this string str)
        {
            return WebUtility.UrlEncode(str);
        }

        /// <summary>
        /// Basic URL decode that converts URL safe characters to their standard form
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UrlDecode(this string str)
        {
            return WebUtility.UrlDecode(str);
        }

        /// <summary>
        /// Smart HTML sanitizer that can remove most dangerous elements without escaping the HTML
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SanitizeHtml(this string str)
        {
            var attrWhiteList = HtmlSanitizer.DefaultAllowedAttributes;

            HtmlSanitizer sanitizer = new HtmlSanitizer(allowedAttributes: attrWhiteList);
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("data-Index");
            sanitizer.AllowedAttributes.Add("data-URL");
            sanitizer.AllowedAttributes.Add("data-MinIndx");
            sanitizer.AllowedAttributes.Add("data-MaxIndx");
            sanitizer.AllowedAttributes.Add("data-Active");
            sanitizer.AllowDataAttributes = true;

            sanitizer.AllowedSchemes.Add("mailto");

            return sanitizer.Sanitize(str);
        }

        /// <summary>
        /// Get first index of any string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strng"></param>
        /// <param name="searchStrng"></param>
        /// <returns></returns>
        public static int FirstIndexOfAny<T>(this string strng, List<T> searchStrng)
        {
            int num = -1;
            foreach (T x in searchStrng)
            {
                if (strng.Contains(Convert.ToString(x)))
                {
                    if (strng.IndexOf(Convert.ToString(x)) < num || num == -1)
                        num = strng.IndexOf(Convert.ToString(x));
                }
            }
            return num;
        }

        /// <summary>
        /// Get last index of any string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strng"></param>
        /// <param name="searchStrng"></param>
        /// <returns></returns>
        public static int LastIndexOfAny<T>(this string strng, List<T> searchStrng)
        {
            int num = -1;
            foreach (T x in searchStrng)
            {
                if (strng.Contains(Convert.ToString(x)))
                {
                    if (strng.LastIndexOf(Convert.ToString(x)) > num)
                        num = strng.LastIndexOf(Convert.ToString(x));
                }
            }
            return num;
        }

        /// <summary>
        /// Replace first instance of a sub string
        /// </summary>
        /// <param name="strng"></param>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string strng, string OldValue, string NewValue)
        {
            System.Text.StringBuilder builder = new StringBuilder();
            builder.Append(strng.Substring(0, strng.IndexOf(OldValue)));
            builder.Append(NewValue);
            builder.Append(strng.Substring(strng.IndexOf(OldValue) + 1));
            return builder.ToString();
        }

        /// <summary>
        /// Capitalize each word of a string
        /// </summary>
        /// <param name="strng"></param>
        /// <returns></returns>
        public static string ToSentenceCase(this string strng)
        {
            List<string> b = new List<string>();
            strng.Split(' ').ToList().ForEach(x => { b.Add(x.Substring(0, 1).ToUpper() + x.Substring(1).ToLower()); });
            return string.Join(" ", b);
        }

        /// <summary>
        /// Get memory stream from string
        /// </summary>
        /// <param name="strng"></param>
        /// <returns></returns>
        public static Stream GetStream(this string strng)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(strng);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Convert hexadecimal string to Byte[] array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexDecode(this string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }

        /// <summary>
        /// Calculate hash of a string
        /// </summary>
        /// <param name="strng"></param>
        /// <param name="hashType"></param>
        /// <returns></returns>
        public static string GetHash(this string strng, HashType hashType)
        {
            if (strng.IsEmpty())
            {
                return null;
            }

            byte[] mKeyBytes_TEMP = null;
            string mKeyString;
            try
            {

                mKeyBytes_TEMP = MachineKeyHelper.GetValidationKey();
                if (mKeyBytes_TEMP != null)
                {
                    mKeyString = BitConverter.ToString(mKeyBytes_TEMP).Replace("-", string.Empty);
                }
                else
                {
                    mKeyString = "";
                }
            }
            catch
            {
                mKeyString = "";
            }
            var mKeyByte_ACTUAL = mKeyString.HexDecode();
            var msgBytes = Encoding.UTF8.GetBytes(strng);


            HashAlgorithm hashAlgBasic = null;

            switch (hashType)
            {
                case HashType.MD5:
                    {
                        hashAlgBasic = MD5.Create();
                        break;
                    }
                case HashType.SHA1:
                    {
                        hashAlgBasic = SHA1.Create();
                        break;
                    }
                case HashType.SHA256:
                    {
                        hashAlgBasic = SHA256.Create();
                        break;
                    }

                case HashType.SHA512:
                    {
                        hashAlgBasic = SHA512.Create();
                        break;
                    }
                case HashType.HMACSHA256:
                case HashType.HMACSHA512:
                    {
                        HMAC hashAlgHmac;
                        if (hashType == HashType.HMACSHA256)
                        {
                            hashAlgHmac = new HMACSHA256(mKeyByte_ACTUAL);
                        }
                        else
                        {
                            hashAlgHmac = new HMACSHA512(mKeyByte_ACTUAL);
                        }
                        var hashBytes = hashAlgHmac.ComputeHash(Encoding.UTF8.GetBytes(strng));
                        hashAlgHmac.Dispose();
                        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                default:
                    {
                        throw new InvalidOperationException("Invalid hash type");
                    }
            }


            var bytes = Encoding.Unicode.GetBytes(strng);
            byte[] hash = hashAlgBasic.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            hashAlgBasic.Dispose();
            return sb.ToString();

        }

        /// <summary>
        /// Get cryptographically secure hash of string.  Use for passwords.  Uses machinekey for keyed hashing
        /// </summary>
        /// <param name="strng"></param>
        /// <param name="HashType"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GetCryptoHash(this string strng, CryptoHashType HashType, string salt = "")
        {
            if (strng.IsEmpty())
            {
                return null;
            }

            byte[] mKeyBytes_TEMP = null;
            string mKeyString;
            try
            {

                mKeyBytes_TEMP = MachineKeyHelper.GetValidationKey();
                if (mKeyBytes_TEMP != null)
                {
                    mKeyString = BitConverter.ToString(mKeyBytes_TEMP).Replace("-", string.Empty);
                }
                else
                {
                    mKeyString = "";
                }
            }
            catch
            {
                mKeyString = "";
            }
            var mKeyByte_ACTUAL = mKeyString.HexDecode();
            var msgBytes = Encoding.UTF8.GetBytes(strng);
            var psdBytes = Encoding.Unicode.GetBytes(strng);
            byte[] saltBytes = null;
            if (salt.IsNotEmpty())
            {
                saltBytes = Encoding.Unicode.GetBytes(salt);
            }


            if (HashType == CryptoHashType.Bcrypt)
            {
                var cryptSalt = BCrypt.Net.BCrypt.GenerateSalt(10, SaltRevision.Revision2Y);
                var hash = BCrypt.Net.BCrypt.HashPassword(strng, cryptSalt);

                return hash;
            }

            if (HashType == CryptoHashType.HMACBlake2)
            {
                HMACBlake2B blk2 = new HMACBlake2B(psdBytes, 512);
                blk2.Key = mKeyByte_ACTUAL;
                blk2.Initialize();

                using (var stream = (strng + salt).GetStream())
                {
                    byte[] hash = blk2.ComputeHash(stream);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("X2"));
                    }

                    return sb.ToString();
                }
            }

            if (HashType == CryptoHashType.Argon2)
            {
                Argon2i argonHash = new Argon2i(psdBytes);
                if (salt.IsNotEmpty())
                {
                    argonHash.Salt = saltBytes;
                }
                argonHash.KnownSecret = mKeyByte_ACTUAL;
                argonHash.DegreeOfParallelism = 2;
                argonHash.MemorySize = 10 * 1024;
                argonHash.Iterations = 10;
                var hash = argonHash.GetBytes(128);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }


            strng = strng + salt;
            if (HashType == CryptoHashType.HMACSHA256)
            {
                return strng.GetHash(Tools.HashType.HMACSHA256);
            }
            else if (HashType == CryptoHashType.HMACSHA512)
            {
                return strng.GetHash(Tools.HashType.HMACSHA512);
            }
            else if (HashType == CryptoHashType.SHA512)
            {
                return strng.GetHash(Tools.HashType.SHA512);
            }

            return null;
        }


        private const string Purpose = "ProtectedData";
        /// <summary>
        /// Encrypt string using machine key
        /// </summary>
        /// <param name="strng"></param>
        /// <param name="Purpose"></param>
        /// <returns></returns>
        public static string Encrypt(this string strng, string Purpose = Purpose)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(strng);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }
        /// <summary>
        /// Decrypt string using machine key
        /// </summary>
        /// <param name="strng"></param>
        /// <param name="Purpose"></param>
        /// <returns></returns>
        public static string Decrypt(this string strng, string Purpose = Purpose)
        {
            var protectedBytes = Convert.FromBase64String(strng);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
            return unprotectedText;
        }
    }
}
