using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.ApiControllers.Entities.Posts;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Entities.Posts.APIControllers.Test
{
    [TestClass]
    public class PostReaderAPITests
    {
        [TestMethod]
        public async Task GetPostCountBySchoolTest()
        {
            TestGlobal.TestInit();

            var controller = await TestGlobal.GetAuthRequest(new PostController(), true);

            var response = await controller.GetPostCountBySchool();
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<long>;
            Assert.IsNotNull(result);
            Console.WriteLine(result.Content);

        }


        [TestMethod]
        public async Task GetPageCountBySchoolTest()
        {
            TestGlobal.TestInit();

            var controller = await TestGlobal.GetAuthRequest(new PostController(), true);

            var response = await controller.GetPageCountBySchool();
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<long>;
            Assert.IsNotNull(result);

        }


        [TestMethod]
        public async Task GetPageBySchoolTest()
        {
            TestGlobal.TestInit();

            var controller = await TestGlobal.GetAuthRequest(new PostController(), true);


            dynamic response = await controller.GetPageBySchool();
            Assert.IsNotNull(response);

            dynamic result = response.Content;
            Assert.IsNotNull(result);

        }



        [TestMethod]
        public async Task GetPostCountByClubTest()
        {
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.TryGetClubsByDomain("@colorado.edu").ToList();
            var clubID = clubSet.FirstOrDefault()?.ID;

            if (clubID == null)
            {
                return;
            }


            var controller = await TestGlobal.GetAuthRequest(new PostController(), true);

            var response = await controller.GetPostCountByClub(clubID.Value);
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<long>;
            Assert.IsNotNull(result);

        }


        [TestMethod]
        public async Task GetPageCountByClubTest()
        {
            TestGlobal.TestInit();

            var clubSet = SchoolClubReader.TryGetClubsByDomain("@colorado.edu").ToList();
            var clubID = clubSet.FirstOrDefault()?.ID;

            if (clubID == null)
            {
                return;
            }

            var controller = await TestGlobal.GetAuthRequest(new PostController(), true);

            var response = await controller.GetPageCountByClub(clubID.Value);
            Assert.IsNotNull(response);


            var result = response as OkNegotiatedContentResult<long>;
            Assert.IsNotNull(result);

        }


        [TestMethod]
        public async Task GetPageByClubTest()
        {
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.TryGetClubsByDomain("@colorado.edu").ToList();
            var clubID = clubSet.FirstOrDefault()?.ID;

            if (clubID == null)
            {
                return;
            }

            var controller = await TestGlobal.GetAuthRequest(new PostController(), true);

            dynamic response = await controller.GetPageByClub(clubID.Value);
            Assert.IsNotNull(response);

            dynamic result = response.Content;
            Assert.IsNotNull(result);

        }

    }
}
