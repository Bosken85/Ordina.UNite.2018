using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace APS.Controllers
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

                JObject redactedResource = null;
                if (authorizationRequest.Resource != null)
                {
                    redactedResource = new JObject();
                    var obfuscationFields = HttpContext.Items["ObfuscateFields"] as List<string> ?? new List<string>();

                    foreach (JProperty property in authorizationRequest.Resource)
                    {
                        var propertyName = property.Name;
                        var propetyValue = authorizationRequest.Resource[propertyName];
                        if (!obfuscationFields.Contains(propertyName))
                        {
                            redactedResource[propertyName] = propetyValue;
                        }
                    }
                }

                responses.Add(new AuthorizationResponse
                {
                    Action = authorizationRequest.Action,
                    HasAccess = result.Succeeded,
                    RedactedResource = redactedResource
                });
            }
            return Ok(responses);
        }
    }
}
