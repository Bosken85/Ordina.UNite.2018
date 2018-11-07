using System;

namespace Private.Api.Models
{
    public class Value
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Unit { get; set; }
        public string Function { get; set; }
        public string Level { get; set; }
        public int YearsOfService { get; set; }

    }
}