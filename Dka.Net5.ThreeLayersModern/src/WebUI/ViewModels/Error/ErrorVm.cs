namespace WebUI.ViewModels.Error
{
    public class ErrorVm
    {
        public int StatusCode { get; set; }
        public string RequestId { get; set; }
        public string TraceId { get; set; }
        public string Path { get; set; }
    }
}