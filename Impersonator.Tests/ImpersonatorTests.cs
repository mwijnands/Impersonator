using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XperiCode.Impersonator.Tests
{
    [TestClass]
    public class ImpersonatorTests
    {
        private const string DomainName = "domainname";
        private const string UserName = "username";
        private const string Password = "password";

        [TestMethod]
        public void Should_Impersonate_With_UserName_And_Password()
        {
            string userName = string.Concat(DomainName, "\\", UserName);

            using (new Impersonator(userName, Password))
            {
                var currentIdentity = WindowsIdentity.GetCurrent();

                Assert.AreEqual(userName, currentIdentity.Name, true);
            }
        }

        [TestMethod]
        public void Should_Impersonate_With_DomainName_UserName_And_Password()
        {
            using (new Impersonator(DomainName, UserName, Password))
            {
                var currentIdentity = WindowsIdentity.GetCurrent();
                string userName = string.Concat(DomainName, "\\", UserName);

                Assert.AreEqual(userName, currentIdentity.Name, true);
            }
        }

        [TestMethod]
        public void Should_Impersonate_With_Identity()
        {
            using (var identity = Impersonator.GetIdentity(DomainName, UserName, Password))
            {
                using (new Impersonator(identity))
                {
                    var currentIdentity = WindowsIdentity.GetCurrent();

                    Assert.AreEqual(identity.Name, currentIdentity.Name, true);
                }
            }
        }

        [TestMethod]
        public void Should_Get_Identity_With_UserName_And_Password()
        {
            string userName = string.Concat(DomainName, "\\", UserName);
            using (var identity = Impersonator.GetIdentity(userName, Password))
            {
                Assert.AreEqual(userName, identity.Name, true);
            }
        }

        [TestMethod]
        public void Should_Get_Identity_With_DomainName_UserName_And_Password()
        {
            using (var identity = Impersonator.GetIdentity(DomainName, UserName, Password))
            {
                string userName = string.Concat(DomainName, "\\", UserName);

                Assert.AreEqual(userName, identity.Name, true);
            }
        }
    }
}
