using Khooversoft.AspMvc;
using Khooversoft.Net;
using Khooversoft.Services;
using Khooversoft.Toolbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace VaultApi.Controllers.V1
{
    [Route("V1/[controller]")]
    public class TokenController : Controller
    {
        private readonly Tag _tag = new Tag(nameof(TokenController));
        private readonly IServerTokenManager _tokenManager;

        public TokenController(IServerTokenManager tokenManager)
        {
            Verify.IsNotNull(nameof(tokenManager), tokenManager);

            _tokenManager = tokenManager;
        }

        /// <summary>
        /// Get JWT security token
        /// </summary>
        /// <param name="authorizationTokenRequest">signed JWT security token request</param>
        /// <returns>signed JWT grant or 403 (Forbidden)</returns>
        [HttpPost()]
        [Produces(typeof(string))]
        [AllowAnonymous()]
        [HmacAuthenticate(AllowAnonymous = true)]
        public async Task<IActionResult> GetAuthorizationToken([FromBody] AuthorizationTokenRequestContractV1 authorizationTokenRequest)
        {
            Verify.IsNotNull(nameof(authorizationTokenRequest), authorizationTokenRequest);
            Verify.IsNotEmpty(nameof(authorizationTokenRequest.RequestToken), authorizationTokenRequest.RequestToken);
            RequestContext requestContext = HttpContext.GetRequestContext();
            var context = requestContext.Context.WithTag(_tag);

            string token = await _tokenManager.CreateAutorizationToken(context, authorizationTokenRequest.RequestToken);
            if (token == null)
            {
                return new StandardActionResult(context, HttpStatusCode.Forbidden);
            }

            return new StandardActionResult(context)
                .SetContent(token);
        }
    }
}
