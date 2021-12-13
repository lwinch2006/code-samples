using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ThreeLayersModernEventsSubscriber;

public class ThreeLayersModernEventsSubscriber
{
    private readonly ILogger<ThreeLayersModernEventsSubscriber> _logger;
    private readonly ExecutionContextOptions _executionContextOptions;
    
    public ThreeLayersModernEventsSubscriber(ILogger<ThreeLayersModernEventsSubscriber> logger, IOptions<ExecutionContextOptions> executionContextOptions)
    {
        _logger = logger;
        _executionContextOptions = executionContextOptions.Value;
    }
    
    [FunctionName("ThreeLayersModernEventsSubscriber")]
    public async Task RunAsync(
        [ServiceBusTrigger("events", "threelayersmoderneventssubscriber", Connection = "ServiceBus:ConnectionString")] string messageAsJson)
    {
        // TODO: Processing logic goes here...
        
        _logger.LogInformation("Current directory: {Directory}", _executionContextOptions.AppDirectory);
        
        _logger.LogInformation("Received topic/subscription {Event} event - raw data {RawData}", "unknown", messageAsJson.Replace(Environment.NewLine, string.Empty));
    }
}