using System;
using System.Linq;

using EGT.ApiGateway.DomainModels;

using StackExchange.Redis;

namespace EGT.ApiGateway.ApplicationServices
{
    public class RedisCacheSessionService : ISessionService, IDisposable
    {
        private readonly ConnectionMultiplexer _muxer;
        public RedisCacheSessionService(ConnectionMultiplexer muxer)
        {
            _muxer = muxer;
        }

        public (UserSession, bool) CreateSession(UserSession userSession, int ttlInSeconds)
        {
            var connection = _muxer.GetDatabase();

            var sessionIdKey = new RedisKey(RedisKeysPrefixes.SESSION + ":" + userSession.SessionId.ToString());
            if (connection.HashGetAll(sessionIdKey).Count() == 0)
            {
                connection.HashSet(
                    sessionIdKey,
                    new HashEntry[]
                    {
                        new HashEntry(new RedisValue(nameof(userSession.RequestId)), new RedisValue(userSession.RequestId)),
                        new HashEntry(new RedisValue(nameof(userSession.Player)), new RedisValue(userSession.Player.ToString())),
                        new HashEntry(new RedisValue(nameof(userSession.Timestamp)), new RedisValue(userSession.Timestamp.ToString())),
                        new HashEntry(new RedisValue(nameof(userSession.SessionId)), new RedisValue(userSession.SessionId.ToString())),
                    });
                connection.KeyExpire(sessionIdKey, TimeSpan.FromSeconds(ttlInSeconds));
            }
            else
            {
                // throw new AlreadyExistsException();
                // Not specified behavior at this scenario.
                return (null, false);
            }

            return (userSession, true);
        }

        public UserSession GetSession(long sessionId)
        {
            IDatabase connection = _muxer.GetDatabase();

            var redisKey = new RedisKey(sessionId.ToString());
            if (connection.HashGetAll(redisKey).Count() == 0)
            {
                return null;
            }

            var result = new UserSession
            {
                RequestId = connection.HashGet(redisKey, nameof(UserSession.RequestId)),
                Player = Convert.ToInt32(connection.HashGet(redisKey, nameof(UserSession.Player)).ToString()),
                Timestamp = long.Parse(connection.HashGet(redisKey, nameof(UserSession.Timestamp)).ToString()),
                SessionId = sessionId
            };

            return result;
        }

        public void Dispose()
        {
            _muxer.Dispose();
        }
    }
}
