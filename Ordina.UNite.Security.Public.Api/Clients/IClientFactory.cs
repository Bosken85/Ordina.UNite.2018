using System.Threading.Tasks;

namespace Public.Api.Clients
{
    public interface IClientFactory
    {
        Task<IApsClient> GetApsClient();
        Task<IPrivateApiClient> GetPrivateApiClient();
    }
}