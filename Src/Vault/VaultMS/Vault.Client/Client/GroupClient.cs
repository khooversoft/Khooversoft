using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Client
{
    public class GroupClient : ClientBase
    {
        public GroupClient(IVaultClientConfiguration vaultClientConfiguration, IRestClientConfiguration restClientConfiguration)
            : base(vaultClientConfiguration.ClientBaseUrl, restClientConfiguration)
        {
        }

        public async Task<RestResponse<GroupContractV1>> Set(IWorkContext context, CreateGroupContractV1 contract)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(contract), contract);

            return await CreateClient()
                .SetContent(contract)
                .PostAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<GroupContractV1>(context);
        }

        public async Task<RestResponse<GroupContractV1>> Get(IWorkContext context, GroupName groupName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(groupName), groupName);

            return await CreateClient()
                .AddPath(groupName)
                .GetAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<GroupContractV1>(context);
        }

        public async Task<RestResponse<RestPageResultV1<GroupContractV1>>> List(IWorkContext context, int limit, string index = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.Assert(limit > 0, nameof(limit));

            return await CreateClient()
                .AddQuery(nameof(limit), limit.ToString())
                .AddQuery(nameof(index), index)
                .GetAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<RestPageResultV1<GroupContractV1>>(context);
        }

        public async Task<RestResponse<RestPageResultV1<GroupContractV1>>> List(IWorkContext context, string continueIndexUri)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(continueIndexUri), continueIndexUri);

            return await CreateClient()
                .SetAbsoluteUri(new Uri(continueIndexUri))
                .GetAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<RestPageResultV1<GroupContractV1>>(context);
        }

        public async Task<RestResponse> Delete(IWorkContext context, GroupName groupName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(groupName), groupName);

            return await CreateClient()
                .AddPath(groupName)
                .DeleteAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context);
        }

        protected override RestClient CreateClient()
        {
            return base.CreateClient()
                .EnableHmac()
                .SetCv()
                .AddPath("v1/Group");
        }
    }
}
