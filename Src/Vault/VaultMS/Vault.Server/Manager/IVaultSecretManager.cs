using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    public interface IVaultSecretManager
    {
        Task<ObjectId> Set(IWorkContext context, ObjectId objectId, string objectData = null);

        Task<InternalVaultSecret> Get(IWorkContext context, ObjectId objectId, bool includeSecret);

        Task Remove(IWorkContext context, ObjectId objectId);

        Task<PageResult<InternalVaultSecret>> List(IWorkContext context, PageRequest<ObjectId> pageRequest);

        Task<InternalVaultSecret> AcquireLease(IWorkContext context, GroupName groupName, TimeSpan leaseTime, LessorId lessorId);

        Task ReleaseLease(IWorkContext context, ObjectId objectId);
    }
}
