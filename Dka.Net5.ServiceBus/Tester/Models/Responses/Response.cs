namespace ServiceBusTester.Models.Responses
{
    public class Response
    {
        public string EventName { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}