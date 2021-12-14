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

        const string fileName = "test-123.txt";
        
        
        
        _logger.LogInformation("Directory (AppDirectory): {Directory}", _executionContextOptions.AppDirectory);
        
        _logger.LogInformation("Directory (Current): {Directory}", Directory.GetCurrentDirectory());
        
        
        
        
        
        
        
        
        _logger.LogInformation("Check first file exists: {IsExists}", File.Exists(fileName).ToString());

        if (!File.Exists(fileName))
        {
            await File.WriteAllTextAsync(fileName, "Hello World!");
        }
        
        _logger.LogInformation("Check second file exists: {IsExists}", File.Exists(fileName).ToString());

        var info = new FileInfo(fileName);
        _logger.LogInformation("Fileinfo full file name: {FullFileName}", info.FullName);
        
        
        
        
        
        
        
        
        
        const string dir = "/home/LogFiles/Application/Functions/Function/ThreeLayersModernEventsSubscriber";

        _logger.LogInformation("Check dir exists: {IsExists}", Directory.Exists(dir).ToString());

        if (Directory.Exists(dir))
        {
            var fullFileName = Path.Combine(dir, fileName);
            
            _logger.LogInformation("Full path: {FullPath}", fullFileName);
            
            _logger.LogInformation("Check third file exists: {IsExists}", File.Exists(fullFileName).ToString());

            if (!File.Exists(fullFileName))
            {
                await File.WriteAllTextAsync(fullFileName, "Hello World!");
            }
        
            _logger.LogInformation("Check forth file exists: {IsExists}", File.Exists(fullFileName).ToString());
        }
        
        
        
        
        
        
        
        
        
        _logger.LogInformation("Received topic/subscription {Event} event - raw data {RawData}", "unknown", messageAsJson.Replace(Environment.NewLine, string.Empty));
    }
}