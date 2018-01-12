using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    public interface IVaultSecretRepository
    {
        Task<ObjectId> Set(IWorkContext context, ObjectId objectId, ObjectData objectData);

        Task<InternalVaultSecret> Get(IWorkContext context, ObjectId objectId, bool includeSecret);

        Task<PageResult<InternalVaultSecret>> List(IWorkContext context, PageRequest<ObjectId> pageRequest);

        Task Remove(IWorkContext context, ObjectId objectId);

        Task<InternalVaultSecret> AcquireLease(IWorkContext context, GroupName groupName, TimeSpan leaseTime, LessorId lessorId);

        Task ReleaseLease(IWorkContext context, ObjectId objectId);
    }
}
