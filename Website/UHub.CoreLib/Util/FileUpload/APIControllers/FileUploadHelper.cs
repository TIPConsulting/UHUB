//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using UHub.CoreLib;
//using UHub.CoreLib.Management;
//using UHub.CoreLib.Extensions;


//namespace UHub.CoreLib.Util.FileUpload
//{
//    internal class FileUploadHelper
//    {
//        private const string PARENT_UID_HEADER = "X-Ent-Parent";

//        private readonly string _uploadPath;
//        private readonly MultipartFormDataStreamProvider _streamProvider;

//        #region Properties

//        private string LocalFileName
//        {
//            get
//            {
//                return _streamProvider.FileData.FirstOrDefault().LocalFileName;
//            }
//        }

//        private string OriginalFileName
//        {
//            get
//            {
//                MultipartFileData fileData = _streamProvider.FileData.FirstOrDefault();
//                return fileData?.Headers?.ContentDisposition?.FileName?.Replace("\"", string.Empty) ?? string.Empty;
//            }
//        }

//        private string OriginalFileMIME
//        {
//            get
//            {
//                MultipartFileData fileData = _streamProvider.FileData.FirstOrDefault();
//                return fileData.Headers.Where(x => x.Key == "Content-Type").SingleOrDefault().Value.First();
//            }
//        }

//        private string OriginalFileExtension
//        {
//            get
//            {
//                return "." + OriginalFileName.Split('.').ToList().Last().ToLower();
//            }
//        }

//        #endregion

//        internal static void ValidateFileSize(FileInfo TempFile)
//        {
//            if (CoreFactory.Singleton.Properties.MaxFileUploadSize.GetAsBytes() == 0)
//            {
//                return;
//            }
//            if (TempFile.Length > CoreFactory.Singleton.Properties.MaxFileUploadSize.GetAsBytes())
//            {
//                TempFile.Delete();
//                throw new InvalidOperationException("File is too large");
//            }
//        }

//        internal static void ValidateFileSize(double FileSize, string FilePath)
//        {
//            if (CoreFactory.Singleton.Properties.MaxFileUploadSize.GetAsBytes() == 0)
//            {
//                return;
//            }
//            //check total file size against the limit
//            if (FileSize > CoreFactory.Singleton.Properties.MaxFileUploadSize.GetAsBytes())
//            {
//                if (FilePath.IsNotEmpty())
//                {
//                    File.Delete(FilePath);
//                }
//                throw new InvalidOperationException("File is too large");
//            }
//        }

//        internal static void ValidateClientChunkID(string FileName)
//        {
//            if (FileName.IsEmpty())
//            {
//                throw new InvalidOperationException("Client Chunk ID Invalid");
//            }

//            if (!FileName.RgxIsMatch($@"^{RgxPatterns.FileUpload.CHUNK_ID}$", RegexOptions.IgnoreCase))
//            {
//                throw new InvalidOperationException("Client Chunk ID Invalid");
//            }
//        }


//        internal static void ValidateFileMetaInfo(string Extension, string MIME, string Path, string parentUIDHeader)
//        {

//            //verify file extension
//            if (!ParentManagerLocal.FileManager.IsExtensionValid(Extension))
//            {
//                File.Delete(Path);
//                throw new InvalidOperationException($"Invalid File Type ({Extension})");
//            }


//            var fileMetaInfo = ParentManagerLocal.UtilReader.GetFileTypeMetaByExtension(Extension);

//            //verify valid file type
//            if (fileMetaInfo == null || fileMetaInfo.Count() == 0)
//            {
//                File.Delete(Path);
//                throw new InvalidOperationException($"Invalid file type ({Extension})");
//            }

//            var fileExtension = fileMetaInfo.FirstOrDefault()?.FileExtension ?? null;

//            var fileMIME = fileMetaInfo.FirstOrDefault()?.MIMETypes ?? new List<string>();
//            var fileMIME_RAW = fileMetaInfo.FirstOrDefault()?.MIMEType_RAW ?? null;
//            //verify MIME/extension match (again)
//            if (fileExtension.IsEmpty() || !fileMIME.Contains(MIME))
//            {
//                File.Delete(Path);

