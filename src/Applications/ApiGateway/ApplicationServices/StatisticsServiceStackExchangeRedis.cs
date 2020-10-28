using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EGT.ApiGateway.ApplicationServices;

using StackExchange.Redis;

namespace EGT.ApiGatewayGateway.ApplicationServices
{
    public class StatisticsServiceStackExchangeRedis : IStatisticsService
    {
        private readonly ConnectionMultiplexer _muxer;
        private readonly IDatabase _connection;

        public StatisticsServiceStackExchangeRedis(ConnectionMultiplexer muxer)
        {
            _muxer = muxer;
            _connection = _muxer.GetDatabase();
        }

        public async Task LogSessionPerUser(string userId, long sessionId, long sessionScore)
        {
            var userKey = new RedisKey(RedisKeysPrefixes.USER + ":" + userId);
            await _connection.SortedSetAddAsync(
                userKey,
                new SortedSetEntry[]
                {
                        new SortedSetEntry(new RedisValue(sessionId.ToString()), sessionScore)
                });
        }

        public async Task<List<long>> GetSessions(string userId)
        {
            var sessions = await _connection.SortedSetRangeByScoreAsync(new RedisKey(RedisKeysPrefixes.USER + ":" + userId));

            return sessions.Select(s => long.Parse(s)).ToList();
        }
    }
}
