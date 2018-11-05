using System.Net.Http;
using System.Threading.Tasks;

namespace Ordina.UNite.Security.Public.Api.Clients
{
    public interface IPrivateApiClient
    {
        Task<HttpClient> GetClient();
    }
}
