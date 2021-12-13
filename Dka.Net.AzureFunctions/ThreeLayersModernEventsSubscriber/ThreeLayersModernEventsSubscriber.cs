using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

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
        [ServiceBusTrigger("events", "threelayersmoderneventssubscriber", Connection = "ServiceBus:ConnectionString")] string messageAsJson)
    {
        // TODO: Processing logic goes here...

        await using (File.Create($"test-{DateTime.Now.Ticks}.txt")) {}
        
        _logger.LogInformation("Received topic/subscription {Event} event - raw data {RawData}", "unknown", messageAsJson.Replace(Environment.NewLine, string.Empty));
    }
}