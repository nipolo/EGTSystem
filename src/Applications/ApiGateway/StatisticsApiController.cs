using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using EGT.ApiGateway.ApplicationServices;

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
        public async Task<ActionResult<List<int>>> GetSessions(string userId)
        {
            var sessions = await _statisticsService.GetSessions(userId);

            return Ok(sessions);
        }
    }
}
