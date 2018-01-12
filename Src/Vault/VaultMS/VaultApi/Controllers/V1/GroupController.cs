using Khooversoft.AspMvc;
using Khooversoft.Net;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vault.Contract;
using Vault.Server;

namespace VaultApi.Controllers.V1
{
    [Route("V1/[controller]")]
    public class GroupController : Controller
    {
        private readonly IVaultGroupManager _groupManager;
        private readonly Tag _tag = new Tag(nameof(GroupController));

        public GroupController(IVaultGroupManager groupManager)
        {
            Verify.IsNotNull(nameof(groupManager), groupManager);

            _groupManager = groupManager;
        }

        /// <summary>
        /// Create new group master
        /// </summary>
        /// <returns>group contract</returns>
        [HttpPost()]
        public async Task<IActionResult> Set([FromBody] CreateGroupContractV1 requestContract)
        {
            Verify.Assert(requestContract.IsValid(), nameof(CreateGroupContractV1));
            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            await _groupManager.Set(
                context,
                new GroupName(requestContract.GroupName),
                new Description(requestContract.Description),
                requestContract.CanLease);

            return new StandardActionResult(context);
        }

        /// <summary>
        /// Get group details
        /// </summary>
        /// <param name="name">group name</param>
        /// <returns>group details</returns>
        [HttpGet("{name}")]
        [Produces(typeof(GroupContractV1))]
        public async Task<IActionResult> Get(string name)
        {
            Verify.IsNotEmpty(nameof(name), name);
            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            InternalGroupMaster result = await _groupManager.Get(context, new GroupName(name));

            return new StandardActionResult(context)
                .SetContent(result.Convert());
        }

        /// <summary>
        /// Get group details
        /// </summary>
        /// <param name="name">group name</param>
        /// <returns>group details</returns>
        [HttpGet()]
        [Produces(typeof(RestPageResultV1<GroupContractV1>))]
        public async Task<IActionResult> List([FromQuery]int limit, [FromQuery]string index = null)
        {
            Verify.Assert(limit > 0, nameof(limit));

            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            PageResult<InternalGroupMaster> result = await _groupManager.List(context, new PageRequest(limit, index));

            var contract = new RestPageResultV1<GroupContractV1>
            {
                Items = new List<GroupContractV1>(result.Items.Select(x => x.Convert())),
                ContinueIndexUri = result.BuildContinueUri(requestContext.Uri, "v1/group")?.ToString(),
            };

            return new StandardActionResult(context)
                .SetContent(contract);
        }

        /// <summary>
        /// Get group details
        /// </summary>
        /// <param name="name">group name</param>
        /// <returns>group details</returns>
        [HttpDelete("{name}")]
        [Produces(typeof(GroupContractV1))]
        public async Task<IActionResult> Delete(string name)
        {
            Verify.IsNotEmpty(nameof(name), name);
            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            await _groupManager.Remove(context, new GroupName(name));

            return new StandardActionResult(context);
        }
    }
}
