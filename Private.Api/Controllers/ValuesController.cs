using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Private.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var given_name = User.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value;
            var family_name = User.Claims.FirstOrDefault(x => x.Type == "family_name")?.Value;
            var role = User.Claims.FirstOrDefault(x => x.Type == "role")?.Value;
            var unit = User.Claims.FirstOrDefault(x => x.Type == "unit")?.Value;
            var function = User.Claims.FirstOrDefault(x => x.Type == "function")?.Value;
            var level = User.Claims.FirstOrDefault(x => x.Type == "level")?.Value;

            return new string[] { given_name, family_name, role, unit, function, level};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
