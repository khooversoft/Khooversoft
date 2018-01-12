using Khooversoft.Security;
using Khooversoft.Services;
using Khooversoft.Sql;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Vault.Server;

namespace Vault.Configuration
{
    public class TestServerConfiguration : IVaultServerConfiguration
    {
        public SqlConfiguration VaultRepository { get; } = new SqlConfiguration("localhost", "Vault_Test");

        public string HostUrls { get; } = "http://*:8080;http://localhost:8080;http://hostname:8080";

        public LocalCertificate DataObjectCertificate { get; } = new LocalCertificate(StoreLocation.CurrentUser, StoreName.My, "A4A261513C973CCCC13ABA45B2062484F71CE32F", true);

        public bool VerboseOnErrors { get; } = true;

        public IServerTokenManagerConfiguration ServerTokenManagerConfiguration { get; } = new ServerTokenManagerConfiguration
        {
            ValidIssuers = new string[] { "testClient.issuer@domain.com" },

            TokenAuthorizationRequestCertificateKeys = new List<LocalCertificateKey>
            {
                new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true),
            },

            TokenAuthorization = new TokenAuthorizationConfiguration(
                new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true),
                TimeSpan.FromDays(1),
                "testAuthority.Issuer@domain.com"),

            HmacConfiguration = new HmacConfiguration(),
        };
    }
}
