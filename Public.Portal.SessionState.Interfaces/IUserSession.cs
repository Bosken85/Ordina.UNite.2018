using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace Public.Portal.SessionState.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IUserSession : IActor
    {
        Task<T> GetSessionItem<T>(string key, CancellationToken cancellationToken);
        Task SetSessionItem<T>(string key, T value, CancellationToken cancellationToken);
        Task RemoveSessionItem(string key, CancellationToken cancellationToken);
    }
}
