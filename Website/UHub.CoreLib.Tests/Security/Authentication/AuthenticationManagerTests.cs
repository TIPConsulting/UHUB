using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Security.Accounts;
using UHub.CoreLib.Security.Authentication.APIControllers;

namespace UHub.CoreLib.Security.Authentication.Tests
{
    [TestClass()]
    public class AuthenticationManagerTests
    {
        [TestMethod()]
        public void GetTokenTest()
        {
            TestGlobal.TestInit();


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            string status = "Login Failed";

            //LOGIN INVALID
            var email = "aual1780@colorado.edu";
            var password = "TEST";
            var persistent = false;


            var tokenResult = CoreFactory.Singleton.Auth.TryGetClientAuthToken(
                    email,
                    password,
                    persistent);

            var token = tokenResult.AuthToken;
            var resultCode = tokenResult.ResultCode;


            if (resultCode == AuthResultCode.UnknownError)
            {
                Assert.Fail();
            }


            if (enableDetail)
            {
                switch (resultCode)
                {
                    case AuthResultCode.EmailEmpty: { status = "Email Empty"; break; }
                    case AuthResultCode.EmailInvalid: { status = "Email Invalid"; break; }
                    case AuthResultCode.PswdEmpty: { status = "Password Empty"; break; }
                    case AuthResultCode.UserInvalid: { status = "Account Invalid"; break; }
                    case AuthResultCode.UserLocked: { status = "Account Locked"; break; }
                    case AuthResultCode.PendingApproval: { status = "Account Pending Approval"; break; }
                    case AuthResultCode.PendingConfirmation: { status = "Account Pending Confirmation"; break; }
                    case AuthResultCode.UserDisabled: { status = "Account Disabled"; break; }
                    case AuthResultCode.PswdExpired: { status = "Password Expired"; break; }
                    case AuthResultCode.CredentialsInvalid: { status = "Credentials Invalid"; break; }
                    case AuthResultCode.Success: { status = "Unknown Error"; break; }
                }
            }

            Console.WriteLine(status);
            if (token.Length > 50)
            {
                Assert.Fail();
            }
        }


        [TestMethod()]
        public void GetTokenTest2()
        {
            TestGlobal.TestInit();


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            string status = "Login Failed";

            //LOGIN VALID
            var email = "aual1780@colorado.edu";
            var password = "testtest";
            var persistent = false;
            var isFailed = false;

            var tokenResult = CoreFactory.Singleton.Auth.TryGetClientAuthToken(
                    email,
                    password,
                    persistent);


            var token = tokenResult.AuthToken;
            var resultCode = tokenResult.ResultCode;

            if (resultCode == AuthResultCode.UnknownError)
            {
                Assert.Fail();
            }


            if (enableDetail)
            {
                switch (resultCode)
                {
                    case AuthResultCode.EmailEmpty: { status = "Email Empty"; break; }
                    case AuthResultCode.EmailInvalid: { status = "Email Invalid"; break; }
                    case AuthResultCode.PswdEmpty: { status = "Password Empty"; break; }
                    case AuthResultCode.UserInvalid: { status = "Account Invalid"; break; }
                    case AuthResultCode.UserLocked: { status = "Account Locked"; break; }
                    case AuthResultCode.PendingApproval: { status = "Account Pending Approval"; break; }
                    case AuthResultCode.PendingConfirmation: { status = "Account Pending Confirmation"; break; }
                    case AuthResultCode.UserDisabled: { status = "Account Disabled"; break; }
                    case AuthResultCode.PswdExpired: { status = "Password Expired"; break; }
                    case AuthResultCode.CredentialsInvalid: { status = "Credentials Invalid"; break; }
                    case AuthResultCode.Success: { status = "Success"; break; }
                }
            }

            if(isFailed)
            {
                Assert.Fail();
            }

            Console.WriteLine(status);
            Assert.AreEqual("Success", status);
        }
    }
}