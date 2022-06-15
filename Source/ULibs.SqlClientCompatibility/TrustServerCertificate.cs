#nullable enable
using System;
using System.Data.Common;
using System.Globalization;
/***using System.Diagnostics.CodeAnalysis;***/
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Data.SqlClient;
#if SMARTASSEMBLY
using SmartAssembly.Attributes;
#endif

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
#if SMARTASSEMBLY
[DoNotCaptureVariables]
#endif
        internal static void SetBackwardsCompatibleTrustServerCertificateValue(this DbConnectionStringBuilder builder)
        {
            // SqlConnectionStringBuilder overrides ContainsKey and [] so we always get back true
            // and the default value, so we can't tell whether keys were explicitly specified. By
            // inspecting the connection string itself, we can tell whether key are actually set.
            var cleanBuilder = new DbConnectionStringBuilder { ConnectionString = builder.ConnectionString };
            var encrypt = cleanBuilder.EncryptIsSet();
            var isAzureAuth = cleanBuilder.IsAzureAuth();
            var server = cleanBuilder.GetServer();

            if (ShouldTrustServerCertificate(encrypt, isAzureAuth, server))
            {
                if (cleanBuilder.ContainsKey("Trust Server Certificate") ||
                    cleanBuilder.ContainsKey("trustservercertificate"))
                {
                    // The connection string specified it explicitly; don't override
                    return;
                }

                builder["Trust Server Certificate"] = "true";
            }
        }

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
#if SMARTASSEMBLY
[DoNotCaptureVariables]
#endif
        internal static string SetBackwardsCompatibleTrustServerCertificateValue(this string connectionString)
        {
            var builder = new DbConnectionStringBuilder {ConnectionString = connectionString};
            builder.SetBackwardsCompatibleTrustServerCertificateValue();
            return builder.ConnectionString;
        }

#if SMARTASSEMBLY
[DoNotCaptureVariables]
#endif
        private static bool EncryptIsSet(this DbConnectionStringBuilder builder)
        {
            return builder.ContainsKey("Encrypt") && ConvertToBoolean(builder["Encrypt"]);
        }

        // Copied from DbConnectionStringBuilderUtil.ConvertToBoolean in Microsoft.Data.SqlClient
        private static bool ConvertToBoolean(object value)
        {
            if (value is string svalue)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(svalue, "true") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "yes"))
                    return true;
                else if (StringComparer.OrdinalIgnoreCase.Equals(svalue, "false") || StringComparer.OrdinalIgnoreCase.Equals(svalue, "no"))
                    return false;
                else
                {
                    string tmp = svalue.Trim();  // Remove leading & trailing white space.
                    if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "true") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "yes"))
                        return true;
                    else if (StringComparer.OrdinalIgnoreCase.Equals(tmp, "false") || StringComparer.OrdinalIgnoreCase.Equals(tmp, "no"))
                        return false;
                }
                return Boolean.Parse(svalue);
            }

            return ((IConvertible) value).ToBoolean(CultureInfo.InvariantCulture);
        }

#if SMARTASSEMBLY
[DoNotCaptureVariables]
#endif
        private static bool IsAzureAuth(this DbConnectionStringBuilder builder)
        {
            return builder.ContainsKey("Authentication") && IsAzureAuth(builder["Authentication"]);
        }

        private static bool IsAzureAuth(object authentication)
        {
            if (authentication == null) return false;
            var authString = authentication.ToString();
            if (authString == null) return false;
            var withSpace = authString.IndexOf("active directory", StringComparison.OrdinalIgnoreCase);
            var withoutSpace = authString.IndexOf("activedirectory", StringComparison.OrdinalIgnoreCase);
            return withSpace >= 0 || withoutSpace >= 0;
        }

#if SMARTASSEMBLY
[DoNotCaptureVariables]
#endif
        private static string? GetServer(this DbConnectionStringBuilder builder)
        {
            if (builder.TryGetValue("Data Source", out var ds) && ds is string dss) return dss;
            if (builder.TryGetValue("server", out var s) && s is string ss) return ss;
            if (builder.TryGetValue("addr", out var a) && a is string @as) return @as;
            if (builder.TryGetValue("address", out var ad) && ad is string ads) return ads;
            if (builder.TryGetValue("network address", out var nad) && nad is string nads) return nads;
            return null;
        }

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
        internal static bool ShouldTrustServerCertificate(bool encrypt, bool isAzureAuth, string? server)
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

            if (server == null)
            {
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
