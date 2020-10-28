using System.Collections.Generic;
using System.Threading.Tasks;

namespace EGT.ApiGateway.ApplicationServices
{
    public interface IStatisticsService
    {
        Task<List<long>> GetSessions(string userId);

        Task LogSessionPerUser(string userId, long sessionId, long sessionScore);
    }
}