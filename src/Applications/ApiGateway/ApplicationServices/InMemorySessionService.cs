using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using EGT.ApiGateway.DomainModels;

namespace EGT.ApiGateway.ApplicationServices
{

    public class InMemorySessionService : ISessionService
    {
        private Dictionary<long, UserSession> _sessions = new Dictionary<long, UserSession>();
        private ReaderWriterLock _lock = new ReaderWriterLock();

        public Task<UserSession> GetSession(long sessionId)
        {
            try
            {
                _lock.AcquireReaderLock(int.MaxValue);

                if (_sessions.ContainsKey(sessionId))
                {
                    return Task.FromResult(_sessions[sessionId]);
                }

                return Task.FromResult((UserSession)null);
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }

        public Task<(UserSession, bool)> CreateSession(UserSession userSession, int ttlInSeconds)
        {
            try
            {
                _lock.AcquireWriterLock(int.MaxValue);
                if (!_sessions.ContainsKey(userSession.SessionId))
                {
                    _sessions.Add(userSession.SessionId, userSession);
                }
                else
                {
                    // throw new AlreadyExistsException();
                    // Not specified behavior at this scenario.
                    return Task.FromResult(((UserSession)null, false));
                }
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }

            return Task.FromResult((_sessions[userSession.SessionId], true));
        }
    }
}
