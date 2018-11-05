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

namespace Ordina.UNite.Security.Public.Api.Clients
{
    public class PrivateApiClient : IPrivateApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient = new HttpClient();

        public PrivateApiClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpClient> GetClient()
        {
            var endpoint = GetEndpoint();
            var uriBuilder = new UriBuilder(new Uri(endpoint));

            _httpClient.BaseAddress = uriBuilder.Uri;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var accessToken = await GetDelegationAccessToken();

            if (!string.IsNullOrWhiteSpace(accessToken))
                _httpClient.SetBearerToken(accessToken);

            return _httpClient;
        }

        private string GetEndpoint()
        {
            ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
            ResolvedServicePartition partition = resolver.ResolveAsync(new Uri("fabric:/Ordina.UNite.Security/Ordina.UNite.Security.Private.Api"), new ServicePartitionKey(), new CancellationToken()).Result;

            ResolvedServiceEndpoint serviceEndpoint= partition.GetEndpoint();

            JObject addresses = JObject.Parse(serviceEndpoint.Address);
            string endpoint = (string)addresses["Endpoints"].First();
            return endpoint;
        }

        private async Task<string> GetDelegationAccessToken()
        {
            var discoveryClient = new DiscoveryClient("http://localhost:44301/");
            var metaDataReponse = await discoveryClient.GetAsync();

            var token = await this._httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var payload = new
            {
                token = token
            };

            var client = new TokenClient(metaDataReponse.TokenEndpoint, "public_api.client", "secret");

            var response = await client.RequestCustomGrantAsync("delegation", "private_api", payload);
            return response.AccessToken;
        }
    }
}