//                var builder = new StringBuilder();
//                builder.Append("MIME/EXTENSION MISMATCH");
//                builder.Append(Environment.NewLine);
//                builder.Append(Environment.NewLine);
//                builder.Append("----------Found----------");
//                builder.Append($"EXTENSION:{Extension}");
//                builder.Append(Environment.NewLine);
//                builder.Append($"MIME:{MIME}");
//                builder.Append(Environment.NewLine);
//                builder.Append(Environment.NewLine);
//                builder.Append("----------Expected----------");
//                builder.Append($"EXTENSION:{fileExtension}");
//                builder.Append(Environment.NewLine);
//                builder.Append($"MIME:{fileMIME_RAW}");
//                CoreFactory.Singleton.Logging.CreateErrorLog(builder.ToString());
//                throw new InvalidOperationException("MIME/EXTENSION mismatch");
//            }

//            try
//            {
//                Guid parentUID = new Guid();

//                var hasValidParentHeader = parentUIDHeader.IsNotEmpty() && Guid.TryParse(parentUIDHeader, out parentUID);

//                IEntityBase parentEnt;

//                if (hasValidParentHeader && parentUID != CoreFactory.Singleton.EntRoot.EntUID)
//                {
//                    parentEnt = CoreFactory.Singleton.EntReader.GetEntity<Entity>(parentUID, false);
//                }
//                else
//                {
//                    parentEnt = CoreFactory.Singleton.EntRoot;
//                }

//                var allowedChildTypes = CoreFactory.Singleton.EntReader.GetEntChildMap()[parentEnt.EntType];
//                var isImage = fileMetaInfo.Any(x => x.FileCategory == FileCategory.Image);

//                bool isValid = true;
//                if (isImage)
//                {
//                    //check if image type is allowed
//                    isValid =
//                        allowedChildTypes.Contains(EntityTypes.Image) &&
//                        (
//                            CoreFactory.Singleton.Properties.AllowedFileUploadTypes.Count == 0 ||
//                            CoreFactory.Singleton.Properties.AllowedFileUploadTypes.Contains(FileCategory.Image)
//                        );
//                }
//                else
//                {
//                    //check if the necessary file type is allowed
//                    isValid =
//                        allowedChildTypes.Contains(EntityTypes.File) &&
//                        (
//                            CoreFactory.Singleton.Properties.AllowedFileUploadTypes.Count == 0 ||
//                            fileMetaInfo.Any(x => CoreFactory.Singleton.Properties.AllowedFileUploadTypes.Contains(x.FileCategory))
//                        );

//                }
//                if (!isValid)
//                {
//                    throw new InvalidOperationException("Invalid child type");
//                }
//            }
//#pragma warning disable CS0168 // The variable 'ex' is declared but never used
//            catch (Exception ex)
//#pragma warning restore CS0168 // The variable 'ex' is declared but never used
//            {
//                File.Delete(Path);
//                throw new InvalidOperationException("Invalid child type");
//            }


//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="uploadPath"></param>
//        /// <param name="AcceptableFileTypes">List of acceptable file extensions</param>
//        public FileUploadHelper(string uploadPath)
//        {
//            _uploadPath = uploadPath;
//            _streamProvider = new MultipartFormDataStreamProvider(_uploadPath);
//        }

//        #region Interface

//        public async Task<UploadProcessingResult> HandleRequest(HttpRequestMessage request)
//        {

//            var totalContentLength = request.Headers.Where(x => x.Key == "Content-TotalFileLength").Single().Value.First();
//            long _fileSize;
//            if (long.TryParse(totalContentLength, out _fileSize))
//            {
//                ValidateFileSize(_fileSize, null);

//                await request.Content.ReadAsMultipartAsync(_streamProvider);
//                return await ProcessFile(request);
//            }
//            else
//            {
//                CoreFactory.Singleton.Logging.CreateErrorLog(new Exception("FileTooLarge"));
//                throw new InvalidOperationException("File content length not specified");
//            }
//        }

//        #endregion

//        #region Private implementation

