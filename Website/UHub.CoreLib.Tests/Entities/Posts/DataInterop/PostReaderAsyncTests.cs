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
    public partial class PostReaderTests
    {

        [TestMethod()]
        public async Task GetAllPostsAsyncTest()
        {
            TestGlobal.TestInit();

            (await PostReader.TryGetAllPostsAsync()).ToList();
        }



        [TestMethod()]
        public async Task GetPostAsyncTest()
        {
            TestGlobal.TestInit();


            var postSet = (await PostReader.TryGetAllPostsAsync()).ToList();

            if (postSet.Count == 0)
            {
                return;
            }

            var id = postSet.First().ID.Value;


            var start = FailoverDateTimeOffset.UtcNow;
            var end = FailoverDateTimeOffset.UtcNow;

            start = FailoverDateTimeOffset.UtcNow;
            await PostReader.TryGetPostAsync(id);
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
            start = FailoverDateTimeOffset.UtcNow;
            await PostReader.TryGetPostAsync(id);
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");
        }


        [TestMethod()]
        public async Task GetPostsBySchoolAsyncTest()
        {
            TestGlobal.TestInit();


            var schoolSet = (await SchoolReader.TryGetAllSchoolsAsync()).ToList();

            if (schoolSet.Count == 0)
            {
                return;
            }

            var schoolId = schoolSet.First().ID.Value;


            var x = await PostReader.TryGetPostsBySchoolAsync(schoolId);

            Console.WriteLine(x.Count());

        }

        [TestMethod()]
        public async Task GetPostsByClubAsyncTest()
        {
            TestGlobal.TestInit();


            var clubSet = (await SchoolClubReader.TryGetAllClubsAsync()).ToList();

            if (clubSet.Count == 0)
            {
                return;
            }

            var clubId = clubSet.First().ID.Value;


            var id = PostReader.TryGetPostsByClubAsync(clubId);
            Assert.IsNotNull(id);

        }

        [TestMethod]
        public async Task GetPostsBySchoolPageAsyncTest()
        {
            //long SchoolID, long? StartID, int? PageNum, short? ItemCount
            TestGlobal.TestInit();


            var schoolId = 1;   //CU Boulder



            await PostReader.TryGetPostsBySchoolPageAsync(schoolId, null);

            var start = FailoverDateTimeOffset.UtcNow;
            var end = FailoverDateTimeOffset.UtcNow;


            start = FailoverDateTimeOffset.UtcNow;
            (await PostReader.TryGetPostsBySchoolPageAsync(schoolId, null, null, 5)).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 0: {(end - start).TotalMilliseconds}ms");


            start = FailoverDateTimeOffset.UtcNow;
            var outSet = (await PostReader.TryGetPostsBySchoolPageAsync(schoolId, null, 0, 5)).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 0: {(end - start).TotalMilliseconds}ms");


            start = FailoverDateTimeOffset.UtcNow;
            var outSet2 = (await PostReader.TryGetPostsBySchoolPageAsync(schoolId, null, 2100, 5)).ToList();
            end = FailoverDateTimeOffset.UtcNow;
            Console.WriteLine($"Page 2000: {(end - start).TotalMilliseconds}ms");

        }


        [TestMethod]
        public async Task GetPostsByClubPageAsyncTest()
        {
            //long SchoolID, long? StartID, int? PageNum, short? ItemCount
            TestGlobal.TestInit();


            var clubSet = (await SchoolClubReader.TryGetAllClubsAsync()).ToList();

            if (clubSet.Count == 0)
            {
                return;
            }

            var schoolId = clubSet.First().ID.Value;


            await PostReader.TryGetPostsByClubPageAsync(schoolId, null);

            await PostReader.TryGetPostsByClubPageAsync(schoolId, 3, null, 1);


        }


    }
}