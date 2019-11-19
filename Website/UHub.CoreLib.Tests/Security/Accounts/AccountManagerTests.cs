using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Entities.Users.DataInterop;

namespace UHub.CoreLib.Security.Accounts.Tests
{
    [TestClass()]
    public class AccountControllerTests
    {
        [TestMethod()]
        public void CreateUserTest()
        {
            TestGlobal.TestInit();

            string status;

            //INVALID DOMAIN
            var testUser = new User_C_PublicDTO()
            {
                Email = "test@test.test",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser);
            Assert.AreEqual("Email Domain Not Supported", status);


            //INVALID COMPANY LENGTH
            testUser = new User_C_PublicDTO()
            {
                Email = "test@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
                Company = "12312312312312312312312312312313123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123"
            };
            status = CreateUserTestWorker(testUser);
            Assert.AreEqual("Account Creation Failed", status);


            //EMPTY EMAIL
            testUser = new User_C_PublicDTO()
            {
                Email = "",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser);
            Assert.AreEqual("Email Empty", status);


            //DUPLICATE EMAIL
            testUser = new User_C_PublicDTO()
            {
                Email = "aual1780@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser);
            Assert.AreEqual("Email Duplicate", status);


            //EMPTY PASSWORD
            testUser = new User_C_PublicDTO()
            {
                Email = "TEST123456@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = "",
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser);
            Assert.AreEqual("Password Empty", status);


            //INVALID PASSWORD
            testUser = new User_C_PublicDTO()
            {
                Email = "TEST123456@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = "1234",
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser);
            Assert.AreEqual("Password Invalid", status);


            //INVALID MAJOR
            testUser = new User_C_PublicDTO()
            {
                Email = "TEST123456@colorado.edu",
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Sciences",
            };
            status = CreateUserTestWorker(testUser);
            Assert.AreEqual("Major Invalid", status);


            //VALID
            var email = Guid.NewGuid().ToString() + "@colorado.edu";
            testUser = new User_C_PublicDTO()
            {
                Email = email,
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
                Name = "Bob",
                Major = "Computer Science",
            };
            status = CreateUserTestWorker(testUser);
            Assert.AreEqual("User Created", status);


            UserWriter.TryPurgeUser(email);

        }

        private string CreateUserTestWorker(User_C_PublicDTO testUser)
        {
            var tmpUser = testUser.ToInternal<User>();


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            string status = "Account Creation Failed";



            var result = CoreFactory.Singleton.Accounts.TryCreateUser(
                tmpUser,
                true,
                SuccessHandler: (newUser, canLogin) =>
                {
                    status = "User Created";
                });

            if (result == AcctCreateResultCode.UnknownError)
            {
                return result.ToString();
            }


            if (result != AcctCreateResultCode.Success && enableDetail)
            {
                switch (result)
                {
                    case AcctCreateResultCode.EmailEmpty: { status = "Email Empty"; break; }
                    case AcctCreateResultCode.EmailInvalid: { status = "Email Invalid"; break; }
                    case AcctCreateResultCode.EmailDuplicate: { status = "Email Duplicate"; break; }
                    case AcctCreateResultCode.EmailDomainInvalid: { status = "Email Domain Not Supported"; break; }
                    case AcctCreateResultCode.UsernameDuplicate: { status = "Username Duplicate"; break; }
                    case AcctCreateResultCode.MajorInvalid: { status = "Major Invalid"; break; }
                    case AcctCreateResultCode.PswdEmpty: { status = "Password Empty"; break; }
                    case AcctCreateResultCode.PswdInvalid: { status = "Password Invalid"; break; }
                }

                Console.WriteLine(result.ToString());
            }


            return status;
        }
    }
}