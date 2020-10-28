using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using EGT.ApiGateway.ApplicationServices;
using EGT.ApiGateway.DomainModels;
using EGT.ApiGateway.Dto;

using Xunit;
using BeetleX.Redis;
using Moq;

namespace EGT.ApiGateway.Tests
{
    public class ApiGatewayControllersTests
    {
        private readonly JsonApiController _jsonApiController;
        private readonly XMLApiController _xmlApiController;
        private readonly StatisticsApiController _statisticsApiController;

        public ApiGatewayControllersTests()
        {
            var redisDB = DefaultRedis.Instance;
            redisDB.DataFormater = new JsonFormater();
            redisDB.Host.AddWriteHost("redisTestServer", 6381);
            redisDB.Flushall();

            var statisticsServiceBeetleXRedis = new StatisticsServiceBeetleXRedis(redisDB);
            var sessionServiceBeetleXRedis = new SessionServiceBeetleXRedis(redisDB);
            var sessionConfiguration = new SessionConfiguration() { TTL = 1000 };

            _jsonApiController = new JsonApiController(sessionServiceBeetleXRedis, statisticsServiceBeetleXRedis, sessionConfiguration);
            _xmlApiController = new XMLApiController(sessionServiceBeetleXRedis, statisticsServiceBeetleXRedis, sessionConfiguration);
            _statisticsApiController = new StatisticsApiController(statisticsServiceBeetleXRedis);
        }

        [Fact]
        public async Task Session_Insert_Find()
        {
            // Arrange
            var insertSessionJsonDto = new InsertSessionJsonDto()
            {
                ProducerId = "1111",
                RequestId = "2222",
                SessionId = 3333,
                Timestamp = 444444444444
            };

            // Act
            await _jsonApiController.Insert(insertSessionJsonDto);

            // Assert
            var findSessionJsonDto = new FindSessionJsonDto()
            {
                RequestId = insertSessionJsonDto.RequestId,
                SessionId = insertSessionJsonDto.SessionId
            };

            var findActionResult = await _jsonApiController.Find(findSessionJsonDto);
            var userSession = (UserSession)((OkObjectResult)findActionResult).Value;

            Assert.Equal(insertSessionJsonDto.ProducerId, userSession.Player.ToString());
            Assert.Equal(insertSessionJsonDto.RequestId, userSession.RequestId);
            Assert.Equal(insertSessionJsonDto.SessionId, userSession.SessionId);
            Assert.Equal(insertSessionJsonDto.Timestamp, userSession.Timestamp);

            // Asserts for StatisticsApiController

            var getSessionsActionResult = (await _statisticsApiController.GetSessions(insertSessionJsonDto.ProducerId)).Result;
            var sessions = (List<long>)((OkObjectResult)getSessionsActionResult).Value;

            Assert.Single(sessions);
            Assert.Equal(insertSessionJsonDto.SessionId, sessions[0]);
        }

        [Fact]
        public async Task Session_AddXMLCommand_GetXMLCommand()
        {
            // Arrange
            var enterSessionString = "<command id=\"1234\">" +
                                        "<enter session = \"13617162\">" +
                                            "<timestamp>1586335186721</timestamp>" +
                                            "<player>238485</player>" +
                                        "</enter>" +
                                     "</command>";
            var insertSessionJson = new UserSession()
            {
                Player = 238485,
                RequestId = "1234",
                SessionId = 13617162,
                Timestamp = 1586335186721
            };

            // Act
            _xmlApiController.ControllerContext = new ControllerContext();
            _xmlApiController.ControllerContext.HttpContext = new DefaultHttpContext();
            _xmlApiController.ControllerContext.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(enterSessionString));

            await _xmlApiController.PostCommand();

            // Assert
            var getSessionString = "<command id=\"1234-8785\">" +
                                       "<get session=\"13617162\" />" +
                                   "</command>";

            _xmlApiController.ControllerContext = new ControllerContext();
            _xmlApiController.ControllerContext.HttpContext = new DefaultHttpContext();
            _xmlApiController.ControllerContext.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(getSessionString));

            var getSessionActionResult = await _xmlApiController.PostCommand();

            var userSession = (UserSession)((OkObjectResult)getSessionActionResult).Value;

            Assert.Equal(insertSessionJson.Player, userSession.Player);
            Assert.Equal(insertSessionJson.RequestId, userSession.RequestId);
            Assert.Equal(insertSessionJson.SessionId, userSession.SessionId);
            Assert.Equal(insertSessionJson.Timestamp, userSession.Timestamp);

            // Asserts for StatisticsApiController

            var getSessionsActionResult = (await _statisticsApiController.GetSessions(insertSessionJson.Player.ToString())).Result;
            var sessions = (List<long>)((OkObjectResult)getSessionsActionResult).Value;

            Assert.Single(sessions);
            Assert.Equal(insertSessionJson.SessionId, sessions[0]);
        }
    }
}
