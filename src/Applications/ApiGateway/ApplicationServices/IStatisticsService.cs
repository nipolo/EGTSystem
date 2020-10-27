using System.Collections.Generic;

namespace EGT.ApiGatewayGateway.ApplicationServices
{
    public interface IStatisticsService
    {
        List<long> GetSessions(string userId);

        void LogSessionPerUser(string userId, long sessionId, long sessionScore);
    }
}