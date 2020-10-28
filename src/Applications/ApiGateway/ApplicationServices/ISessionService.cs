using EGT.ApiGateway.DomainModels;
using System.Threading.Tasks;

namespace EGT.ApiGateway.ApplicationServices
{
    public interface ISessionService
    {
        Task<(UserSession, bool)> CreateSession(UserSession userSession, int ttlInSeconds);

        Task<UserSession> GetSession(long sessionId);
    }
}