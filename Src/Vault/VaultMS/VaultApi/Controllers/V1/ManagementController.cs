using Khooversoft.Actor;
using Khooversoft.AspMvc;
using Khooversoft.Net;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vault.Contract;
using Vault.Server;

namespace VaultApi.Controllers.V1
{
    [Route("V1/[controller]")]
    public class ManagementController : Controller
    {
        private readonly Tag _tag = new Tag(nameof(ManagementController));
        private readonly IVaultAdministratorStore _adminstrationRepository;
        private readonly IActorManager _actorManager;

        public ManagementController(IVaultAdministratorStore vaultAdministratorRepository, IActorManager actorManager)
        {
            Verify.IsNotNull(nameof(vaultAdministratorRepository), vaultAdministratorRepository);
            Verify.IsNotNull(nameof(actorManager), actorManager);

            _adminstrationRepository = vaultAdministratorRepository;
            _actorManager = actorManager;
        }

        /// <summary>
        /// Create new group master
        /// </summary>
        /// <returns>OK</returns>
        [Produces(typeof(string))]
        [HttpGet("health-check")]
        [HmacAuthenticate(AllowAnonymous = true)]
        public Task<IActionResult> HealthCheck()
        {
            RequestContext requestContext = this.HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            var result = new StandardActionResult(context)
                .SetContent(new HealthCheckContractV1 { Status = "ok" });

            return Task.FromResult<IActionResult>(result);
        }

#if DEBUG
        /// <summary>
        /// Clear database
        /// TODO: Need to secure with roles
        /// </summary>
        /// <returns>OK</returns>
        [HttpPost("clear-all")]
        public async Task<IActionResult> ClearDatabase()
        {
            RequestContext requestContext = this.HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            // Clear out all actors
            await _actorManager.DeactivateAllAsync(context);

            // Clear out all data in repository
            await _adminstrationRepository.ClearAllData(context);

            return new StandardActionResult(context);
        }
#endif
    }
}
