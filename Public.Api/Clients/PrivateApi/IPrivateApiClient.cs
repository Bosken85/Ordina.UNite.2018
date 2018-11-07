using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Public.Api.Clients
{
    public interface IPrivateApiClient : IDelegationClient
    {
        Task<IEnumerable<string>> GetValues();
        Task<IEnumerable<string>> GetValue(Guid id);
    }
}
