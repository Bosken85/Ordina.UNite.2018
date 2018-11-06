using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Ordina.UNite.Security.APS.Policies
{
    public static class DomainPolicies
    {
        public static void Register(this AuthorizationOptions options)
        {
            options.AddPolicy("VALUES_READ", x =>
            {
                x.RequireAuthenticatedUser();
                x.RequireClaim("unit", "NCore");
                x.RequireClaim("role", "Employee");
            });

            options.AddPolicy("VALUES_READ_DETAIL", x =>
            {
                x.RequireAuthenticatedUser();
                x.RequireClaim("unit", "NCore");
                x.RequireClaim("role", "Employee");
                x.AddRequirements(new RequiresLevel("Senior"));
            });
        }
    }
}
