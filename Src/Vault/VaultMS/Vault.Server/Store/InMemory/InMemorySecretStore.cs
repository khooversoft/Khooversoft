using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    public class InMemorySecretStore : IVaultSecretStore
    {
        private readonly InMemoryVaultStore _inMemoryStore;

        public InMemorySecretStore(InMemoryVaultStore inMemoryRepository)
        {
            Verify.IsNotNull(nameof(inMemoryRepository), inMemoryRepository);

            _inMemoryStore = inMemoryRepository;
        }

        public Task<InternalVaultSecret> AcquireLease(IWorkContext context, GroupName groupName, TimeSpan leaseTime, LessorId lessorId)
        {
            return _inMemoryStore.AcquireLease(context, groupName, leaseTime, lessorId);
        }

        public Task<ObjectId> Set(IWorkContext context, ObjectId objectId, ObjectData objectData)
        {
            return _inMemoryStore.SetSecret(context, objectId, objectData);
        }

        public Task<InternalVaultSecret> Get(IWorkContext context, ObjectId objectId, bool includeSecret)
        {
            return _inMemoryStore.GetSecret(context, objectId, includeSecret);
        }

        public Task<PageResult<InternalVaultSecret>> List(IWorkContext context, PageRequest<ObjectId> pageRequest)
        {
            return _inMemoryStore.ListSecrets(context, pageRequest);
        }

        public Task ReleaseLease(IWorkContext context, ObjectId objectId)
        {
            return _inMemoryStore.ReleaseLease(context, objectId);
        }

        public Task Remove(IWorkContext context, ObjectId objectId)
        {
            return _inMemoryStore.RemoveSecret(context, objectId);
        }
    }
}
