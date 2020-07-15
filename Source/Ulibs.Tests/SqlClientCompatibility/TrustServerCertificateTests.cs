using System.Data.Common;
using Microsoft.Data.SqlClient;
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

        // Don't trust: client requested encryption
        [TestCase(true, SqlAuthenticationMethod.NotSpecified, "(local)", ExpectedResult = false)]
        [TestCase(true, SqlAuthenticationMethod.SqlPassword, "(local)", ExpectedResult = false)]
        // Don't trust: azure auth
        [TestCase(false, SqlAuthenticationMethod.ActiveDirectoryIntegrated, "(local)", ExpectedResult = false)]
        [TestCase(false, SqlAuthenticationMethod.ActiveDirectoryInteractive, "(local)", ExpectedResult = false)]
        [TestCase(false, SqlAuthenticationMethod.ActiveDirectoryPassword, "(local)", ExpectedResult = false)]
        [TestCase(false, SqlAuthenticationMethod.ActiveDirectoryServicePrincipal, "(local)", ExpectedResult = false)]
        // Don't trust: not on LAN
        [TestCase(false, SqlAuthenticationMethod.NotSpecified, "example.com", ExpectedResult = false)]
        [TestCase(false, SqlAuthenticationMethod.SqlPassword, "example.com", ExpectedResult = false)]
        // Do trust: on LAN
        [TestCase(false, SqlAuthenticationMethod.NotSpecified, "(local)", ExpectedResult = true)]
        [TestCase(false, SqlAuthenticationMethod.SqlPassword, "(local)", ExpectedResult = true)]
        public bool AddShouldTrustServerCertificateToSqlConnectionStringBuilder(bool encrypt, SqlAuthenticationMethod auth, string server)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                Encrypt = encrypt,
                Authentication = auth
            };
            builder.AddTrustServerCertificateForCompatibility();
            return builder.TrustServerCertificate;
        }

        // Don't trust: client requested encryption
        [TestCase(true, null, "(local)", ExpectedResult = false)]
        [TestCase(true, nameof(SqlAuthenticationMethod.SqlPassword), "(local)", ExpectedResult = false)]
        [TestCase(true, "Sql Password", "(local)", ExpectedResult = false)]
        // Don't trust: azure auth
        [TestCase(false, nameof(SqlAuthenticationMethod.ActiveDirectoryIntegrated), "(local)", ExpectedResult = false)]
        [TestCase(false, nameof(SqlAuthenticationMethod.ActiveDirectoryInteractive), "(local)", ExpectedResult = false)]
        [TestCase(false, nameof(SqlAuthenticationMethod.ActiveDirectoryPassword), "(local)", ExpectedResult = false)]
        [TestCase(false, nameof(SqlAuthenticationMethod.ActiveDirectoryServicePrincipal), "(local)", ExpectedResult = false)]
        [TestCase(false, "Active Directory Integrated", "(local)", ExpectedResult = false)]
        [TestCase(false, "Active Directory Interactive", "(local)", ExpectedResult = false)]
        [TestCase(false, "Active Directory Password", "(local)", ExpectedResult = false)]
        [TestCase(false, "Active Directory Service Principal", "(local)", ExpectedResult = false)]
        // Don't trust: not on LAN
        [TestCase(false, null, "example.com", ExpectedResult = false)]
        [TestCase(false, nameof(SqlAuthenticationMethod.SqlPassword), "example.com", ExpectedResult = false)]
        [TestCase(false, "Sql Password", "example.com", ExpectedResult = false)]
        // Do trust: on LAN
        [TestCase(false, null, "(local)", ExpectedResult = true)]
        [TestCase(false, nameof(SqlAuthenticationMethod.SqlPassword), "(local)", ExpectedResult = true)]
        [TestCase(false, "Sql Password", "(local)", ExpectedResult = true)]
        public bool AddShouldTrustServerCertificateToDbConnectionStringBuilder(bool encrypt, string auth, string server)
        {
            var builder = new DbConnectionStringBuilder
            {
                ["Server"] = server,
            };

            if (encrypt)
            {
                builder["Encrypt"] = "true";
            }

            if (auth != null)
            {
                builder["Authentication"] = auth;
            }

            builder.AddTrustServerCertificateForCompatibility();
            return new SqlConnectionStringBuilder(builder.ConnectionString).TrustServerCertificate;
        }

        [TestCase("Data Source=localhost", "Data Source=localhost;Trust Server Certificate=true")]
        [TestCase("Server=localhost", "Server=localhost;Trust Server Certificate=true")]
        [TestCase("Addr=localhost", "Addr=localhost;Trust Server Certificate=true")]
        [TestCase("Address=localhost", "Address=localhost;Trust Server Certificate=true")]
        [TestCase("Network Address=localhost", "Network Address=localhost;Trust Server Certificate=true")]
        [TestCase("Server=localhost;encrypt=yes", "Server=localhost;encrypt=yes")]
        public void AddTrustServerCertificateForCompatibility(string input, string expected)
        {
            var actual = input.AddTrustServerCertificateForCompatibility();
            Assert.That(actual, Is.EqualTo(expected).IgnoreCase);
        }
    }
}
