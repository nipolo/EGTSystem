using System;
using Microsoft.AspNetCore.Mvc;

using EGT.ApiGateway.DomainModels;
using EGT.ApiGateway.Dto;
using EGT.ApiGateway.ApplicationServices;
using EGT.ApiGatewayGateway.ApplicationServices;

namespace EGT.ApiGateway
{
    [ApiController]
    public class JsonApiController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IStatisticsService _statisticsService;
        private readonly SessionConfiguration _sessionConfiguration;

        public JsonApiController(
            ISessionService sessionService,
            IStatisticsService statisticsService,
            SessionConfiguration sessionConfiguration)
        {
            _sessionService = sessionService;
            _statisticsService = statisticsService;
            _sessionConfiguration = sessionConfiguration;
        }

        [HttpPost("json_api/insert")]
        public ActionResult Insert(InsertSessionJsonDto sessionCommand)
        {
            var userSession = new UserSession()
            {
                RequestId = sessionCommand.RequestId,
                Player = int.Parse(sessionCommand.ProducerId),
                SessionId = sessionCommand.SessionId,
                Timestamp = sessionCommand.Timestamp
            };
            (_, var isNew)= _sessionService.CreateSession(userSession, _sessionConfiguration.TTL);

            if (!isNew)
            {
                return Ok(new JsonApiError() { ErrorCode = ErrorCodeEnum.SessionAlreadyExists });
            }

            _statisticsService.LogSessionPerUser(sessionCommand.ProducerId, sessionCommand.SessionId, DateTimeOffset.Now.ToUnixTimeSeconds() + _sessionConfiguration.TTL);

            return Ok();
        }

        [HttpPost("json_api/find")]
        public ActionResult Find(FindSessionJsonDto findSessionCommand)
        {
            var userSession = _sessionService.GetSession(findSessionCommand.SessionId);

            return Ok(userSession);
        }
    }

    public class JsonApiError
    {
        public ErrorCodeEnum ErrorCode { get; set; }
    }

    public enum ErrorCodeEnum
    {
        SessionAlreadyExists = 0
    }
}
