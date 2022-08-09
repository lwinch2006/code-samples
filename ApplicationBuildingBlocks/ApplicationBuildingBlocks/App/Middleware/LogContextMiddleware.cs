namespace ApplicationBuildingBlocks.App.Middleware
{
    public class LogContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogContextMiddleware> _logger;
        
        public LogContextMiddleware(RequestDelegate next, ILogger<LogContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = context.TraceIdentifier;
            var traceId = Activity.Current?.Id ?? string.Empty;

            var properties = new Dictionary<string, object>
            {
                {"RequestId", requestId},
                {"TraceId", traceId}
            };
            
            using (_logger.BeginScope(properties))
            {
                await _next(context);
            }
        }        
    }
    
    public static class LogContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogContextMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogContextMiddleware>();
        }
    }
}