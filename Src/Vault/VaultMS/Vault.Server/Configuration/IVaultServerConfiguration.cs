using Khooversoft.Security;
using Khooversoft.Services;

namespace Vault.Server
{
    public interface IVaultServerConfiguration
    {
        string HostUrls { get; }

        LocalCertificate DataObjectCertificate { get; }

        bool VerboseOnErrors { get; }

        IServerTokenManagerConfiguration ServerTokenManagerConfiguration { get; }
    }
}
