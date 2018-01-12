using Khooversoft.Security;
using Khooversoft.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Vault.Client;

namespace Vault.Configuration
{
    public class LocalClientConfiguration : IVaultClientConfiguration
    {
        public LocalCertificateKey RequestSigningCertificateKey { get; } = new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true);

        public string RequestingIssuer { get; } = "testClient.issuer@domain.com";

        public Uri AuthorizationUri { get; } = new Uri("HTTP://localhost:8080");

        public IEnumerable<LocalCertificateKey> ServerSigningCertificateKeys { get; } = new List<LocalCertificateKey>
        {
            new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true),
        };

        public Uri ClientBaseUrl { get; } = new Uri("HTTP://localhost:8080");

        public IEnumerable<string> HmacHeaders { get; } = Enumerable.Empty<string>();

        public TokenKey TokenKey { get; } = new TokenKey("testAuthority@domain.com", "testClient@domain.com");

        public IHmacConfiguration HmacConfiguration { get; } = new HmacConfiguration();
    }
}

