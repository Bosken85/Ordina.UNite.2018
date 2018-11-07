using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Public.Api.Clients
{
    public class ApsClient : DelegationClient, IApsClient
    {
        public ApsClient(IHttpContextAccessor httpContextAccessor) 
            : base(httpContextAccessor, "Ordina.UNite.Security", "APS", "public_api.client", "aps", "secret")
        {
        }

        public async Task<AuthorizationResponse> Authorize(AuthorizationRequest authorizationRequests)
        {
            var result = await Authorize(new[] {authorizationRequests});
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<AuthorizationResponse>> Authorize(IEnumerable<AuthorizationRequest> authorizationRequests)
        {
            var payload = JsonConvert.SerializeObject(authorizationRequests, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver() 
            });
            var client = await ConstructClient();
            var response = await client.PostAsync("Authorization", new StringContent(payload, Encoding.UTF8, "application/json"));

            var result = new List<AuthorizationResponse>();
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                Console.WriteLine("Response Message Header \n\n" + response.Content.Headers + "\n");
                // Get the response
                var customerJsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Your response data is: " + customerJsonString);

                // Deserialise the data (include the Newtonsoft JSON Nuget package if you don't already have it)
                var deserialized = JsonConvert.DeserializeObject<IEnumerable<AuthorizationResponse>>(customerJsonString);
                // Do something with it
                result.AddRange(deserialized);
            }
            return result;
        }
    }
}