namespace ServiceBusTester.Models.Responses
{
    public class Response
    {
        public string EventName { get; init; } = default!;
        public int StatusCode { get; init; }
        public string Message { get; init; } = default!;
    }
}