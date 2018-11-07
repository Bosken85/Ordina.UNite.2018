using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Public.Api.Clients
{
    public interface IApsClient : IDelegationClient
    {
        Task<AuthorizationResponse> Authorize(AuthorizationRequest authorizationRequests);
        Task<IEnumerable<AuthorizationResponse>> Authorize(IEnumerable<AuthorizationRequest> authorizationRequests);
    }
}