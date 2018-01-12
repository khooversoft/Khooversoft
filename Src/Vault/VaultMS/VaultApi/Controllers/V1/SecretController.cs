using Khooversoft.AspMvc;
using Khooversoft.Net;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Contract;
using Vault.Server;

namespace VaultApi.Controllers.V1
{
    [Route("V1/[controller]")]
    public class SecretController : Controller
    {
        private readonly IVaultSecretManager _secretManager;
        private readonly Tag _tag = new Tag(nameof(SecretController));

        public SecretController(IVaultSecretManager secretManager)
        {
            Verify.IsNotNull(nameof(secretManager), secretManager);

            _secretManager = secretManager;
        }

        /// <summary>
        /// Create new group master
        /// </summary>
        /// <returns>group contract</returns>
        [HttpPut()]
        [Produces(typeof(string))]
        public async Task<IActionResult> Set([FromBody] CreateValutSecretContractV1 requestContract)
        {
            Verify.IsNotNull(nameof(requestContract), requestContract);
            Verify.Assert(requestContract.IsValid(), nameof(CreateValutSecretContractV1));

            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            var objectId = new ObjectId(requestContract.ObjectId);
            Verify.IsValueValid(nameof(objectId), objectId);

            ObjectId response = await _secretManager.Set(context, objectId, requestContract.ObjectData);

            return new StandardActionResult(context)
                .SetContent(response.ToString());
        }

        /// <summary>
        /// Get secret details
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <param name="name">secret id</param>
        /// <param name="version">version</param>
        /// <returns>vault secret data</returns>
        [HttpGet("detail/{groupName}/{name}/{version?}")]
        [Produces(typeof(VaultSecretContractV1))]
        public async Task<IActionResult> GetDetails(string groupName, string name, string version)
        {
            Verify.IsNotEmpty(nameof(groupName), groupName);
            Verify.IsNotEmpty(nameof(name), name);

            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            var objectId = new ObjectId(groupName, name, version);
            Verify.IsValueValid(nameof(objectId), objectId);

            InternalVaultSecret result = await _secretManager.Get(context, objectId, includeSecret: false);

            return new StandardActionResult(context)
                .SetContent(result.Convert());
        }

        /// <summary>
        /// Get secret value
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <param name="name">secret id</param>
        /// <param name="version">version</param>
        /// <returns>vault secret data</returns>
        [HttpGet("value/{groupName}/{name}/{version?}")]
        [Produces(typeof(VaultSecretContractV1))]
        public async Task<IActionResult> GetValue(string groupName, string name, string version)
        {
            Verify.IsNotEmpty(nameof(groupName), groupName);
            Verify.IsNotEmpty(nameof(name), name);

            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            var objectId = new ObjectId(groupName, name, version);
            Verify.IsValueValid(nameof(objectId), objectId);

            InternalVaultSecret result = await _secretManager.Get(context, objectId, includeSecret: true);

            return new StandardActionResult(context)
                .SetContent(result.Convert());
        }

        /// <summary>
        /// Get versions for a secret
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="limit"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet("versions/{groupName}/{name}")]
        [Produces(typeof(RestPageResultV1<VaultSecretContractV1>))]
        public async Task<IActionResult> GetVersionDetails(string groupName, string name, [FromQuery]int limit, [FromQuery]string index = null)
        {
            Verify.IsNotEmpty(nameof(groupName), groupName);
            Verify.IsNotEmpty(nameof(name), name);
            Verify.Assert(limit > 0, nameof(limit));

            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            var objectId = new ObjectId(groupName, name);
            Verify.IsValueValid(nameof(objectId), objectId);

            PageResult<InternalVaultSecret> result = await _secretManager.List(context, new PageRequest<ObjectId>(objectId, limit, index));

            var contract = new RestPageResultV1<VaultSecretContractV1>
            {
                Items = new List<VaultSecretContractV1>(result.Items.Select(x => x.Convert())),
                ContinueIndexUri = result.BuildContinueUri(requestContext.Uri, "v1/secret", $"versions/{groupName}/{name}")?.ToString(),
            };

            return new StandardActionResult(context)
                .SetContent(contract);
        }

        /// <summary>
        /// Delete lease
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <param name="name">secret id</param>
        /// <param name="version">version</param>
        /// <returns>group details</returns>
        [HttpDelete("{groupName}/{name}/{version}")]
        public async Task<IActionResult> Delete(string groupName, string name, string version)
        {
            Verify.IsNotEmpty(nameof(groupName), groupName);
            Verify.IsNotEmpty(nameof(name), name);
            Verify.IsNotEmpty(nameof(version), version);
            RequestContext requestContext = this.HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            var objectId = new ObjectId(groupName, name, version);
            await _secretManager.Remove(context, objectId);

            return new StandardActionResult(context);
        }

        /// <summary>
        /// Acquire lease
        /// </summary>
        /// <param name="requestContract">lease request</param>
        /// <returns>lease contract</returns>
        [HttpPost("acquire")]
        [Produces(typeof(VaultSecretContractV1))]
        public async Task<IActionResult> AcquireLease([FromBody] AcquireLeaseContractV1 requestContract)
        {
            Verify.Assert(requestContract.IsValid(), nameof(AcquireLeaseContractV1));
            RequestContext requestContext = this.HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            InternalVaultSecret result = await _secretManager.AcquireLease(
                    context,
                    new GroupName(requestContract.GroupName),
                    TimeSpan.FromSeconds(requestContract.LeaseInSeconds),
                    new LessorId(requestContract.LessorId));

            return new StandardActionResult(context)
                .SetContent(result.Convert());
        }

        /// <summary>
        /// Release lease
        /// </summary>
        /// <param name="requestContract">lease request</param>
        /// <returns>lease contract</returns>
        [HttpPost("release/{groupName}/{name}/{version?}")]
        [Produces(typeof(VaultSecretContractV1))]
        public async Task<IActionResult> ReleaseLease(string objectId)
        {
            Verify.IsNotEmpty(nameof(objectId), objectId);
            RequestContext requestContext = this.HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            await _secretManager.ReleaseLease(context, new ObjectId(objectId));

            return new StandardActionResult(context);
        }
    }
}
