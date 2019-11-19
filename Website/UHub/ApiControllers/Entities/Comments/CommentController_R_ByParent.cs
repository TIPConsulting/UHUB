using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Comments.DTOs;
using UHub.CoreLib.Entities.Comments.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.ApiControllers.Entities.Comments
{
    public sealed partial class CommentController
    {
        //[HttpPost()]
        //[Route("GetByParent")]
        //[ApiAuthControl]
        //public async Task<IHttpActionResult> GetByParent(long ParentID)
        //{
        //    string status = "";
        //    HttpStatusCode statCode = HttpStatusCode.BadRequest;
        //    if (!this.ValidateSystemState(out status, out statCode))
        //    {
        //        return Content(statCode, status);
        //    }



        //    var comments = await CommentReader.TryGetCommentsByParentAsync(ParentID);
        //    if(comments == null)
        //    {
        //        return InternalServerError();
        //    }


        //    var outSet = comments.Select(x => x.ToDto<Comment_R_PublicDTO>());
        //    return Ok(outSet);
        //}
    }
}
