using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Public.Portal.Clients;

namespace Public.Portal.Controllers
{
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

            var values = new List<string>();
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");
                // Get the response
                var customerJsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Your response data is: " + customerJsonString);

                // Deserialise the data (include the Newtonsoft JSON Nuget package if you don't already have it)
                var deserialized = JsonConvert.DeserializeObject<IEnumerable<string>>(customerJsonString);
                // Do something with it
                values.AddRange(deserialized);
            }
            return View(values);
        }

        // GET: Values/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
    }
}