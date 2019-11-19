using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Tools;
using UHub.CoreLib.EmailInterop;
using UHub.CoreLib.EmailInterop.Templates;

namespace UHub.CoreLib.EmailInterop.Tests
{
    [TestClass]
    public class SmtpInteropTests
    {

        //[TestMethod]
        public void SendMessageTest()
        {
            TestGlobal.TestInit();


            DateTimeOffset start, end;

            var msg = new EmailMessage_ConfirmAcct($"Confirm U-HUB Account (TEST1)", "U-HUB", "aual1780@colorado.edu")
            {
                ConfirmationURL = "https://u-hub.life"
            };

            start = FailoverDateTimeOffset.UtcNow;
            var val1 = CoreFactory.Singleton.Mail.TrySendMessageAsync(msg).Result;
            end = FailoverDateTimeOffset.UtcNow;

            Console.WriteLine($"{(end - start).TotalMilliseconds}ms");

            Assert.IsTrue(val1 == EmailResultCode.Success);

        }
    }
}
