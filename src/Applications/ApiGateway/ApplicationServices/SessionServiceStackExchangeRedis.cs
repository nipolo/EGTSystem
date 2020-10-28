using System;
using System.Linq;
using System.Threading.Tasks;

using EGT.ApiGateway.DomainModels;

using StackExchange.Redis;

namespace EGT.ApiGateway.ApplicationServices
{
    public class SessionServiceStackExchangeRedis : ISessionService, IDisposable
    {
        private readonly ConnectionMultiplexer _muxer;
        public SessionServiceStackExchangeRedis(ConnectionMultiplexer muxer)
        {
            _muxer = muxer;
        }

        public async Task<(UserSession, bool)> CreateSession(UserSession userSession, int ttlInSeconds)
        {
            var connection = _muxer.GetDatabase();

            var sessionIdKey = new RedisKey(RedisKeysPrefixes.SESSION + ":" + userSession.SessionId.ToString());
            if ((await connection.HashGetAllAsync(sessionIdKey)).Count() == 0)
            {
                await connection.HashSetAsync(
                    sessionIdKey,
                    new HashEntry[]
                    {
                        new HashEntry(new RedisValue(nameof(userSession.RequestId)), new RedisValue(userSession.RequestId)),
                        new HashEntry(new RedisValue(nameof(userSession.Player)), new RedisValue(userSession.Player.ToString())),
                        new HashEntry(new RedisValue(nameof(userSession.Timestamp)), new RedisValue(userSession.Timestamp.ToString())),
                        new HashEntry(new RedisValue(nameof(userSession.SessionId)), new RedisValue(userSession.SessionId.ToString())),
                    });
                await connection.KeyExpireAsync(sessionIdKey, TimeSpan.FromSeconds(ttlInSeconds));
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
            IDatabase connection = _muxer.GetDatabase();

            var redisKey = new RedisKey(RedisKeysPrefixes.SESSION + ":" + sessionId.ToString());
            if ((await connection.HashGetAllAsync(redisKey)).Count() == 0)
            {
                return null;
            }

            var result = new UserSession
            {
                RequestId = await connection.HashGetAsync(redisKey, nameof(UserSession.RequestId)),
                Player = Convert.ToInt32((await connection.HashGetAsync(redisKey, nameof(UserSession.Player))).ToString()),
                Timestamp = long.Parse((await connection.HashGetAsync(redisKey, nameof(UserSession.Timestamp))).ToString()),
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
