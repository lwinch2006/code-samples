using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusTester.Logic;
using ServiceBusTester.Models;

namespace ServiceBusTester.Services
{
    public class ServiceB : BackgroundService, ICustomHostedService
    {
        private readonly ITalentechAdminServiceBusClient _talentechAdminServiceBusClient;
        private readonly ILogger<ServiceB> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource = default!;

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public ServiceB(
            ILogger<ServiceB> logger,
            IHostApplicationLifetime hostApplicationLifetime,
            ITalentechAdminServiceBusClient talentechAdminServiceBusClient)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _talentechAdminServiceBusClient = talentechAdminServiceBusClient;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Init(cancellationToken);
            return Task.CompletedTask;
        }

        private void Init(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _cancellationToken.Register(() =>
                _logger.LogInformation("{ServiceName} cancellation token triggered", nameof(ServiceB)));

            _hostApplicationLifetime.ApplicationStarted.Register(HostStarted);
            _hostApplicationLifetime.ApplicationStopping.Register(HostStopping);
            _hostApplicationLifetime.ApplicationStopped.Register(HostStopped);

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void HostStarted()
        {
            Task.Run(DoWork);
        }

        private void HostStopping()
        {
            _cancellationTokenSource.Cancel();
        }

        private void HostStopped()
        {
        }

        private async Task DoWork()
        {
            var receiveEventsFromQueue = _talentechAdminServiceBusClient.ReceiveEventsFromQueue("tenantevents");
            // var receiveEventsFromTopicSubscription1 = _talentechAdminServiceBusClient.ReceiveEventsFromTopicSubscription("events", "servicebustester");
            // var receiveEventsFromTopicSubscription2 = _talentechAdminServiceBusClient.ReceiveEventsFromTopicSubscription("events", "servicebustester2");
            
            var startReceiveMessages = _talentechAdminServiceBusClient.StartReceiveMessages("events", "servicebustester2", CancellationToken.None);
            var startReceiveMessagesFromDeadLetterQueue = _talentechAdminServiceBusClient.StartReceiveMessagesFromDeadLetterQueue("events", "servicebustester2", CancellationToken.None);
            
            //var receiveRequestAndSendResponse = _talentechAdminServiceBusClient.ReceiveRequestsAndSendResponses(AppConstants.ServiceBus.Publish.RequestQueue1, AppConstants.ServiceBus.Receive.ResponseQueue1);

            await Task.WhenAll(
                receiveEventsFromQueue,
                startReceiveMessages,
                startReceiveMessagesFromDeadLetterQueue
            );

            _logger.LogInformation("{ServiceName} finished work", nameof(ServiceB));
            _cancellationTokenSource.Cancel();
        }
    }
}