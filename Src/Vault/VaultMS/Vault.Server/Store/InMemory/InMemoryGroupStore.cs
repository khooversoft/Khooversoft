using Khooversoft.Toolbox;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    public class InMemoryGroupStore : IVaultGroupStore
    {
        private readonly InMemoryVaultStore _inMemoryStore;

        public InMemoryGroupStore(InMemoryVaultStore inMemoryRepository)
        {
            Verify.IsNotNull(nameof(inMemoryRepository), inMemoryRepository);

            _inMemoryStore = inMemoryRepository;
        }

        public Task<InternalGroupMaster> Get(IWorkContext context, GroupName groupName)
        {
            return _inMemoryStore.GetGroup(context, groupName);
        }

        public Task<PageResult<InternalGroupMaster>> List(IWorkContext context, PageRequest pageRequest)
        {
            return _inMemoryStore.ListGroups(context, pageRequest);
        }

        public Task Remove(IWorkContext context, GroupName groupName)
        {
            return _inMemoryStore.RemoveGroup(context, groupName);
        }

        public Task Set(IWorkContext context, GroupName groupName, Description description, bool canLease)
        {
            return _inMemoryStore.SetGroup(context, groupName, description, canLease);
        }
    }
}
