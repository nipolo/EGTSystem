using EGT.ApiGateway.DomainModels;

namespace EGT.ApiGateway.ApplicationServices
{
    public interface ISessionService
    {
        (UserSession, bool) CreateSession(UserSession userSession, int ttlInSeconds);

        UserSession GetSession(long sessionId);
    }
}