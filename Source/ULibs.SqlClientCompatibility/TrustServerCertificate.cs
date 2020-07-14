#nullable enable
using System;
/***using System.Diagnostics.CodeAnalysis;***/
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace /***$rootnamespace$.***/ULibs.SqlClientCompatibility
{
    /***[ExcludeFromCodeCoverage]***/
    internal static class TrustServerCertificate
    {
        /// <summary>
        /// <para>
        /// System.Data.SqlClient didn't verify the certificate when connecting to a SQL Server using TLS,
        /// unless the Encrypt connection property is set to true. In Microsoft.Data.SqlClient 2.0.0, this
        /// behaviour has changed to always verify the server certificate. This could be disruptive to
        /// customers, so we have decided to go for a middle ground: skip verification for on-premise SQL
        /// Servers, that are being connected to over the LAN, when Encrypt is not set.
        /// </para><para>
        /// We should revisit this in the future; as encryption becomes more commonplace and more important,
        /// even on LAN connections, verifying the server certificate should be enabled all the time. TBH,
        /// this should already be the case in 2020!
        /// </para>
        /// </summary>
        internal static bool ShouldTrustServerCertificate(bool encrypt, bool isAzureAuth, string server)
        {
            if (encrypt)
            {
                // If we've explicitly requested encryption, the certificate should be validated.
                return false;
            }

            if (isAzureAuth)
            {
                // All connection to azure should have the certificate validated.
                return false;
            }

            // If the server is on the LAN, skip verification.
            var host = server.Split('\\')[0].Trim();
            return IsHostOnLan(host);
        }

        private static bool IsHostOnLan(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return false;
            }

            if ("(local)".Equals(host, StringComparison.OrdinalIgnoreCase) || ".".Equals(host, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            try
            {
                var ipTask = Dns.GetHostAddressesAsync(host);
                // Non-existent hosts seem to take ~2s to fail, but existent hosts take a fraction of a second.
                // We need a timeout (in case it doesn't return for some reason), and 1s seems a reasonable compromise.
                if (!ipTask.Wait(TimeSpan.FromSeconds(1)))
                {
                    return false;
                }

                var ips = ipTask.Result;
                // We can't tell whether IPv6 addresses are on the LAN, so check that all the
                // IPv4 addresses are on the LAN (and that there are any IPv4 addresses, because
                // if there aren't we can't tell if it's on the LAN so have to assume it isn't)
                var isOnLan = ips.Any(IsIPv4Address) && ips.Where(IsIPv4Address).All(IsPrivateIPv4Address);
                return isOnLan;
            }
            catch
            {
                // DNS lookup failed, default to validating the certificate.
                return false;
            }
        }

        private static bool IsIPv4Address(IPAddress ip)
        {
            return ip.AddressFamily == AddressFamily.InterNetwork;
        }

        private static bool IsPrivateIPv4Address(IPAddress ip)
        {
            if (!IsIPv4Address(ip))
            {
                return false;
            }

            var bytes = ip.GetAddressBytes();
            return bytes[0] == 10 ||
                   bytes[0] == 127 ||
                   bytes[0] == 169 && bytes[1] == 254 ||
                   bytes[0] == 172 && (bytes[1] & 0xf0) == 16 ||
                   bytes[0] == 192 && bytes[1] == 168;
        }
    }
}
