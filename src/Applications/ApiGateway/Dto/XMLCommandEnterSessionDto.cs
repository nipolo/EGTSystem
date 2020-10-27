namespace EGT.ApiGateway.Dto
{
    public class XMLCommandEnterSessionDto : XMLCommandDtoBase
    {
        public int SessionId { get; set; }

        public long Timestamp { get; set; }

        public int Player { get; set; }
    }
}
