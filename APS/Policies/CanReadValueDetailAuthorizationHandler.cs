using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APS.Policies
{
    public class CanReadValueDetailAuthorizationHandler : AuthorizationHandler<CanReadValueDetail, dynamic>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CanReadValueDetailAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            // You can inject dependencies in the c'tor since we need to register the handler with de DI
            // This can be useful is you need to query external resources like database or API
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanReadValueDetail requirement, dynamic resource)
        {
            var role = context.User.Claims.FirstOrDefault(x => x.Type == "role");
            if (role != null && role.Value.Equals(requirement.Role, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            Guid resourceOwnerId = resource["userId"];
            var sub = context.User.Claims.FirstOrDefault(x => x.Type == "sub");

            if (sub == null || !Guid.TryParse(sub.Value, out Guid userId))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var obfuscateFields = new List<string>();
            if (!resourceOwnerId.Equals(userId))
            {
                obfuscateFields.AddRange(new[] { "role", "yearsOfService" });
            }
            _httpContextAccessor.HttpContext.Items.Add("ObfuscateFields", obfuscateFields);

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class Value
    {
        public Guid UserId { get; set; }
    }
}
