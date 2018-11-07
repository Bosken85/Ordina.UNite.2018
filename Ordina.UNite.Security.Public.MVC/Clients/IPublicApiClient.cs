using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Public.Portal.Clients
{
    public interface IPublicApiClient
    {
        Task<HttpClient> GetClient();
    }
}
