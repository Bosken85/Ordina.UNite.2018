using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Private.Api.Data;

namespace Private.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(Values.Data);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(Guid id)
        {
            var value = Values.Data.FirstOrDefault(x => x.UserId == id);
            if (value == null) return NoContent();
            else return Ok(value);
        }
    }
}
