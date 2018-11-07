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
using Newtonsoft.Json.Linq;

namespace Public.Api.Clients
{
    public class DelegationClient : IDelegationClient
    {
        private readonly string _client;
        private readonly string _scope;
        private readonly string _secret;

        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly Uri ApplicationUri;

        protected DelegationClient(IHttpContextAccessor httpContextAccessor, string application, string service, string client, string scope, string secret)
        {
            this.HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _client = client;
            _scope = scope;
            _secret = secret;
            this.ApplicationUri = new Uri($"fabric:/{application}/{service}");
        }

        public virtual async Task<HttpClient> ConstructClient()
        {
            var httpClient = new HttpClient();
            var endpoint = GetEndpoint();
            var uriBuilder = new UriBuilder(new Uri(endpoint));

            httpClient.BaseAddress = uriBuilder.Uri;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var accessToken = await RequestDelegationAccessToken();

            if (!string.IsNullOrWhiteSpace(accessToken))
                httpClient.SetBearerToken(accessToken);

            return httpClient;
        }

        protected virtual string GetEndpoint()
        {
            ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
            ResolvedServicePartition partition = resolver.ResolveAsync(ApplicationUri, new ServicePartitionKey(), new CancellationToken()).Result;

            ResolvedServiceEndpoint serviceEndpoint = partition.GetEndpoint();

            JObject addresses = JObject.Parse(serviceEndpoint.Address);
            string endpoint = (string)addresses["Endpoints"].First();
            return endpoint;
        }

        private async Task<string> RequestDelegationAccessToken()
        {
            var discoveryClient = new DiscoveryClient("http://localhost:44301/");
            var metaDataReponse = await discoveryClient.GetAsync();

            var token = await this.HttpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var payload = new
            {
                token = token
            };

            var client = new TokenClient(metaDataReponse.TokenEndpoint, this._client, this._secret);

            var response = await client.RequestCustomGrantAsync("delegation", this._scope, payload);
            return response?.AccessToken;
        }
    }
}
