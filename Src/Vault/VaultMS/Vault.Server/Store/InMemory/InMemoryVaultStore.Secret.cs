using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Contract;

namespace Vault.Server
{
    /// <summary>
    /// In memory store for secret
    /// </summary>
    public partial class InMemoryVaultStore
    {
        private readonly Dictionary<string, Stack<InternalVaultSecret>> _secretData = new Dictionary<string, Stack<InternalVaultSecret>>(StringComparer.OrdinalIgnoreCase);

        public Task<InternalVaultSecret> AcquireLease(IWorkContext context, GroupName groupName, TimeSpan leaseTime, LessorId lessorId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set secret, add if not exist, update if exist
        /// If object id does not have a version, a version will be created.
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">object id</param>
        /// <param name="objectData">object data (optional)</param>
        /// <returns></returns>
        public Task<ObjectId> SetSecret(IWorkContext context, ObjectId objectId, ObjectData objectData)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsValueValid(nameof(objectId), objectId);

            lock (_lock)
            {
                // Lookup base ID (Group Name + Name)
                Stack<InternalVaultSecret> secretStack;
                string baseId = objectId.GetBaseId();

                if (!_secretData.TryGetValue(baseId, out secretStack))
                {
                    secretStack = new Stack<InternalVaultSecret>();
                    _secretData.Add(baseId, secretStack);
                }

                if (objectId.Version != null)
                {
                    // Find in stack
                    InternalVaultSecret found = secretStack.FirstOrDefault(x => x.ObjectId.Version.Value.Equals(objectId.Version.Value, StringComparison.OrdinalIgnoreCase));
                    if (found != null)
                    {
                        found.ObjectData = objectData;
                        found._UpdatedDate = DateTimeOffset.UtcNow;
                        return Task.FromResult(found.ObjectId);
                    }
                }
                else
                {
                    objectId = new ObjectId(objectId.GroupName, objectId.Name, version: new SecretVersion(Guid.NewGuid().ToString()));
                }

                var secret = new InternalVaultSecret
                {
                    ObjectId = objectId,
                    ObjectData = objectData,
                };

                secretStack.Push(secret);
                return Task.FromResult(secret.ObjectId);
            }
        }

        /// <summary>
        /// Get secret
        /// If no version is specified, 
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">group id</param>
        /// <param name="includeSecret">true to include secret data</param>
        /// <returns>secret or null if not found</returns>
        public Task<InternalVaultSecret> GetSecret(IWorkContext context, ObjectId objectId, bool includeSecret)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(objectId), objectId);

            lock (_lock)
            {
                Stack<InternalVaultSecret> secretStack;
                string baseId = objectId.GetBaseId();

                if (!_secretData.TryGetValue(objectId.GetBaseId(), out secretStack))
                {
                    return Task.FromResult<InternalVaultSecret>(null);
                }

                return Task.FromResult(secretStack.Peek());
            }
        }

        /// <summary>
        /// List secrets
        /// If object id is null, return the complete list
        /// or use object id's group and secret name to return list of versions for object id
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="limit">limit of data set returned</param>
        /// <param name="objectId">object id or null for full list</param>
        /// <param name="index">index (optional)</param>
        /// <returns>list of vault secrets</returns>
        public Task<PageResult<InternalVaultSecret>> ListSecrets(IWorkContext context, PageRequest<ObjectId> pageRequest)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(pageRequest), pageRequest);

            int skip = pageRequest.Index.SafeParse<int>();
            int dataSetCount;

            lock (_lock)
            {
                IEnumerable<InternalVaultSecret> query;

                if (pageRequest.Query == null)
                {
                    query = _secretData.Values
                        .SelectMany(x => x);
                }
                else
                {
                    Stack<InternalVaultSecret> secretStack;
                    string baseId = pageRequest.Query.GetBaseId();

                    if (!_secretData.TryGetValue(baseId, out secretStack))
                    {
                        query = Enumerable.Empty<InternalVaultSecret>();
                    }
                    else
                    {
                        query = secretStack
                            .Where(x => x.ObjectId.IsEqual(pageRequest.Query, false));
                    }
                }

                dataSetCount = query.Count();

                var list = query
                    .Skip(skip)
                    .Take(pageRequest.Limit)
                    .Select(x => x.Clone(false))
                    .ToList();

                var result = new PageResult<InternalVaultSecret>(list, skip, pageRequest.Limit, dataSetCount);
                return Task.FromResult(result);
            }
        }

        /// <summary>
        /// Release lease
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">object id</param>
        /// <returns>task</returns>
        public Task ReleaseLease(IWorkContext context, ObjectId objectId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove secret, requires version
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="objectId">object id</param>
        /// <returns>task</returns>
        public Task RemoveSecret(IWorkContext context, ObjectId objectId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(objectId), objectId);
            Verify.IsValueValid(nameof(objectId), objectId);
            Verify.IsNotNull(nameof(objectId.Version), objectId.Version);

            lock (_lock)
            {
                Stack<InternalVaultSecret> secretStack;
                string baseId = objectId.GetBaseId();

                if (!_secretData.TryGetValue(baseId, out secretStack))
                {
                    return Task.FromResult<InternalVaultSecret>(null);
                }

                var newStack = secretStack
                    .Where(x => x.ObjectId.IsEqual(objectId, true) == false);

                if (newStack.Count() == 0)
                {
                    _secretData.Remove(baseId);
                    return Task.FromResult(0);
                }

                _secretData[baseId] = new Stack<InternalVaultSecret>(newStack.Reverse());
                return Task.FromResult(0);
            }
        }
    }
}
