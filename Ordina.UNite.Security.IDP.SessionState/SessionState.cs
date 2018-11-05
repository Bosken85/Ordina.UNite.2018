using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Ordina.UNite.Security.IDP.SessionState.Interfaces;

namespace Ordina.UNite.Security.IDP.SessionState
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class SessionState : Actor, ISessionState
    {
        /// <summary>
        /// Initializes a new instance of SessionState
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public SessionState(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<string> GetSessionItem(string key, CancellationToken cancellationToken)
        {
            var stateValue = await this.StateManager.TryGetStateAsync<string>(key, cancellationToken);
            return stateValue.HasValue ? stateValue.Value : null;
        }

        public async Task RemoveSessionItem(string key, CancellationToken cancellationToken)
        {
            await this.StateManager.TryRemoveStateAsync(key, cancellationToken);
        }

        public Task SetSessionItem(string key, string value, CancellationToken cancellationToken)
        {
            return this.StateManager.AddOrUpdateStateAsync(key, value, (k, v) => value, cancellationToken);
        }
    }
}
