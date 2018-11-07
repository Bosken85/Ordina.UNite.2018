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

        public ClientFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<IApsClient> ApsClient()
        {
            var client = _serviceProvider.GetService<IApsClient>();
            return client;
        }

        public async Task<IPrivateApiClient> PrivateApiClient()
        {
            var client = _serviceProvider.GetService<IPrivateApiClient>();
            return client;
        }
    }
}