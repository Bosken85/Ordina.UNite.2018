using System;
using System.Collections.Generic;
using Private.Api.Models;

namespace Private.Api.Data
{
    public class Values
    {
        public static List<Value> Data = new List<Value>
        {
            new Value
            {
                UserId = new Guid("85441825-C3EC-4314-8102-08EE8699D96A"),
                FirstName = "Kevin",
                Name = "Bosteels",
                Role = "Employee",
                Unit = "NCore",
                Function = "Developer",
                Level = "Senior",
                YearsOfService = 3
            },
            new Value
            {
                UserId = new Guid("85441825-C3EC-4314-8102-08EE8699D96B"),
                FirstName = "Jorgen",
                Name = "Jacob",
                Role = "Admin",
                Unit = "NCore",
                Function = "Bum",
                Level = "Senior",
                YearsOfService = 2
            }
        };
    }
}