using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace APS.Policies
{
    public class RequiresLevel : IAuthorizationRequirement
    {
        public string Level { get; }

        public RequiresLevel(string level)
        {
            Level = level;
        }
    }
}
