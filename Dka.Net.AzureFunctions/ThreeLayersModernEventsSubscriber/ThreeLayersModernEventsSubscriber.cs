using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ThreeLayersModernEventsSubscriber;

public class ThreeLayersModernEventsSubscriber
{
    private readonly ILogger<ThreeLayersModernEventsSubscriber> _logger;
    
    public ThreeLayersModernEventsSubscriber(ILogger<ThreeLayersModernEventsSubscriber> logger)
    {
        _logger = logger;
    }
    
    [FunctionName("ThreeLayersModernEventsSubscriber")]
    public async Task RunAsync(
        [ServiceBusTrigger("events", "threelayersmoderneventssubscriber", Connection = "ServiceBus:ConnectionString")] string messageAsJson, ILogger log)
    {
        // TODO: Processing logic goes here...
        
        _logger.LogInformation("(1) Received topic/subscription {Event} event - raw data {RawData}", "unknown", messageAsJson.Replace(Environment.NewLine, string.Empty));
        log.LogInformation("(2) Received topic/subscription {Event} event - raw data {RawData}", "unknown", messageAsJson.Replace(Environment.NewLine, string.Empty));
        Log.Information("(3) Received topic/subscription {Event} event - raw data {RawData}", "unknown", messageAsJson.Replace(Environment.NewLine, string.Empty));
    }
}