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

namespace Public.Api.Clients
{
    public class ApsClient : IApsClient
    {
        private HttpClient _httpClient = new HttpClient();
        private string accessToken;

        public HttpClient Client
        {
            get
            {
                var endpoint = GetEndpoint();
                var uriBuilder = new UriBuilder(new Uri(endpoint));

                _httpClient.BaseAddress = uriBuilder.Uri;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(accessToken))
                    _httpClient.SetBearerToken(accessToken);

                return _httpClient;
            }
        }

        public void SetAccessToken(string token)
        {
            accessToken = token;
        }

        public async Task<AuthorizationResponse> Authorize(AuthorizationRequest authorizationRequests)
        {
            var result = await Authorize(new[] {authorizationRequests});
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<AuthorizationResponse>> Authorize(IEnumerable<AuthorizationRequest> authorizationRequests)
        {
            var payload = JsonConvert.SerializeObject(authorizationRequests);
            var response = await Client.PostAsync("Authorization", new StringContent(payload, Encoding.UTF8, "application/json"));

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

        private string GetEndpoint()
        {
            ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
            ResolvedServicePartition partition = resolver.ResolveAsync(new Uri("fabric:/Ordina.UNite.Security/APS"), new ServicePartitionKey(), new CancellationToken()).Result;

            ResolvedServiceEndpoint serviceEndpoint = partition.GetEndpoint();

            JObject addresses = JObject.Parse(serviceEndpoint.Address);
            string endpoint = (string)addresses["Endpoints"].First();
            return endpoint;
        }
    }
}