using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BeetleX.Redis;

namespace EGT.ApiGateway.ApplicationServices
{
    public class StatisticsServiceBeetleXRedis : IStatisticsService
    {
        private readonly RedisDB _redisDB;

        public StatisticsServiceBeetleXRedis(RedisDB redisDB)
        {
            _redisDB = redisDB;
        }

        public async Task LogSessionPerUser(string userId, long sessionId, long sessionScore)
        {
            var userKey = RedisKeysPrefixes.USER + ":" + userId;

            var sequence = _redisDB.CreateSequence(userKey);
            await sequence.ZAdd((sessionScore, sessionId.ToString()));
        }

        public async Task<List<long>> GetSessions(string userId)
        {
            var userKey = RedisKeysPrefixes.USER + ":" + userId;
            
            var sequence = _redisDB.CreateSequence(userKey);
            var sessions = await sequence.ZRange(0, -1);

            return sessions.Select(s => long.Parse(s.Member)).ToList();
        }
    }
}
