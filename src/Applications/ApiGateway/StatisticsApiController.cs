using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using EGT.ApiGatewayGateway.ApplicationServices;

namespace EGT.ApiGateway
{
    [ApiController]
    public class StatisticsApiController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsApiController(
            IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("statistics_api/get_sessions")]
        public ActionResult<List<int>> GetSessions(string userId)
        {
            var sessions = _statisticsService.GetSessions(userId);

            return Ok(sessions);
        }
    }
}
