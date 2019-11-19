//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Net;
//using System.Net.Http;
//using System.Web;
//using System.Web.Http;
//using System.IO;
//using UHub.CoreLib.Management;
//using UHub.CoreLib.APIControllers;
//using UHub.CoreLib.Extensions;

//namespace UHub.CoreLib.Util.FileUpload.APIControllers
//{
//    [RoutePrefix(Common.API_ROUTE_PREFIX + "/util")]
//    internal sealed class FileUploadController : APIController
//    {
//        private const string PARENT_UID_HEADER = "X-Ent-Parent";


//        private protected override bool ValidateSystemState(out string error)
//        {
//            if (!base.ValidateSystemState(out error))
//            {
//                return false;
//            }


//            //CHECK IF UPLOADS ARE ALLOWED
//            if (!CoreFactory.Singleton.Properties.EnableAPIFileUploads)
//            {
//                error = "File Uploads Disabled";
//                return false;
//            }

//            return true;
//        }


//        [Route("FileUpload")]
//        [HttpPost()]
//        public async Task<IHttpActionResult> FileUpload()
//        {
//            if (!base.ValidateSystemState(out var error))
//            {
//                return Ok(error);
//            }

//            var isLoggedIn = IsUserLoggedIn(out var currentUser, out TokenValidationStatus tokenStatus);
//            HandleFatalTokenStatus(tokenStatus);


//            if (!isLoggedIn)
//            {
//                return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.Forbidden, "User Not Authenticated");
//            }
//            //CHECK USER PRIVS
//            if (!currentUser?.UserSecurityTypes?.Contains(UserSecurityTypes.ent_CanCreateFile) ?? false)
//            {
//                return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.Forbidden, "User Does Not Have Permission");
//            }



//            return await FileUpload_Worker();
//        }


//        private async Task<IHttpActionResult> FileUpload_Worker()
//        {
//            string uploadPath = CoreFactory.Singleton.Properties.TempCacheDirectory;
//            //PROCESS REQUEST
//            if (Request.Content.IsMimeMultipartContent("form-data"))
//            {

//                List<IHttpActionResult> messages = new List<IHttpActionResult>();

//                try
//                {
//                    var uploadFileService = new FileUploadHelper(uploadPath);
//                    UploadProcessingResult uploadResult = await uploadFileService.HandleRequest(Request);

//                    if (uploadResult.IsComplete)
//                    {
//                        WriteEntUpload(uploadResult);
//                    }

//                    return Ok("Waiting");
//                }
//                catch (InvalidOperationException ex)
//                {
//                    //dont bother logging these becuase they are most likely intentionally thrown by the processor as permission/validation errors
//                    return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
//                }
//                catch (IOException ex)
//                {
//                    if (ex.Message.Contains("Error reading MIME multipart body part. ---> System.Web.HttpException: The client disconnected"))
//                    {
//                        //Error fired when user aborts an upload
//                        //No action is necessary
//                        return Ok("Operation Aborted");
//                    }
//                    else
//                    {
//                        //Log these because they shouldnt be here
//                        CoreFactory.Singleton.Logging.CreateErrorLog(ex);
//                        return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.InternalServerError, "Server Error");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    //Log these because they shouldnt be here
//                    CoreFactory.Singleton.Logging.CreateErrorLog(ex);
//                    return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.InternalServerError, "Server Error");
//                }

//            }
//            else
//            {
//                return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request");
//            }
//        }


//        private void WriteEntUpload(UploadProcessingResult uploadResult)
//        {
//            var request = HttpContext.Current.Request;

//            FileUploadHelper.ValidateFileMetaInfo(uploadResult.FileExtension, uploadResult.FileMIME, uploadResult.LocalFilePath, request.Headers[PARENT_UID_HEADER]);


//            //check if the client file name can be used as the Ent Name
//            var canUseClientName = uploadResult.FileName.IsValidFileName();

