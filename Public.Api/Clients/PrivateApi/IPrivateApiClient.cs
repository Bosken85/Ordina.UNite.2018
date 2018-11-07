using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Public.Api.Clients
{
    public interface IPrivateApiClient : IDelegationClient
    {
        Task<IEnumerable<dynamic>> GetValues();
        Task<dynamic> GetValue(Guid id);
    }
}
