using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Client
{
    public class SecretClient : ClientBase
    {
        public SecretClient(IVaultClientConfiguration vaultClientConfiguration, IRestClientConfiguration restClientConfiguration)
            : base(vaultClientConfiguration.ClientBaseUrl, restClientConfiguration)
        {
        }

        public async Task<RestResponse<string>> Set(IWorkContext context, CreateValutSecretContractV1 contract)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(contract), contract);

            return await CreateClient()
                .SetContent(contract)
                .PutAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<string>(context);
        }

        public async Task<RestResponse<VaultSecretContractV1>> Get(IWorkContext context, ObjectId objectId, bool includeSecret)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(objectId), objectId);

            string prefixPath = includeSecret ? "value" : "detail";

            return await CreateClient()
                .AddPath(prefixPath)
                .AddPath(objectId)
                .GetAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<VaultSecretContractV1>(context);
        }

        public async Task<RestResponse> Delete(IWorkContext context, ObjectId objectId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(objectId), objectId);

            return await CreateClient()
                .AddPath(objectId)
                .DeleteAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context);
        }

        public async Task<RestResponse<RestPageResultV1<VaultSecretContractV1>>> List(IWorkContext context, ObjectId objectId, int limit)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(objectId), objectId);
            Verify.Assert(limit > 0, nameof(limit));

            return await CreateClient()
                .AddPath("versions")
                .AddPath(objectId.GroupName)
                .AddPath(objectId.Name)
                .AddQuery(nameof(limit), limit.ToString())
                .GetAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<RestPageResultV1<VaultSecretContractV1>>(context);
        }

        public async Task<RestResponse<RestPageResultV1<VaultSecretContractV1>>> List(IWorkContext context, string continueIndexUri)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(continueIndexUri), continueIndexUri);

            return await CreateClient()
                .SetAbsoluteUri(new Uri(continueIndexUri))
                .GetAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<RestPageResultV1<VaultSecretContractV1>>(context);
        }

        public async Task<RestResponse<VaultSecretContractV1>> AcquireLease(IWorkContext context, GroupName groupName, TimeSpan leaseTime, LessorId lessorId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(groupName), groupName);
            Verify.IsNotNull(nameof(leaseTime), leaseTime);
            Verify.Assert(leaseTime.TotalSeconds > 0, $"{nameof(leaseTime)} must be greater then 0");
            Verify.IsValueValid(nameof(lessorId), lessorId);

            var contract = new AcquireLeaseContractV1
            {
                GroupName = groupName,
                LeaseInSeconds = (int)leaseTime.TotalSeconds,
                LessorId = lessorId
            };

            return await CreateClient()
                .SetContent(contract)
                .AddPath("acquire")
                .PostAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context)
                .GetContentAsync<VaultSecretContractV1>(context);
        }

        public async Task<RestResponse> ReleaseLease(IWorkContext context, ObjectId objectId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(objectId), objectId);

            return await CreateClient()
                .AddPath("release")
                .AddPath(objectId)
                .PutAsync(context)
                .ToRestResponseAsync(context)
                .EnsureSuccessStatusCodeAsync(context);
        }

        protected override RestClient CreateClient()
        {
            return base.CreateClient()
                .EnableHmac()
                .SetCv()
                .AddPath("v1/Secret");
        }
    }
}
