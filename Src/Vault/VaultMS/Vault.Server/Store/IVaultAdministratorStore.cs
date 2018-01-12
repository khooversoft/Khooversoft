using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Vault.Server
{
    public interface IVaultAdministratorStore
    {
        Task ClearAllData(IWorkContext context);
    }
}
