using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ThreeLayersModernEventsSubscriber.Models.ServiceBus.Tenants.V1;

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
        
        _logger.LogWarning("Received topic/subscription {Event} event - raw data {RawData}", "unknown", messageAsJson.Replace(Environment.NewLine, string.Empty));
        await Task.Delay(0);
    }    
    
    [FunctionName("ProcessMessage1")]
    public async Task ProcessMessage1Async(
        [ServiceBusTrigger("events", "subscription1", Connection = "ServiceBus:ConnectionString")] string message)
    {
        // TODO: Processing logic goes here...
        
        _logger.LogWarning("Received topic/subscription {Event} event - raw data {RawData}", "unknown", message.Replace(Environment.NewLine, string.Empty));
        await Task.Delay(0);
    }
    
    [FunctionName("TenantUpdatedEventProcessor")]
    public async Task TenantUpdatedEventProcessor(
        [ServiceBusTrigger("events", "subscription2", Connection = "ServiceBus:ConnectionString")] TenantUpdatedEvent message)
    {
        // TODO: Processing logic goes here...
        
        _logger.LogWarning("Received topic/subscription {Event} event - raw data {RawData}", nameof(TenantUpdatedEvent), JsonConvert.SerializeObject(message));
        await Task.Delay(0);
    }
    
    [FunctionName("TenantCreatedEventProcessor")]
    public async Task ProcessMessage2Async(
        [ServiceBusTrigger("events", "subscription2", Connection = "ServiceBus:ConnectionString")] TenantCreatedEvent message)
    {
        // TODO: Processing logic goes here...
        
        _logger.LogWarning("Received topic/subscription {Event} event - raw data {RawData}", nameof(TenantCreatedEvent), JsonConvert.SerializeObject(message));
        await Task.Delay(0);
    }
    
    [FunctionName("ProcessMessage3")]
    public async Task ProcessMessage3Async(
        [ServiceBusTrigger("events", "subscription3", Connection = "ServiceBus:ConnectionString")] TenantUpdatedEvent message)
    {
        throw new Exception("Test");
    }
    
    [FunctionName("ProcessMessage4")]
    public async Task ProcessMessage4Async(
        [ServiceBusTrigger("events", "subscription4", Connection = "ServiceBus:ConnectionString", AutoCompleteMessages = false)] ServiceBusReceivedMessage message, 
        ServiceBusMessageActions messageActions)
    {
        // TODO: Processing logic goes here...
        
        _logger.LogWarning("Received topic/subscription {Event} event - raw data {RawData}", nameof(ServiceBusReceivedMessage), JsonConvert.SerializeObject(message));
        await Task.Delay(0);

        await messageActions.CompleteMessageAsync(message);
    }
}