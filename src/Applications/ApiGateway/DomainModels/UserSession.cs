namespace EGT.ApiGateway.DomainModels
{
    public class UserSession
    {
        public string RequestId { get; set; }

        // Best practice for session id is to be of type Guid + Digital signiture and is not ok to be long due to possible guess attack.
        public long SessionId { get; set; }

        public long Timestamp { get; set; }

        public int Player { get; set; }
    }
}