//            //check if the resource is an Image or Generic File
//            EntityTypes entType;
//            var imageMetaInfo = CoreFactory.Singleton.UtilReader.GetFileTypeMetaByCategory(FileCategory.Image);
//            var currentFileInfo = new FileInfo(uploadResult.LocalFilePath);
//            string trueName = Guid.NewGuid().ToString();
//            string truePath;
//            //MOVE TO IMAGE FOLDER
//            //check to see if the uploaded file extension is an image type
//            if (imageMetaInfo.Select(x => x.FileExtension).Contains(uploadResult.FileExtension))
//            {
//                entType = EntityTypes.Image;
//                var dir = CoreFactory.Singleton.FileManager.GetCurrentActiveImageDirectory();
//                truePath = Path.Combine(dir, trueName);
//                //TODO: Investigate better exif systems
//                //currentFileInfo.RemoveExifData();
//            }
//            //MOVE TO FILE FOLDER
//            //generic files move to the other folder
//            else
//            {
//                entType = EntityTypes.File;
//                var dir = CoreFactory.Singleton.FileManager.GetCurrentActiveFileDirectory();
//                truePath = Path.Combine(dir, trueName);
//            }

//            currentFileInfo.CopyTo(truePath);
//            currentFileInfo.Delete();
//            /////////////////////////////////
//            //WRITE TO PERSISTENT STORAGE
//            ////////////////////////////////


//            //TODO: add better filename processing

//            var trueFileInfo = new FileInfo(truePath);

//            Guid parentUID = new Guid();
//            var hasValidRequestParent = request.Headers[PARENT_UID_HEADER] != null && Guid.TryParse(request.Headers[PARENT_UID_HEADER], out parentUID);

//            var entName = canUseClientName ? uploadResult.FileName.TrimToLength(200) : $"New {entType}";

//            var ent = Entity.Create(ParentManager, entName, entType);
//            ent.Description = uploadResult.FileName;
//            //
//            ent.ParentUID = hasValidRequestParent ? parentUID : parentUID;
//            //
//            switch (entType)
//            {
//                case EntityTypes.Image:
//                    {
//                        var entDefaults = CoreFactory.Singleton.EntReader.GetImageDefault();
//                        ent.IsEnabled = entDefaults.IsEnabled;
//                        ent.IsDefault = entDefaults.IsDefault;
//                        ent.IsMenuItem = entDefaults.IsMenuItem;
//                        ((IEntImage)ent).FilePath = truePath;
//                        ((IEntImage)ent).SourceName = entName;
//                        ((IEntImage)ent).SourceType = uploadResult.FileExtension;
//                        ((IEntImage)ent).FileHash_MD5 = trueFileInfo.GetHash(HashTypes.MD5);
//                        ((IEntImage)ent).FileHash_SHA256 = trueFileInfo.GetHash(HashTypes.SHA256);

//                        break;
//                    }
//                case EntityTypes.File:
//                    {
//                        var entDefaults = ParentManager.EntReader.GetFileDefault();
//                        ent.IsEnabled = entDefaults.IsEnabled;
//                        ent.IsDefault = entDefaults.IsDefault;
//                        ent.IsMenuItem = entDefaults.IsMenuItem;
//                        ((IEntFile)ent).FilePath = truePath;
//                        ((IEntFile)ent).DownloadName = entName;
//                        ((IEntFile)ent).SourceName = entName;
//                        ((IEntFile)ent).SourceType = uploadResult.FileExtension;
//                        ((IEntFile)ent).FileHash_MD5 = trueFileInfo.GetHash(HashTypes.MD5);
//                        ((IEntFile)ent).FileHash_SHA256 = trueFileInfo.GetHash(HashTypes.SHA256);
//                        break;
//                    }
//                default:
//                    {
//                        throw new InvalidOperationException("Unexpected error while creating entity");
//                    }
//            }

//            ent.Save();
//        }



//    }
//}
