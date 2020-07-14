using NUnit.Framework;
using ULibs.SqlClientCompatibility;

namespace Ulibs.Tests.SqlClientCompatibility
{
    [TestFixture]
    public class TrustServerCertificateTests
    {
        // Don't trust: client requested encryption
        [TestCase(true, false, "(local)", ExpectedResult = false)]
        // Don't trust: azure auth
        [TestCase(false, true, "(local)", ExpectedResult = false)]
        // Don't trust: not on LAN
        [TestCase(false, false, "8.8.8.8", ExpectedResult = false)]
        [TestCase(false, false, "8.8.8.8\\sql2014", ExpectedResult = false)]
        [TestCase(false, false, "example.com", ExpectedResult = false)]
        [TestCase(false, false, "example.com\\sql2014", ExpectedResult = false)]
        // Do trust: on LAN
        [TestCase(false, false, "localhost", ExpectedResult = true)]
        [TestCase(false, false, "localhost\\sql2014", ExpectedResult = true)]
        [TestCase(false, false, "(local)", ExpectedResult = true)]
        [TestCase(false, false, "(local)\\sql2014", ExpectedResult = true)]
        [TestCase(false, false, ".", ExpectedResult = true)]
        [TestCase(false, false, ".\\sql2014", ExpectedResult = true)]
        [TestCase(false, false, "192.168.1.120", ExpectedResult = true)]
        [TestCase(false, false, "192.168.1.120\\sql2014", ExpectedResult = true)]
        public bool ShouldTrustServerCertificate(bool encrypt, bool isAzureAuth, string server)
        {
            return TrustServerCertificate.ShouldTrustServerCertificate(encrypt, isAzureAuth, server);
        }
    }
}
