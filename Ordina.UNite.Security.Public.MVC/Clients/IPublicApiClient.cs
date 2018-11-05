using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ordina.UNite.Security.Public.MVC.Clients
{
    public interface IPublicApiClient
    {
        Task<HttpClient> GetClient();
    }
}
