using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Comments.DTOs;
using UHub.CoreLib.Entities.Posts;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.Comments.DataInterop.Test
{
    [TestClass]
    public partial class CommentWriterTests
    {
        [TestMethod]
        public void TryCreateCommentTest()
        {
            var cub = 1;    //CU Boulder ID


            TestGlobal.TestInit();

            var testPost = new Post_C_PublicDTO()
            {
                Name = "POST TEST " + DateTimeOffset.UtcNow,
                Content = "20041183-4FEE-430E-A417-ABDCFD05A7A9",
                ParentID = cub,
                CanComment = true,
                IsPublic = true

            };

            var post = testPost.ToInternal<Post>();
            var postId = PostWriter.CreatePost(post);

            if(postId == null)
            {
                throw new Exception();
            }

            Console.WriteLine("New Post: " + postId);

            


            var testComment = new Comment_C_PublicDTO()
            {
                Content = "E507759F-AFCF-4857-BEB4-0F62F6ED2205",
                ParentID = postId.Value
            };

            var comment = testComment.ToInternal<Comment>();
            var commentId = CommentWriter.CreateComment(comment);


            if (commentId == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Comment: " + commentId);

        }
    }
}
