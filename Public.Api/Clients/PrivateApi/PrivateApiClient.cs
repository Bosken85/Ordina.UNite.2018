using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Public.Api.Clients
{
    public class PrivateApiClient : DelegationClient, IPrivateApiClient
    {
        public PrivateApiClient(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor, "Ordina.UNite.Security", "Private.Api", "public_api.client", "private_api", "secret")
        {
        }

        public async Task<IEnumerable<string>> GetValues()
        {
            var client = await ConstructClient();
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
            return values;
        }

        public Task<IEnumerable<string>> GetValue(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}