using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    /// <summary>
    /// In memory store for group
    /// </summary>
    public partial class InMemoryVaultStore : IVaultAdministratorStore
    {
        private readonly Dictionary<string, InternalGroupMaster> _groupData = new Dictionary<string, InternalGroupMaster>(StringComparer.OrdinalIgnoreCase);
        private readonly object _lock = new object();
        private readonly Guid _objectKey = Guid.NewGuid();

        public InMemoryVaultStore()
        {
            Debug.WriteLine($"Creating {nameof(InMemoryVaultStore)}, ObjectKey={_objectKey}");
        }

        /// <summary>
        /// Clear all data, group and secret
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        public Task ClearAllData(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            lock (_lock)
            {
                _groupData.Clear();
                _secretData.Clear();
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Set group, if exist, update, not exist, create
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="groupName">group name</param>
        /// <returns>internal group</returns>
        public Task<InternalGroupMaster> GetGroup(IWorkContext context, GroupName groupName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(groupName), groupName);

            lock (_lock)
            {
                if (_groupData.TryGetValue(groupName, out InternalGroupMaster groupMaster))
                {
                    return Task.FromResult(groupMaster.Clone());
                }

                return Task.FromResult<InternalGroupMaster>(null);
            }
        }

        /// <summary>
        /// List groups
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="pageRequest">page request</param>
        /// <returns>list of internal groups</returns>
        public Task<PageResult<InternalGroupMaster>> ListGroups(IWorkContext context, PageRequest pageRequest)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(pageRequest), pageRequest);

            int skip = pageRequest.Index.SafeParse<int>();

            lock (_lock)
            {
                IEnumerable<InternalGroupMaster> list = _groupData.Values
                    .Skip(skip)
                    .Take(pageRequest.Limit)
                    .Select(x => x.Clone());

                var result = new PageResult<InternalGroupMaster>(list, skip, pageRequest.Limit, _groupData.Count());
                return Task.FromResult(result);
            }
        }

        /// <summary>
        /// Remove group
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="groupName">group name</param>
        /// <returns>task</returns>
        public Task RemoveGroup(IWorkContext context, GroupName groupName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(groupName), groupName);

            lock (_lock)
            {
                _groupData.Remove(groupName);
                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// Set group, add if not exist, update if exist
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="groupName">group name</param>
        /// <param name="description">group's description</param>
        /// <param name="canLease">can the group lease secrets</param>
        /// <returns>task</returns>
        public Task SetGroup(IWorkContext context, GroupName groupName, Description description, bool canLease)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(groupName), groupName);
            Verify.IsNotNull(nameof(description), description);

            lock (_lock)
            {
                InternalGroupMaster groupMaster;

                if (_groupData.TryGetValue(groupName, out groupMaster))
                {
                    groupMaster.Description = description;
                    groupMaster.CanLease = canLease;

                    return Task.FromResult(0);
                }

                groupMaster = new InternalGroupMaster
                {
                    GroupName = groupName,
                    Description = description,
                    CanLease = canLease,
                    _CreatedDate = DateTimeOffset.UtcNow,
                };

                _groupData.Add(groupMaster.GroupName, groupMaster);
                return Task.FromResult(0);
            }
        }
    }
}
