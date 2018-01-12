using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Services;
using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Vault.Client
{
    public class VaultClient : IVaultClient
    {
        private readonly ITokenClientRepository _tokenClientRepository;
        private readonly IVaultClientConfiguration _vaultClientConfiguration;
        private readonly IRestClientConfiguration _restClientConfiguration;

        public VaultClient(ITokenClientRepository tokenClientRepository, IVaultClientConfiguration vaultClientConfiguration, IRestClientConfiguration restClientConfiguration)
        {
            Verify.IsNotNull(nameof(tokenClientRepository), tokenClientRepository);
            Verify.IsNotNull(nameof(vaultClientConfiguration), vaultClientConfiguration);
            Verify.IsNotNull(nameof(restClientConfiguration), restClientConfiguration);

            _tokenClientRepository = tokenClientRepository;
            _vaultClientConfiguration = vaultClientConfiguration;

            var hmacClient = new HmacClient(_vaultClientConfiguration.HmacConfiguration, _vaultClientConfiguration.TokenKey.RequestingSubject, x => GetApiKey(x));

            _restClientConfiguration = restClientConfiguration
                .WithProperty(hmacClient);

            Management = new ManagementClient(_vaultClientConfiguration, _restClientConfiguration);
            Group = new GroupClient(_vaultClientConfiguration, _restClientConfiguration);
            Secret = new SecretClient(_vaultClientConfiguration, _restClientConfiguration);
        }

        /// <summary>
        /// Management client
        /// </summary>
        public ManagementClient Management { get; }

        /// <summary>
        /// Group client
        /// </summary>
        public GroupClient Group { get; }

        /// <summary>
        /// Secret client
        /// </summary>
        public SecretClient Secret { get; }

        /// <summary>
        /// Get token for HMAC
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>token</returns>
        private async Task<string> GetApiKey(IWorkContext context)
        {
            return await _tokenClientRepository.GetApiKey(context, _vaultClientConfiguration.TokenKey);
        }
    }
}
