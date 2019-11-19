using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.Comments.DataInterop.Test
{
    [TestClass]
    public partial class CommentReaderTests
    {
        [TestMethod]
        public void GetCommentsByPostTest()
        {
            TestGlobal.TestInit();


            long postID = 353;
            var comments = CommentReader.TryGetCommentsByPost(postID);

            Assert.IsNotNull(comments);
        }


        [TestMethod]
        public void GetCommentsByParentTest()
        {
            TestGlobal.TestInit();


            long postID = 353;
            var comments = CommentReader.TryGetCommentsByParent(postID);

            Assert.IsNotNull(comments);
        }
    }
}
