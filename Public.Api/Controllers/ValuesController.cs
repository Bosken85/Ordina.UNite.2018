﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Public.Api.Clients;

namespace Public.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private readonly IClientFactory _clientFactory;

        public ValuesController(IClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var apsClient = await _clientFactory.ApsClient();
            var authorization = await apsClient.Authorize(new AuthorizationRequest { Action = "read", ResourceType = "Values"});

            if (!authorization.HasAccess) return Unauthorized();

            var privateClient = await _clientFactory.PrivateApiClient();
            var values = await privateClient.GetValues();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var privateClient = await _clientFactory.PrivateApiClient();
            var value = await privateClient.GetValue(id);

            var apsClient = await _clientFactory.ApsClient();
            var authorization = await apsClient.Authorize(new AuthorizationRequest { Action = "read_detail", ResourceType = "Values", Resource = value });

            if (!authorization.HasAccess) return Unauthorized();
            
            return Ok(authorization.RedactedResource);
        }
    }
}
