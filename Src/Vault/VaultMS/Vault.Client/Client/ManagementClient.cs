using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Toolbox;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Client
{
    public class ManagementClient : ClientBase
    {
        public ManagementClient(IVaultClientConfiguration vaultClientConfiguration, IRestClientConfiguration restClientConfiguration)
            : base(vaultClientConfiguration.ClientBaseUrl, restClientConfiguration)
        {
        }

        /// <summary>
        /// Get health check data (returned JSON as a string)
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>health check</returns>
        public async Task<RestResponse<HealthCheckContractV1>> HealthCheck(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            return await CreateClient()
                .AddPath("health-check")
                .GetAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<HealthCheckContractV1>(context);
        }

        /// <summary>
        /// Used for testing, clear database
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>OK</returns>
        public async Task<RestResponse> ClearDatabase(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            return await CreateClient()
                .EnableHmac()
                .AddPath("clear-all")
                .PostAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context);
        }

        protected override RestClient CreateClient()
        {
            return base.CreateClient()
                .SetCv()
                .AddPath("v1/management");
        }
    }
}
