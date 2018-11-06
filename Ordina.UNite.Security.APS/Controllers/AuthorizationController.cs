using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ordina.UNite.Security.APS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IEnumerable<AuthorizationRequest> authorizationRequests)
        {
            var responses = new List<AuthorizationResponse>();
            foreach (var authorizationRequest in authorizationRequests)
            {
                var policy = $"{authorizationRequest.ResourceType}_{authorizationRequest.Action}".ToUpper();
                var result = await _authorizationService.AuthorizeAsync(User, authorizationRequest.Resource, policy);
                responses.Add(new AuthorizationResponse
                {
                    Action = authorizationRequest.Action,
                    HasAccess = result.Succeeded
                });
            }
            return Ok(responses);
        }
    }
}
