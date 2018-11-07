using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Public.Api.Clients
{
    public class ClientFactory : IClientFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TokenResponse _delegationToken;
        private DateTime? _expirationTime;

        public ClientFactory(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IApsClient> GetApsClient()
        {
            var client = _serviceProvider.GetService<IApsClient>();
            await RequestDelegationAccessToken();
            client.SetAccessToken(_delegationToken.AccessToken);
            return client;
        }

        public async Task<IPrivateApiClient> GetPrivateApiClient()
        {
            var client = _serviceProvider.GetService<IPrivateApiClient>();
            await RequestDelegationAccessToken();
            client.SetAccessToken(_delegationToken.AccessToken);
            return client;
        }

        private async Task RequestDelegationAccessToken()
        {
            if (_delegationToken != null && DateTime.UtcNow < _expirationTime) return; 

            var discoveryClient = new DiscoveryClient("http://localhost:44301/");
            var metaDataReponse = await discoveryClient.GetAsync();

            var token = await this._httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var payload = new
            {
                token = token
            };

            var client = new TokenClient(metaDataReponse.TokenEndpoint, "public_api.client", "secret");

            var response = await client.RequestCustomGrantAsync("delegation", "aps private_api", payload);
            if (!response.IsError)
            {
                _expirationTime = DateTime.UtcNow.AddSeconds(response.ExpiresIn); 
                _delegationToken = response;
            }
            else
            {
                _expirationTime = null;
                _delegationToken = null;
            }
        }
    }
}