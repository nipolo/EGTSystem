using System;
using System.Threading;
using System.Threading.Tasks;

using EGT.ApiGateway.ApplicationServices;
using EGT.ApiGateway.DomainModels;

using Xunit;
using BeetleX.Redis;
using StackExchange.Redis;

namespace EGT.ApiGateway.Tests
{
    public class SessionServiceTests : IDisposable
    {
        private readonly RedisDB _redisDB;
        private readonly ConnectionMultiplexer _muxer;
        private readonly SessionServiceBeetleXRedis _sessionServiceBeetleXRedis;
        private readonly SessionServiceStackExchangeRedis _sessionServiceStackExchangeRedis;

        public SessionServiceTests()
        {
            _redisDB = DefaultRedis.Instance;
            _redisDB.DataFormater = new JsonFormater();
            _redisDB.Host.AddWriteHost("redisTestServer",6381);
            _redisDB.Flushall();
            _sessionServiceBeetleXRedis = new SessionServiceBeetleXRedis(_redisDB);

            _muxer = ConnectionMultiplexer.Connect("redisTestServer:6381");
            _sessionServiceStackExchangeRedis = new SessionServiceStackExchangeRedis(_muxer);
        }

        [Fact]
        public async Task AddSessionThroughStackExchangeRedisClient()
        {
            await Session_Add_CheckEverythingIsAddedCorrect(_sessionServiceStackExchangeRedis);
        }

        [Fact]
        public async Task AddSessionThroughBeetleXRedisClient()
        {
            await Session_Add_CheckEverythingIsAddedCorrect(_sessionServiceBeetleXRedis);
        }

        private async Task Session_Add_CheckEverythingIsAddedCorrect(ISessionService sessionService)
        {
            // Arrange
            var userSession = new UserSession()
            {
                Player = 238485,
                RequestId = "1234",
                SessionId = 13617162,
                Timestamp = 1586335186721
            };

            // Act
            await sessionService.CreateSession(userSession, 1000);

            // Assert
            var sessionFromDb = await sessionService.GetSession(userSession.SessionId);

            Assert.NotNull(sessionFromDb);
            Assert.Equal(userSession.Player, sessionFromDb.Player);
            Assert.Equal(userSession.RequestId, sessionFromDb.RequestId);
            Assert.Equal(userSession.SessionId, sessionFromDb.SessionId);
            Assert.Equal(userSession.Timestamp, sessionFromDb.Timestamp);
        }

        [Fact]
        public async Task AddSessionAndWaitThroughStackExchangeRedisClient()
        {
            await Session_AddAndWait_CheckSessionHasExpired(_sessionServiceStackExchangeRedis);
        }

        [Fact]
        public async Task AddSessionAndWaitThroughBeetleXRedisClient()
        {
            await Session_AddAndWait_CheckSessionHasExpired(_sessionServiceBeetleXRedis);
        }

        private async Task Session_AddAndWait_CheckSessionHasExpired(ISessionService sessionService)
        {
            // Arrange
            var userSession = new UserSession()
            {
                Player = 238485,
                RequestId = "1234",
                SessionId = 13617162,
                Timestamp = 1586335186721
            };

            // Act
            await sessionService.CreateSession(userSession, 1);
            Thread.Sleep(1010);

            // Assert
            var session = await sessionService.GetSession(userSession.SessionId);

            Assert.Null(session);
        }

        public void Dispose()
        {
            _redisDB.Dispose();
            _muxer.Dispose();
        }
    }
}
