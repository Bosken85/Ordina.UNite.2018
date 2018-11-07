using System.Net.Http;
using System.Threading.Tasks;

namespace Public.Api.Clients
{
    public interface IDelegationClient
    {
        Task<HttpClient> ConstructClient();

    }
}