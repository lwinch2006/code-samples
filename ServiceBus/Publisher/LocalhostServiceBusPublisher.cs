using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServiceBusMessages;

namespace ServiceBusPublisher;

public class LocalhostServiceBusPublisher : IServiceBusPublisher
{
	private readonly ILogger<LocalhostServiceBusPublisher> _logger;

	public LocalhostServiceBusPublisher(ILogger<LocalhostServiceBusPublisher> logger)
	{
		_logger = logger;
	}

	public Task SendMessage(string queueOrTopicName, ServiceBusReceivedMessage message,
		CancellationToken cancellationToken = default)
	{
		throw new System.NotImplementedException();
	}

	public Task SendMessage<T>(string queueOrTopicName, ServiceBusMessage<T> message, CancellationToken cancellationToken = default) where T : class
	{
		var messageAsJson = JsonConvert.SerializeObject(message, GetJsonSerializerSettings());
		_logger.LogInformation("New message registered to be sent to queue (topic) {QueueOrTopicName} with body {Body}", queueOrTopicName, messageAsJson);
            
		return Task.CompletedTask;
	}

	public async Task SendMessages<T>(string queueOrTopicName, IEnumerable<ServiceBusMessage<T>> messages, CancellationToken cancellationToken = default) where T : class
	{
		foreach (var message in messages)
		{
			await SendMessage(queueOrTopicName, message, cancellationToken);
		}
	}

	public Task EnsureTopic(string topicName, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Topic {TopicName} registered to be ensured created", topicName);
		return Task.CompletedTask;
	}

	public Task EnsureQueue(string queueName, CancellationToken cancellationToken = default)
	{
		throw new System.NotImplementedException();
	}

	private JsonSerializerSettings GetJsonSerializerSettings()
	{
		var contractResolver = new DefaultContractResolver
		{
			NamingStrategy = new CamelCaseNamingStrategy()
		};

		var jsonSerializerSettings = new JsonSerializerSettings
		{
			ContractResolver = contractResolver
		};

		return jsonSerializerSettings;
	}        
}