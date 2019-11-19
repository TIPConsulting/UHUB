using System;
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.Posts.DataInterop.Tests
{
    [TestClass]
    public class PostWriterTests
    {
        const int MAX_NAME_LEN = 100;
        const int MAX_CONTENT_LEN = 10000;


        [TestMethod]
        public void TryCreatePostTest1()
        {
            var cub = 1;    //CU Boulder ID


            TestGlobal.TestInit();

            var testPost = new Post_C_PublicDTO()
            {
                Name = "POST TEST " + DateTimeOffset.UtcNow,
                Content = "A1F27E0D-35E5-4323-9BAF-6C86A6E2A394",
                ParentID = cub,
                CanComment = true,
                IsPublic = true

            };

            var post = testPost.ToInternal<Post>();


            var postId = PostWriter.CreatePost(post);


            if (postId == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Post: " + postId);

        }



        [TestMethod]
        public void TryCreatePostTest2()
        {
            var cub = 1;    //CU Boulder ID


            TestGlobal.TestInit();

            StringBuilder postNameBuilder = new StringBuilder(210);
            for (int i = 0; i < 210; i++)
            {
                postNameBuilder.Append("a");
            }
            string postName = "POST TEST " + postNameBuilder.ToString();


            var testPost = new Post_C_PublicDTO()
            {
                Name = postName,
                Content = "A1F27E0D-35E5-4323-9BAF-6C86A6E2A394",
                ParentID = cub,
                CanComment = true,
                IsPublic = true
            };

            var post = testPost.ToInternal<Post>();


            long? postID = null;
            try
            {
                postID = PostWriter.CreatePost(post);
                Assert.Fail();
            }
            catch
            {
                if (postID == null)
                {
                    Console.WriteLine("Post create failure");
                }
            }

        }


        [TestMethod]
        public void TryCreatePostTest3()
        {
            TestGlobal.TestInit();
            long? postID = null;


            var club = 365;    //TEST CLUB


            var testPost = new Post_C_PublicDTO()
            {
                Name = "POST TEST " + DateTimeOffset.UtcNow,
                Content = "D8FBACBD-B069-4FC9-849C-2F1EEF2A938C",
                ParentID = club,
                CanComment = true,
                IsPublic = true

            };

            var post = testPost.ToInternal<Post>();


            postID = PostWriter.CreatePost(post);


            if (postID == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Post: " + postID);

        }


        [TestMethod]
        public void TryCreatePostTest4()
        {
            TestGlobal.TestInit();
            long? postID = null;


            var cub = 1;    //CU Boulder


            //100 chars, 100 utf-16 code points
            var str = new StringBuilder(MAX_CONTENT_LEN + 10);
            for (int i = 0; i < MAX_NAME_LEN; i++)
            {
                str.Append("a");
            }

            var codePointCount = str.ToString().Length;
            Assert.IsTrue(codePointCount == MAX_NAME_LEN);

            var charCountActual = StringInfo.ParseCombiningCharacters(str.ToString()).Length;
            Assert.IsTrue(charCountActual == MAX_NAME_LEN);


            var testPost = new Post_C_PublicDTO()
            {
                Name = str.ToString(),
                Content = "0D5C3DF4-009D-4750-9875-808A3774DB13",
                ParentID = cub,
                CanComment = true,
                IsPublic = true
            };

            var post = testPost.ToInternal<Post>();


            postID = PostWriter.CreatePost(post);



            if (postID == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Post: " + postID);
        }


        [TestMethod]
        public void TryCreatePostTest5()
        {
            TestGlobal.TestInit();
            long? postID = null;


            var cub = 1;    //CU Boulder


            //99 chars, 100 UTF-16 code points
            var str = new StringBuilder(MAX_CONTENT_LEN + 10);
            for (int i = 0; i < MAX_NAME_LEN - 2; i++)
            {
                str.Append("a");
            }
            str.Append("🙂");

            var codePointCount = str.ToString().Length;
            Assert.IsTrue(codePointCount == MAX_NAME_LEN);

            var charCountActual = StringInfo.ParseCombiningCharacters(str.ToString()).Length;
            Assert.IsTrue(charCountActual == MAX_NAME_LEN - 1);


            var testPost = new Post_C_PublicDTO()
            {
                Name = str.ToString(),
                Content = "0B5AEA7B-BC6B-4355-9031-C5E765C6B120",
                ParentID = cub,
                CanComment = true,
                IsPublic = true
            };

            var post = testPost.ToInternal<Post>();


            postID = PostWriter.CreatePost(post);



            if (postID == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Post: " + postID);
        }


        [TestMethod]
        public void TryCreatePostTest6()
        {
            TestGlobal.TestInit();
            long? postID = null;


            var cub = 1;    //CU Boulder


            //99 chars, 101 UTF-16 code points
            var str = new StringBuilder(MAX_CONTENT_LEN + 10);
            for (int i = 0; i < MAX_NAME_LEN - 3; i++)
            {
                str.Append("a");
            }
            str.Append("🙂🙂");


            var codePointCount = str.ToString().Length;
            Assert.IsTrue(codePointCount == MAX_NAME_LEN + 1);

            var charCountActual = StringInfo.ParseCombiningCharacters(str.ToString()).Length;
            Assert.IsTrue(charCountActual == MAX_NAME_LEN - 1);


            var testPost = new Post_C_PublicDTO()
            {
                Name = str.ToString(),
                Content = "0B5AEA7B-BC6B-4355-9031-C5E765C6B120",
                ParentID = cub,
                CanComment = true,
                IsPublic = true
            };

            var post = testPost.ToInternal<Post>();

            try
            {
                postID = PostWriter.CreatePost(post);
                Assert.Fail();
            }
            catch { }


        }



        [TestMethod]
        public void TryCreatePostTest7()
        {
            TestGlobal.TestInit();
            long? postID = null;


            var cub = 1;    //CU Boulder


            //2000 chars, 2000 utf-16 code points
            var str = new StringBuilder(MAX_CONTENT_LEN + 10);
            for (int i = 0; i < MAX_CONTENT_LEN; i++)
            {
                str.Append("a");
            }

            var codePointCount = str.ToString().Length;
            Assert.IsTrue(codePointCount == MAX_CONTENT_LEN);

            var charCountActual = StringInfo.ParseCombiningCharacters(str.ToString()).Length;
            Assert.IsTrue(charCountActual == MAX_CONTENT_LEN);



            var testPost = new Post_C_PublicDTO()
            {
                Name = "POST TEST",
                Content = str.ToString(),
                ParentID = cub,
                CanComment = true,
                IsPublic = true
            };

            var post = testPost.ToInternal<Post>();


            postID = PostWriter.CreatePost(post);



            if (postID == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Post: " + postID);
        }



        [TestMethod]
        public void TryCreatePostTest8()
        {
            TestGlobal.TestInit();
            long? postID = null;


            var cub = 1;    //CU Boulder



            //1999 chars, 2000 utf-16 code points
            var str = new StringBuilder(MAX_CONTENT_LEN + 10);
            for (int i = 0; i < MAX_CONTENT_LEN - 2; i++)
            {
                str.Append("a");
            }
            str.Append("🙂");


            var codePointCount = str.ToString().Length;
            Assert.IsTrue(codePointCount == MAX_CONTENT_LEN);


            var charCountActual = StringInfo.ParseCombiningCharacters(str.ToString()).Length;
            Assert.IsTrue(charCountActual == MAX_CONTENT_LEN - 1);



            var testPost = new Post_C_PublicDTO()
            {
                Name = "POST TEST",
                Content = str.ToString(),
                ParentID = cub,
                CanComment = true,
                IsPublic = true
            };

            var post = testPost.ToInternal<Post>();


            postID = PostWriter.CreatePost(post);



            if (postID == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Post: " + postID);
        }


        [TestMethod]
        public void TryCreatePostTest9()
        {
            TestGlobal.TestInit();
            long? postID = null;


            var cub = 1;    //CU Boulder



            //1999 chars, 2001 utf-16 code points
            var str = new StringBuilder(MAX_CONTENT_LEN + 10);
            for (int i = 0; i < MAX_CONTENT_LEN - 3; i++)
            {
                str.Append("a");
            }
            str.Append("🙂🙂");


            var codePointCount = str.ToString().Length;
            Assert.IsTrue(codePointCount == MAX_CONTENT_LEN + 1);

            var charCountActual = StringInfo.ParseCombiningCharacters(str.ToString()).Length;
            Assert.IsTrue(charCountActual == MAX_CONTENT_LEN - 1);




            var testPost = new Post_C_PublicDTO()
            {
                Name = "POST TEST ",
                Content = str.ToString(),
                ParentID = cub,
                CanComment = true,
                IsPublic = true
            };

            var post = testPost.ToInternal<Post>();


            try
            {
                postID = PostWriter.CreatePost(post);
                Assert.Fail();
            }
            catch { }

        }



        [TestMethod]
        public void TryCreatePostTestConfirm()
        {
            TestGlobal.TestInit();
            long? postID = null;


            var cub = 1;    //CU Boulder

            var str = new StringBuilder();
            str.AppendLine("🙂🙂");
            str.AppendLine("🙂🙂");
            str.AppendLine("🙂🙂");


            var testPost = new Post_C_PublicDTO()
            {
                Name = "POST TEST",
                Content = str.ToString(),
                ParentID = cub,
                CanComment = true,
                IsPublic = true
            };

            var post = testPost.ToInternal<Post>();



            postID = PostWriter.CreatePost(post);

            if (postID == null)
            {
                throw new Exception();
            }
            Console.WriteLine("New Post: " + postID);



            var readPost = PostReader.TryGetPost(postID.Value);

            Assert.AreEqual(str.ToString(), readPost.Content);

        }

    }
}
