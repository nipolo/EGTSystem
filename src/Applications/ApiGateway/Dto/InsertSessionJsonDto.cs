namespace EGT.ApiGateway.Dto
{
    public class InsertSessionJsonDto
    {
        public string RequestId { get; set; }

        public long Timestamp { get; set; }

        public string ProducerId { get; set; }

        public long SessionId { get; set; }
    }
}
