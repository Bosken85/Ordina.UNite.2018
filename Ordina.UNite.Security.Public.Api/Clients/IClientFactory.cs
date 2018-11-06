using System.Threading.Tasks;

namespace Ordina.UNite.Security.Public.Api.Clients
{
    public interface IClientFactory
    {
        Task<IApsClient> GetApsClient();
        Task<IPrivateApiClient> GetPrivateApiClient();
    }
}