//        private async Task<UploadProcessingResult> ProcessFile(HttpRequestMessage request)
//        {
//            if (request.IsChunkUpload())
//            {
//                return await ProcessChunk(request);
//            }


//            return new UploadProcessingResult()
//            {
//                IsComplete = true,
//                FileName = OriginalFileName.Substring(0, OriginalFileName.LastIndexOf('.')),
//                FileExtension = OriginalFileExtension,
//                FileMIME = OriginalFileMIME,
//                LocalFilePath = LocalFileName,
//                FileMetadata = _streamProvider.FormData
//            };
//        }



//        /// <summary>
//        /// Check if a request message is part of a chunked upload
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        public static bool IsChunkUpload(this HttpRequestMessage request)
//        {
//            return request.Content.Headers.ContentRange != null;
//        }

//        /// <summary>
//        /// Get chunk metadata from a request message
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        public static FileUploadChunkMetaData GetChunkMetaData(this HttpRequestMessage request)
//        {
//            return new FileUploadChunkMetaData()
//            {
//                ChunkIdentifier = request.Headers.Contains("X-File-Identifier") ? request.Headers.GetValues("X-File-Identifier").FirstOrDefault() : null,
//                ChunkStart = request.Content.Headers.ContentRange.From,
//                ChunkEnd = request.Content.Headers.ContentRange.To,
//                TotalLength = request.Content.Headers.ContentRange.Length
//            };
//        }



//        private async Task<UploadProcessingResult> ProcessChunk(HttpRequestMessage request)
//        {

//            //get extension
//            //just the final portion
//            var nameParts = OriginalFileName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
//            if (nameParts.Count() < 2)
//            {
//                throw new InvalidOperationException($"Invalid File - file type cannot be determined");
//            }
//            string extension = "." + nameParts?.Last()?.ToLower();


//            //use the unique identifier sent from client to identify the file
//            FileUploadChunkMetaData chunkMetaData = request.GetChunkMetaData();

//            ValidateClientChunkID(chunkMetaData.ChunkIdentifier);

//            string filePath = Path.Combine(_uploadPath, string.Format("{0}.temp", chunkMetaData.ChunkIdentifier));

//            var parentHeader = request.Headers.Where(x => x.Key == PARENT_UID_HEADER).Single().Value.First();
//            ValidateFileMetaInfo(extension, OriginalFileMIME, filePath, parentHeader);



//            ////check file length again at start of chunk (in case the header is hacked)
//            var tempFileInfo1 = new FileInfo(filePath);
//            if (tempFileInfo1.Exists)
//            {
//                //#TODO
//                if (CoreFactory.Singleton.Properties.MaxFileUploadSize.GetAsBytes() != 0)
//                {
//                    if (tempFileInfo1.Length > CoreFactory.Singleton.Properties.MaxFileUploadSize.GetAsBytes())
//                    {
//                        tempFileInfo1.Delete();
//                        throw new InvalidOperationException("File is too large");
//                    }
//                }
//            }

//            //append chunks to construct original file
//            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate | FileMode.Append))
//            {
//                var localFileInfo = new FileInfo(LocalFileName);
//                var localFileStream = localFileInfo.OpenRead();

//                await localFileStream.CopyToAsync(fileStream);
//                await fileStream.FlushAsync();

//                fileStream.Close();
//                localFileStream.Close();

//                //delete chunk
//                localFileInfo.Delete();
//            }

//            //check file size again at end of chunk (in case the header is hacked)
//            var tempFileInfo2 = new FileInfo(filePath);
//            ValidateFileSize(tempFileInfo2);

//            ValidateFileSize(tempFileInfo2.Length, filePath);



//            return new UploadProcessingResult()
//            {
//                IsComplete = chunkMetaData.IsLastChunk,
//                FileName = OriginalFileName.Substring(0, OriginalFileName.LastIndexOf('.')),
//                FileExtension = extension,
//                FileMIME = OriginalFileMIME,
//                LocalFilePath = chunkMetaData.IsLastChunk ? filePath : null,
//                FileMetadata = _streamProvider.FormData
//            };

//        }

//        #endregion


//    }
//}
