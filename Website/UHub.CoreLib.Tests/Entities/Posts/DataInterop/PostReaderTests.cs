using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Posts.DataInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Posts.DataInterop.Tests
{
    [TestClass()]
    public partial class PostReaderTests
    {
        [TestMethod()]
        public void GetAllPostsTest()
        {
            TestGlobal.TestInit();

            PostReader.TryGetAllPosts().ToList();
        }

        [TestMethod()]
        public void GetAllPostsDTOTest()
        {
            TestGlobal.TestInit();

            var postSet = PostReader.TryGetAllPosts().ToList();


            var dtoSet = postSet.Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();


            postSet = dtoSet.Select(x => x.ToInternal<Post>()).ToList();

        }


        [TestMethod()]
        public void GetPostTest()
        {
            TestGlobal.TestInit();


            var postSet = PostReader.TryGetAllPosts().ToList();

            if (postSet.Count == 0)
            {
                return;
            }

            var id = postSet.First().ID.Value;


            var start = FailoverDateTimeOffset.UtcNow;
            var end = FailoverDateTimeOffset.UtcNow;

            start = FailoverDateTimeOffset.UtcNow;
            PostReader.TryGetPost(id);
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
            start = FailoverDateTimeOffset.UtcNow;
            PostReader.TryGetPost(id);
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
        }


        [TestMethod()]
        public void GetPostsBySchoolTest()
        {
            TestGlobal.TestInit();


            var schoolSet = SchoolReader.TryGetAllSchools().ToList();

            if (schoolSet.Count == 0)
            {
                return;
            }

            var schoolId = schoolSet.First().ID.Value;


            var x = PostReader.TryGetPostsBySchool(schoolId);

            Console.WriteLine(x.Count());

        }

        [TestMethod()]
        public void GetPostsByClubTest()
        {
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.TryGetAllClubs().ToList();

            if (clubSet.Count == 0)
            {
                return;
            }

            var clubId = clubSet.First().ID.Value;


            var id = PostReader.TryGetPostsByClub(clubId);
            Assert.IsNotNull(id);

        }

        [TestMethod]
        public void GetPostsBySchoolPageTest()
        {
            //long SchoolID, long? StartID, int? PageNum, short? ItemCount
            TestGlobal.TestInit();


            var schoolId = 1;   //CU Boulder



            PostReader.TryGetPostsBySchoolPage(schoolId, null, out _);

            var start = FailoverDateTimeOffset.UtcNow;
            var end = FailoverDateTimeOffset.UtcNow;


            start = FailoverDateTimeOffset.UtcNow;
            PostReader.TryGetPostsBySchoolPage(schoolId, null, null, 5).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 0: {(end - start).TotalMilliseconds}ms");


            start = FailoverDateTimeOffset.UtcNow;
            var outSet = PostReader.TryGetPostsBySchoolPage(schoolId, null, 0, 5).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 0: {(end - start).TotalMilliseconds}ms");


            start = FailoverDateTimeOffset.UtcNow;
            var outSet2 = PostReader.TryGetPostsBySchoolPage(schoolId, null, 2100, 5).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 2000: {(end - start).TotalMilliseconds}ms");

        }


        [TestMethod]
        public void GetPostsByClubPageTest()
        {
            //long SchoolID, long? StartID, int? PageNum, short? ItemCount
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.TryGetAllClubs().ToList();

            if (clubSet.Count == 0)
            {
                return;
            }

            var schoolId = clubSet.First().ID.Value;


            PostReader.TryGetPostsByClubPage(schoolId, null, out _);

            PostReader.TryGetPostsByClubPage(schoolId, 3, null, 1);


        }

    }
}