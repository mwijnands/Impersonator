using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XperiCode.Impersonator.Tests
{
    [TestClass]
    public class ImpersonatorTests
    {
        private const string DomainName = "";
        private const string UserName = "test";
        private const string Password = @"92~H\qh|+^9=2!N";

        private string GetFullTestUserName()
        {
            if (string.IsNullOrWhiteSpace(DomainName) || ".".Equals(DomainName))
            {
                return string.Concat(Environment.MachineName, @"\", UserName);
            }
            return string.Concat(DomainName, @"\", UserName);
        }

        [TestMethod]
        public void Should_Impersonate_With_UserName_And_Password()
        {
            string userName = GetFullTestUserName();

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
                string userName = GetFullTestUserName();

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
            string userName = GetFullTestUserName();
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
                string userName = GetFullTestUserName();

                Assert.AreEqual(userName, identity.Name, true);
            }
        }
    }
}
