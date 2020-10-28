using System;
using System.Threading;
using System.Threading.Tasks;

using EGT.ApiGateway.ApplicationServices;
using EGT.ApiGateway.DomainModels;

using Xunit;
using BeetleX.Redis;
using StackExchange.Redis;
using EGT.ApiGatewayGateway.ApplicationServices;
using System.Collections.Generic;

namespace EGT.ApiGateway.Tests
{
    public class StatisticsServiceTests
    {
        private readonly RedisDB _redisDB;
        private readonly ConnectionMultiplexer _muxer;
        private readonly StatisticsServiceBeetleXRedis _statisticsServiceBeetleXRedis;
        private readonly StatisticsServiceStackExchangeRedis _statisticsServiceStackExchangeRedis;

        public StatisticsServiceTests()
        {
            _redisDB = DefaultRedis.Instance;
            _redisDB.DataFormater = new JsonFormater();
            _redisDB.Host.AddWriteHost("redisTestServer", 6381);
            _redisDB.Flushall();
            _statisticsServiceBeetleXRedis = new StatisticsServiceBeetleXRedis(_redisDB);

            _muxer = ConnectionMultiplexer.Connect("redisTestServer:6381");
            _statisticsServiceStackExchangeRedis = new StatisticsServiceStackExchangeRedis(_muxer);
        }

        [Fact]
        public async Task AddSessionForUserThroughStackExchangeRedisClient()
        {
            await Session_AddToUser_CheckEverythingIsAddedCorrect(_statisticsServiceStackExchangeRedis);
        }

        [Fact]
        public async Task AddSessionForUserThroughBeetleXRedisClient()
        {
            await Session_AddToUser_CheckEverythingIsAddedCorrect(_statisticsServiceBeetleXRedis);
        }

        private async Task Session_AddToUser_CheckEverythingIsAddedCorrect(IStatisticsService _statisticsService)
        {
            // Arrange
            var userId = 111;
            var sessionIds = new List<long>() { 22222, 33333, 4444 };
            // Act
            await _statisticsService.LogSessionPerUser(userId.ToString(), sessionIds[0], 1);
            await _statisticsService.LogSessionPerUser(userId.ToString(), sessionIds[1], 3);
            await _statisticsService.LogSessionPerUser(userId.ToString(), sessionIds[2], 2);

            Thread.Sleep(1010);
            // Assert
            var sessionIdsFromDb = await _statisticsService.GetSessions(userId.ToString());

            Assert.NotEmpty(sessionIdsFromDb);
            Assert.Equal(sessionIds.Count, sessionIdsFromDb.Count);
            Assert.Equal(sessionIds[0], sessionIdsFromDb[0]);
            Assert.Equal(sessionIds[1], sessionIdsFromDb[2]);
            Assert.Equal(sessionIds[2], sessionIdsFromDb[1]);
        }
    }
}
