using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    /// <summary>
    /// Base class for converting file sizes between various unit sizes
    /// </summary>
    /// <param name="FileSizeUnit"></param>
    /// <param name="FileSize"></param>
    public sealed class FileSize
    {
        //File size in bytes
        private double _fileSize_Bytes;

        /// <summary>
        /// Create file sized object of specified size and base unit
        /// </summary>
        /// <param name="fileSizeUnit"></param>
        /// <param name="fileSize"></param>
        public FileSize(FileSizeUnit fileSizeUnit, double fileSize)
        {
            if (fileSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fileSize), "File size cannot be negative");
            }

            switch (fileSizeUnit)
            {
                case FileSizeUnit.Bytes:
                    {
                        checked
                        {
                            _fileSize_Bytes = Math.Floor(fileSize);
                            break;
                        }
                    }
                case FileSizeUnit.Kibibytes:
                    {
                        checked
                        {
                            _fileSize_Bytes = Math.Floor(fileSize * 1024);
                            break;
                        }
                    }
                case FileSizeUnit.Mebibyte:
                    {
                        checked
                        {
                            _fileSize_Bytes = Math.Floor(fileSize * Math.Pow(1024, 2));
                            break;
                        }
                    }
                case FileSizeUnit.Gibibyte:
                    {
                        checked
                        {
                            _fileSize_Bytes = Math.Floor(fileSize * Math.Pow(1024, 3));
                            break;
                        }
                    }
                case FileSizeUnit.Tebibyte:
                    {
                        checked
                        {
                            _fileSize_Bytes = Math.Floor(fileSize * Math.Pow(1024, 4));
                            break;
                        }
                    }
                default:
                    {
                        throw new InvalidOperationException("Invalid FileSizeUnit");
                    }
            }
        }

        public double GetAsBytes()
        {
            return _fileSize_Bytes;
        }

        public double GetAsKibibytes()
        {
            return (_fileSize_Bytes / 1024);
        }

        public double GetAsMebibyte()
        {
            return (_fileSize_Bytes / Math.Pow(1024, 2));
        }

        public double GetAsGibibyte()
        {
            return (_fileSize_Bytes / Math.Pow(1024, 3));
        }

        public double GetAsTebibyte()
        {
            return (_fileSize_Bytes / Math.Pow(1024, 4));
        }

    }
}
