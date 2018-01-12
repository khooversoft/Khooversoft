using Khooversoft.Net;
using Khooversoft.Toolbox;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    /// <summary>
    /// Vault secret group manager, support groups having a collection of secrets.
    /// Secrets can only be leased by a group that has been enabled for leasing.
    /// 
    /// Group's name is not case sensitive
    /// </summary>
    public class VaultGroupManager : IVaultGroupManager
    {
        private readonly IVaultGroupRepository _vaultGroupRepository;
        private readonly Tag _tag = new Tag(nameof(VaultGroupManager));

        public VaultGroupManager(IVaultGroupRepository vaultGroupRepository)
        {
            Verify.IsNotNull(nameof(vaultGroupRepository), vaultGroupRepository);

            _vaultGroupRepository = vaultGroupRepository;
        }

        /// <summary>
        /// Set group (create or update)
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="groupName">name of group</param>
        /// <param name="description">description</param>
        /// <param name="canLease">true if secrets associated with group can be leased</param>
        /// <returns>task</returns>
        public Task Set(IWorkContext context, GroupName groupName, Description description, bool canLease)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(groupName), groupName);
            Verify.IsNotNull(nameof(description), description);

            return _vaultGroupRepository.Set(context, groupName, description, canLease);
        }

        /// <summary>
        /// Get group
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="groupName">name of group</param>
        /// <exception cref="NotFoundException">if group cannot be located</exception>
        /// <returns>group details</returns>
        public async Task<InternalGroupMaster> Get(IWorkContext context, GroupName groupName)
        {
            Verify.IsNotNull(nameof(context), context);

            return (await _vaultGroupRepository.Get(context, groupName))
                .RunIfNotNull(x => throw new NotFoundException($"Cannot find group {groupName.Value}", context.WithTag(_tag)));
        }

        /// <summary>
        /// List groups
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="index">index (optional)</param>
        /// <returns>list of all groups</returns>
        public async Task<PageResult<InternalGroupMaster>> List(IWorkContext context, PageRequest pageRequest)
        {
            Verify.IsNotNull(nameof(context), context);

            PageResult<InternalGroupMaster> results = await _vaultGroupRepository.List(context, pageRequest);
            return results;
        }

        /// <summary>
        /// Remove group
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="groupName">name of group</param>
        /// <returns>task</returns>
        public Task Remove(IWorkContext context, GroupName groupName)
        {
            Verify.IsNotNull(nameof(context), context);

            return _vaultGroupRepository.Remove(context, groupName);
        }
    }
}
