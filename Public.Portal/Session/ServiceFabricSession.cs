using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Newtonsoft.Json;
using Public.Portal.SessionState.Interfaces;

namespace Public.Portal.Session
{
    public class ServiceFabricSession : ISession
    {
        private const int IdByteCount = 16;
        private readonly string sessionKey;
        private readonly TimeSpan idleTimeout;
        private readonly TimeSpan ioTimeout;
        private readonly Func<bool> tryEstablishSession;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<ServiceFabricSession> logger;
        private readonly bool isNewSessionKey;

        private string _sessionId;
        private byte[] _sessionIdBytes;
        private IUserSession userSession;

        private Dictionary<string, byte[]> store;

        public ServiceFabricSession(IHttpContextAccessor httpContextAccessor, string sessionKey, TimeSpan idleTimeout,
            TimeSpan ioTimeout, Func<bool> tryEstablishSession, ILoggerFactory loggerFactory, bool isNewSessionKey)
        {
            if (string.IsNullOrEmpty(sessionKey))
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(sessionKey));
            }

            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.logger = loggerFactory.CreateLogger<ServiceFabricSession>();
            this.sessionKey = sessionKey;
            this.idleTimeout = idleTimeout;
            this.ioTimeout = ioTimeout;
            this.tryEstablishSession = tryEstablishSession;
            this.isNewSessionKey = isNewSessionKey;
        }

        private void Load()
        {
            LoadAsync().GetAwaiter().GetResult();
        }


        public Task LoadAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!this.IsAvailable)
            {
                var actorId = new ActorId(this.Id);
                this.userSession = ActorProxy.Create<IUserSession>(actorId, "Ordina.UNite.Security", "Public.Portal.SessionState");
            }
            return Task.CompletedTask;
        }

        public Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        internal byte[] ConvertToBytes(object value)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    using (var jr = new JsonTextWriter(sw))
                    {
                        var serializer = JsonSerializer.Create();
                        serializer.Serialize(jr, value);
                    }
                }
                return ms.ToArray();
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            Load();
            var item = this.userSession.GetSessionItem<byte[]>(key, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
            if (item != null)
            {
                value = item;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            Load();
            var item = this.userSession.GetSessionItem<byte[]>(key, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
            if (item != null)
            {
                value = item.GetType() == typeof(byte[]) ? item : ConvertToBytes(item);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public void Set<T>(string key, T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Load();
            var payload = ConvertToBytes(value);
            this.userSession.SetSessionItem(key, payload, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Set(string key, byte[] value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Load();

            this.userSession.SetSessionItem(key, value, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Remove(string key)
        {
            Load();
            this.userSession.RemoveSessionItem(key, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Clear()
        {
            ActorId actorToDelete = new ActorId(this.Id);
            var myActorServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/Ordina.UNite.Security/Public.Portal.SessionState"), actorToDelete);
            myActorServiceProxy.DeleteActorAsync(actorToDelete, new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public bool IsAvailable
        {
            get
            {
                Load();
                return this.userSession != null;
            }
        }

        public string Id
        {
            get
            {
                Load();
                if (string.IsNullOrWhiteSpace(_sessionId))
                {
                    if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    {
                        var idClaim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x=> x.Type == ClaimTypes.NameIdentifier);
                        _sessionId = idClaim?.Value;
                    }

                    if (string.IsNullOrWhiteSpace(_sessionId))
                    {
                        _sessionId = CryptoRandom.CreateUniqueId();
                    }
                }

                return _sessionId;
            }
        }

        public IEnumerable<string> Keys { get; }
    }
}
