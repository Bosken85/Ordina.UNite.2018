using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;

namespace Public.Portal.Session
{
    public class ServiceFabricSessionStorage : ISessionStore
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceFabricSessionStorage(ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _httpContextAccessor = httpContextAccessor;
        }

        public ISession Create(string sessionKey, TimeSpan idleTimeout, TimeSpan ioTimeout, Func<bool> tryEstablishSession,
            bool isNewSessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                throw new ArgumentNullException(nameof(sessionKey));
            }
            if (tryEstablishSession == null)
            {
                throw new ArgumentNullException(nameof(tryEstablishSession));
            }

            return new ServiceFabricSession(_httpContextAccessor, sessionKey, idleTimeout, ioTimeout, tryEstablishSession, _loggerFactory, isNewSessionKey);
        }
    }
}
