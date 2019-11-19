using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.DTOs;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop.Tests
{
    [TestClass()]
    public class SchoolClubReaderTests
    {
        [TestMethod()]
        public void GetAllClubsTest()
        {
            TestGlobal.TestInit();

            var clubSet = SchoolClubReader.TryGetAllClubs().ToList();
            clubSet.ForEach(x => Console.WriteLine(x));
        }


        [TestMethod()]
        public void GetClubTest()
        {
            TestGlobal.TestInit();


            var clubSet = SchoolClubReader.TryGetAllClubs().ToList();

            if(clubSet.Count == 0)
            {
                return;
            }

            var id = clubSet.First().ID.Value;


            SchoolClubReader.TryGetClub(id);
        }



        [TestMethod()]
        public void GetClubsBySchoolTest()
        {
            TestGlobal.TestInit();

            var schoolID = 1;   //CU Boulder


            var clubSet = SchoolClubReader.TryGetClubsBySchool(schoolID).ToList();
            clubSet.ForEach(x => Console.WriteLine(x.Name));
        }



        [TestMethod()]
        public void GetClubsByEmailTest()
        {
            TestGlobal.TestInit();

            var email = "aual1780@colorado.edu";


            var clubSet= SchoolClubReader.TryGetClubsByEmail(email).ToList();
            clubSet.ForEach(x => Console.WriteLine(x.Name));
        }


        [TestMethod()]
        public void GetClubsByDomainTest()
        {
            TestGlobal.TestInit();

            var email= "aual1780@colorado.edu";   //CU Boulder
            var domain = email.Substring(email.IndexOf("@"));   //@colorado.edu


            var clubSet = SchoolClubReader.TryGetClubsByDomain(domain).ToList();
            clubSet.ForEach(x => Console.WriteLine(x.Name));
        }


        [TestMethod()]
        public void ValidateMembershipTest()
        {
            TestGlobal.TestInit();


            var userID = UserReader.GetUserID("aual1780@colorado.edu");

            if(userID == null)
            {
                return;
            }

            var clubDto = new SchoolClub_C_PublicDTO()
            {
                Name = "TEST CLUB",
                Description = "TEST CLUB"
            };

            var cmsUser = UserReader.GetUser(userID.Value);
            var club = clubDto.ToInternal<SchoolClub>();

            club.SchoolID = cmsUser.SchoolID.Value;
            club.CreatedBy = cmsUser.ID.Value;

            var clubID = SchoolClubWriter.CreateClub(club);
            Assert.IsNotNull(clubID);


            SchoolClubReader.TryValidateMembership(clubID.Value, userID.Value);

        }


        [TestMethod()]
        public void IsUserBannedTest()
        {
            TestGlobal.TestInit();


            var userID = UserReader.GetUserID("aual1780@colorado.edu");

            if (userID == null)
            {
                return;
            }

            var clubDto = new SchoolClub_C_PublicDTO()
            {
                Name = "TEST CLUB",
                Description = "TEST CLUB"
            };


            var cmsUser = UserReader.GetUser(userID.Value);
            var club = clubDto.ToInternal<SchoolClub>();

            club.SchoolID = cmsUser.SchoolID.Value;
            club.CreatedBy = cmsUser.ID.Value;

            var clubID = SchoolClubWriter.CreateClub(club);
            Assert.IsNotNull(clubID);


            SchoolClubReader.TryIsUserBanned(clubID.Value, userID.Value);

        }
    }
}