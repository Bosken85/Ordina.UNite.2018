using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace APS.Policies
{
    public class CanReadValueDetail : IAuthorizationRequirement
    {
        public string Role { get; }

        public CanReadValueDetail(string role)
        {
            Role = role;
        }
    }
}
