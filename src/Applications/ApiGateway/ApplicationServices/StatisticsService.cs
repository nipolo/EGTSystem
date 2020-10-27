using System.Collections.Generic;
using System.Linq;

using EGT.ApiGateway.ApplicationServices;

using StackExchange.Redis;

namespace EGT.ApiGatewayGateway.ApplicationServices
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ConnectionMultiplexer _muxer;
        private readonly IDatabase _connection;

        public StatisticsService(ConnectionMultiplexer muxer)
        {
            _muxer = muxer;
            _connection = _muxer.GetDatabase();
        }

        public void LogSessionPerUser(string userId, long sessionId, long sessionScore)
        {
            var userKey = new RedisKey(RedisKeysPrefixes.USER + ":" + userId);
            _connection.SortedSetAdd(
                userKey,
                new SortedSetEntry[]
                {
                        new SortedSetEntry(new RedisValue(sessionId.ToString()), sessionScore)
                });
        }

        public List<long> GetSessions(string userId)
        {
            var sessions = _connection.SortedSetRangeByScore(new RedisKey(RedisKeysPrefixes.USER + ":" + userId));

            return sessions.Select(s => long.Parse(s)).ToList();
        }
    }
}
