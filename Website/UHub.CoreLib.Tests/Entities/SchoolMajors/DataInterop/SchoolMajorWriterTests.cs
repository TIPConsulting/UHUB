using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.SchoolMajors;

namespace UHub.CoreLib.Tests.Entities.SchoolMajors.Management
{
    [TestClass]
    public class SchoolMajorWriterTests
    {
        [TestMethod()]
        public async Task CreateMajorTest()
        {
            TestGlobal.TestInit();


            var newMajor = new SchoolMajor()
            {
                Name = "ASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDASDAS"
            };


            var t = await CoreLib.Entities.SchoolMajors.Management.SchoolMajorManager.TryCreateMajorAsync(newMajor, 1);

            Console.WriteLine(t);
        }
    }
}
