using Khooversoft.Security;
using Khooversoft.Services;
using Khooversoft.Sql;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Vault.Server;

namespace Vault.Configuration
{
    public class LocalServerConfiguration : IVaultServerConfiguration
    {
        private const string _testClientIssuerName = "testClient.issuer@domain.com";
        private const string _testClientSubjectName = "testClient@domain.com";
        private const string _testServerName = "testAuthority@domain.com";

        public SqlConfiguration VaultRepository { get; } = new SqlConfiguration("localhost", "Vault_Local");

        public string HostUrls { get; } = "http://*:8080;http://localhost:8080;http://hostname:8080";

        public LocalCertificate DataObjectCertificate { get; } = new LocalCertificate(
            StoreLocation.CurrentUser,
            StoreName.My,
            "A4A261513C973CCCC13ABA45B2062484F71CE32F",
            true);

        public bool VerboseOnErrors { get; } = true;

        public IServerTokenManagerConfiguration ServerTokenManagerConfiguration { get; } = new ServerTokenManagerConfiguration
        {
            ValidIssuers = new string[] { _testClientIssuerName },

            TokenAuthorizationRequestCertificateKeys = new List<LocalCertificateKey>
            {
                new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true),
            },

            TokenAuthorization = new TokenAuthorizationConfiguration(
                new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true),
                TimeSpan.FromDays(1),
                "testAuthorityIssuer@domain.comn"),

            HmacConfiguration = new HmacConfiguration(),
        };
    }
}
