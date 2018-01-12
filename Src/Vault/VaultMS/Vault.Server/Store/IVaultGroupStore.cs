using Khooversoft.Toolbox;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    public interface IVaultGroupStore
    {
        Task Set(IWorkContext context, GroupName groupName, Description description, bool canLease);

        Task<InternalGroupMaster> Get(IWorkContext context, GroupName groupName);

        Task Remove(IWorkContext context, GroupName groupName);

        Task<PageResult<InternalGroupMaster>> List(IWorkContext context, PageRequest pageRequest);
    }
}
