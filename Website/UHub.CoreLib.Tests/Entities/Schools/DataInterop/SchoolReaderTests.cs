using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Tests;

namespace UHub.CoreLib.Entities.Schools.DataInterop.Tests
{
    [TestClass]
    public class SchoolReaderTests
    {
        [TestMethod]
        public void GetAllSchoolsTest()
        {

            TestGlobal.TestInit();

            SchoolReader.TryGetAllSchools();

        }


        [TestMethod]
        public void GetSchoolTest()
        {

            TestGlobal.TestInit();

            var schoolSet = SchoolReader.TryGetAllSchools().ToList();


            if(schoolSet.Count == 0)
            {
                return;
            }

            var id = schoolSet.First().ID.Value;


            SchoolReader.TryGetSchool(id);
        }


        [TestMethod]
        public void GetSchoolByNameTest()
        {

            TestGlobal.TestInit();


            School school;
            school = SchoolReader.TryGetSchoolByName("CU Boulder");
            Assert.IsNotNull(school);


            school = SchoolReader.TryGetSchoolByName("hu98fp2qghp9jfkiwe");
            Assert.IsNull(school);


        }


        [TestMethod]
        public void GetSchoolByEmailTest()
        {

            TestGlobal.TestInit();


            School school;
            school = SchoolReader.TryGetSchoolByEmail("aual1780@colorado.edu");
            Assert.IsNotNull(school);


            school = SchoolReader.TryGetSchoolByEmail("test@colorado.edu");
            Assert.IsNotNull(school);


            school = SchoolReader.TryGetSchoolByEmail("test@test.test");
            Assert.IsNull(school);

        }


        [TestMethod]
        public void GetSchoolByDomainTest()
        {

            TestGlobal.TestInit();


            School school;
            school = SchoolReader.TryGetSchoolByEmail("@colorado.edu");
            Assert.IsNotNull(school);


            school = SchoolReader.TryGetSchoolByEmail("@test.test");
            Assert.IsNull(school);

        }


        [TestMethod]
        public void IsEmailValidTest()
        {

            TestGlobal.TestInit();


            bool isValid;
            isValid = SchoolReader.TryIsEmailValid("");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsEmailValid("asdafq");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsEmailValid("a");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsEmailValid("@colorado.edu");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsEmailValid("test@test.test");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsEmailValid("aual1780@colorado.edu");
            Assert.IsTrue(isValid);

        }


        [TestMethod]
        public void IsDomainValidTest()
        {

            TestGlobal.TestInit();


            bool isValid;
            isValid = SchoolReader.TryIsDomainValid("");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsDomainValid("asdafq");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsDomainValid("a");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsDomainValid("test@test.test");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsDomainValid("@test.test");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsDomainValid("aual1780@colorado.edu");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsDomainValid("colorado.edu");
            Assert.IsFalse(isValid);


            isValid = SchoolReader.TryIsDomainValid("@colorado.edu");
            Assert.IsTrue(isValid);

        }
    }
}
