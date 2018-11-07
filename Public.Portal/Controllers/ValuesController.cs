using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Public.Portal.Clients;
using Public.Portal.Models;

namespace Public.Portal.Controllers
{
    [Authorize]
    public class ValuesController : Controller
    {
        private readonly IPublicApiClient _publicApiClient;

        public ValuesController(IPublicApiClient publicApiClient)
        {
            _publicApiClient = publicApiClient;
        }

        // GET: Values
        public async Task<IActionResult> Index()
        {
            var client = await _publicApiClient.GetClient();
            var response = await client.GetAsync("values");

            var values = new List<Value>();
            if (response.IsSuccessStatusCode)
            {
                // Get the response
                var customerJsonString = await response.Content.ReadAsStringAsync();
                // Deserialise the data (include the Newtonsoft JSON Nuget package if you don't already have it)
                var deserialized = JsonConvert.DeserializeObject<IEnumerable<Value>>(customerJsonString);
                // Do something with it
                values.AddRange(deserialized);
            }
            return View(values);
        }

        // GET: Values/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var client = await _publicApiClient.GetClient();
            var response = await client.GetAsync($"values/{id}");

            Value value = null;
            if (response.IsSuccessStatusCode)
            {
                // Get the response
                var customerJsonString = await response.Content.ReadAsStringAsync();
                // Deserialise the data (include the Newtonsoft JSON Nuget package if you don't already have it)
                value = JsonConvert.DeserializeObject<Value>(customerJsonString);
                return View(value);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}