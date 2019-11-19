using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Tools.Extensions
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Calculate cryptoHash of a file (does not support bcrypt)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="hashType"></param>
        /// <returns></returns>
        public static string GetHash(this FileInfo file, HashType hashType)
        {
            if (!file.Exists)
            {
                return null;
            }

            var mKeyBytes = MachineKeyHelper.GetValidationKey();
            string mKeyString;
            if (mKeyBytes != null)
            {
                mKeyString = Encoding.UTF8.GetString(mKeyBytes);
            }
            else
            {
                mKeyString = "";
            }
            //TEMP KEY FOR TESTING
            //mKeyString = "61574d6b157f757d02457573556645750e0341481b127a07476303136c005145436c7b46651c6e4f4f040e1569464a794e534309097258550c17616075060950";


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
                            hashAlgHmac = new HMACSHA256(mKeyString.HexDecode());
                        }
                        else
                        {
                            hashAlgHmac = new HMACSHA512(mKeyString.HexDecode());
                        }
                        using (var stream = file.OpenRead())
                        {
                            var hashBytes = hashAlgHmac.ComputeHash(stream);
                            hashAlgHmac.Dispose();
                            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                        }
                    }
                default:
                    {
                        throw new InvalidOperationException("Invalid hash type");
                    }
            }

            using (var stream = file.OpenRead())
            {
                byte[] hash = hashAlgBasic.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                hashAlgBasic.Dispose();
                return sb.ToString();
            }

        }

        /// <summary>
        /// Change the name of a file to a spcified new name
        /// </summary>
        /// <param name="info"></param>
        /// <param name="NewName">New file name - include extension</param>
        public static void Rename(this FileInfo info, string NewName)
        {
            string dirPath = info.Directory.FullName;
            string newFilePath = Path.Combine(dirPath, NewName);

            if (File.Exists(newFilePath))
            {
                throw new InvalidOperationException("Cannot override existing file");
            }

            //this method used CopyTo instead of MoveTo
            //This is to avoid security issues

            //copy file to new name
            info.CopyTo(newFilePath);
            //delete old version
            info.Delete();

        }

        /// <summary>
        /// Removes EXIF data from images
        /// </summary>
        /// <param name="info"></param>
        public static void RemoveExifData(this FileInfo info)
        {
            string fullName = info.FullName;

            //create new file, delete old file, rename new file
            //this is to avoid the possibility of data loss while a file is being recreated
            Image img;
            using (var bmpTemp = new Bitmap(fullName))
            {
                img = new Bitmap(bmpTemp);
                img.Save(fullName + ".temp", ImageFormat.Png);
            }
            info.Delete();

            new FileInfo(fullName + ".temp").Rename(fullName);
        }

    }
}
