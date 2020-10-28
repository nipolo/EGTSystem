using System.Collections.Generic;
using System.Threading.Tasks;

namespace EGT.ApiGatewayGateway.ApplicationServices
{
    public interface IStatisticsService
    {
        Task<List<long>> GetSessions(string userId);

        Task LogSessionPerUser(string userId, long sessionId, long sessionScore);
    }
}