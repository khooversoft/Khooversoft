using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    /// <summary>
    /// Vault secret manager.  This manager handles all the necessary business logic for
    /// secrets including leasing temporary
    /// 
    /// Secret id (object Id) is not case sensitive
    /// </summary>
    public class VaultSecretManager : IVaultSecretManager
    {
        private readonly IVaultServerConfiguration _vaultCoreConfiguration;
        private readonly IVaultSecretRepository _vaultSecretRepository;
        private readonly Tag _tag = new Tag(nameof(VaultSecretManager));

        public VaultSecretManager(IVaultServerConfiguration vaultCoreConfiguration, IVaultSecretRepository vaultSecretRepository)
        {
            Verify.IsNotNull(nameof(vaultCoreConfiguration), vaultCoreConfiguration);
            Verify.IsNotNull(nameof(vaultSecretRepository), vaultSecretRepository);

            _vaultCoreConfiguration = vaultCoreConfiguration;
            _vaultSecretRepository = vaultSecretRepository;
        }

        /// <summary>
        /// Create or update secret.  Vault item are created when the version is null or the version
        /// does not exists for the vault item.
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">object id</param>
        /// <param name="objectData">secret data (optional)</param>
        /// <returns>object id specified or new object ID if secret is created</returns>
        public async Task<ObjectId> Set(IWorkContext context, ObjectId objectId, string objectData = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(objectId), objectId);
            context = context.WithTag(_tag);

            ObjectData encryptedObjectData = null;
            if (objectData != null)
            {
                encryptedObjectData = new SecretDataBuilder()
                    .SetValue(objectData)
                    .SetCertificate(_vaultCoreConfiguration.DataObjectCertificate.GetCertificate(context))
                    .Encrypt()
                    .ToObjectData();
            }

            return (await _vaultSecretRepository.Set(context, objectId, encryptedObjectData))
                .RunIfNotNull(x => throw new InternalServerErrorException("No data returned from repository", context));
        }

        /// <summary>
        /// Get secret
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">object id</param>
        /// <param name="includeSecret">true to include secret data</param>
        /// <exception cref="NotFoundException">If secret cannot be located</exception>
        /// <returns>vault secret data or null if not found</returns>
        public async Task<InternalVaultSecret> Get(IWorkContext context, ObjectId objectId, bool includeSecret)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(objectId), objectId);
            context = context.WithTag(_tag);

            InternalVaultSecret result = (await _vaultSecretRepository.Get(context, objectId, includeSecret))
                .RunIfNotNull(x => throw new NotFoundException("Secret not found", context.WithTag(_tag)));

            // If secret is requested and certificate is specified
            if (!includeSecret || result.ObjectData == null)
            {
                return result;
            }

            // With certificate
            return new InternalVaultSecret
            {
                ObjectId = result.ObjectId,
                ObjectData = result.ObjectData.WithCertificate(_vaultCoreConfiguration.DataObjectCertificate.GetCertificate(context)).ToObjectData(),
                LeasedDate = result.LeasedDate,
                GoodToDate = result.GoodToDate,
                LessorId = result.LessorId,
            };
        }

        /// <summary>
        /// Remove secret (if secret does not exist, no action is taken)
        /// 
        /// Version is required
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">object id</param>
        /// <returns>task</returns>
        public async Task Remove(IWorkContext context, ObjectId objectId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(objectId), objectId);
            context = context.WithTag(_tag);

            await _vaultSecretRepository.Remove(context, objectId);
        }

        /// <summary>
        /// List secrets (paging)
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">object id</param>
        /// <param name="limit">limit of data set</param>
        /// <param name="index">index (optional)</param>
        /// <returns>page result</returns>
        public async Task<PageResult<InternalVaultSecret>> List(IWorkContext context, PageRequest<ObjectId> pageRequest)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(pageRequest), pageRequest);
            context = context.WithTag(_tag);

            return await _vaultSecretRepository.List(context, pageRequest);
        }

        /// <summary>
        /// Acquire secret lease, if available
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="groupName">group to lease secret from</param>
        /// <param name="leaseTime">how much time to lease secret</param>
        /// <param name="lessorId">lessor's ID (optional)</param>
        /// <exception cref="NotFoundException">No secret can be leased</exception>
        /// <returns>secret that was leased</returns>
        public async Task<InternalVaultSecret> AcquireLease(IWorkContext context, GroupName groupName, TimeSpan leaseTime, LessorId lessorId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(groupName), groupName);
            Verify.IsValueValid(nameof(lessorId), lessorId, allowNull: true);
            context = context.WithTag(_tag);

            return (await _vaultSecretRepository.AcquireLease(context, groupName, leaseTime, lessorId))
                .RunIfNotNull(x => throw new NotFoundException("Cannot acquire lease", context.WithTag(_tag)));
        }

        /// <summary>
        /// Release secret lease, if lease is still in effect
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">object id</param>
        /// <returns>task</returns>
        public async Task ReleaseLease(IWorkContext context, ObjectId objectId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(objectId), objectId);
            context = context.WithTag(_tag);

            await _vaultSecretRepository.ReleaseLease(context, objectId);
        }
    }
}
