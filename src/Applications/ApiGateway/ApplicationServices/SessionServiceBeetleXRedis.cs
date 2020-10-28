using System;
using System.Linq;
using EGT.ApiGateway.DomainModels;

using BeetleX.Redis;
using System.Threading.Tasks;

namespace EGT.ApiGateway.ApplicationServices
{
    public class SessionServiceBeetleXRedis : ISessionService
    {
        private readonly RedisDB _redisDB;

        public SessionServiceBeetleXRedis(RedisDB redisDB)
        {
            _redisDB = redisDB;
        }

        public async Task<(UserSession, bool)> CreateSession(UserSession userSession, int ttlInSeconds)
        {
            var sessionIdKey = RedisKeysPrefixes.SESSION + ":" + userSession.SessionId.ToString();

            var table = _redisDB.CreateHashTable(sessionIdKey);
            var sessions = await _redisDB.MGet(new string[] { sessionIdKey }, new Type[] { typeof(UserSession) });

            if (sessions.Where(s => s != null).Count() == 0)
            {
                await table.MSet((sessionIdKey, userSession));
                await _redisDB.Expire(sessionIdKey, ttlInSeconds);
            }
            else
            {
                // throw new AlreadyExistsException();
                // Not specified behavior at this scenario.
                return (null, false);
            }

            return (userSession, true);
        }

        public async Task<UserSession> GetSession(long sessionId)
        {
            var sessionIdKey = RedisKeysPrefixes.SESSION + ":" + sessionId.ToString();

            var table = _redisDB.CreateHashTable(sessionIdKey);
            var sessions = await table.Get(
                new string[] { sessionIdKey },
                new Type[] { typeof(UserSession) });

            if (sessions.Where(s => s != null).Count() == 0)
            {
                return null;
            }

            return (UserSession)sessions.First();
        }
    }
}
