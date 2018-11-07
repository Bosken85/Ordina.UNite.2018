using System;
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
using Newtonsoft.Json.Linq;

namespace Public.Api.Clients
{
    public class PrivateApiClient : IPrivateApiClient
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

        private string GetEndpoint()
        {
            ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
            ResolvedServicePartition partition = resolver.ResolveAsync(new Uri("fabric:/Ordina.UNite.Security/Private.Api"), new ServicePartitionKey(), new CancellationToken()).Result;

            ResolvedServiceEndpoint serviceEndpoint= partition.GetEndpoint();

            JObject addresses = JObject.Parse(serviceEndpoint.Address);
            string endpoint = (string)addresses["Endpoints"].First();
            return endpoint;
        }
    }
}