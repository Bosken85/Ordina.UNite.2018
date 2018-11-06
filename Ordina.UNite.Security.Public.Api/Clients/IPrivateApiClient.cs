using System.Net.Http;
using System.Threading.Tasks;

namespace Ordina.UNite.Security.Public.Api.Clients
{
    public interface IPrivateApiClient
    {
        HttpClient Client { get; }
        void SetAccessToken(string token);

    }
}
