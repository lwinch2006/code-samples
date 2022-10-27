using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusTester.Logic;
using ServiceBusTester.Models;

namespace ServiceBusTester.Services
{
    public class ServiceA : BackgroundService, ICustomHostedService
    {
        private readonly ITalentechAdminServiceBusClient _talentechAdminServiceBusClient;
        private readonly ILogger<ServiceA> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource = default!;

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public ServiceA(
            ILogger<ServiceA> logger,
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
                _logger.LogInformation("{ServiceName} cancellation token triggered", nameof(ServiceA)));

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
            //var sendTenantEventsToQueueTask = _talentechAdminServiceBusClient.SendTenantChangedEvent("tenantevents");
            //var sendUserEventsToQueueTask = _talentechAdminServiceBusClient.SendUserChangedEvent("tenantevents");
            var sendTenantEventsToTopicTask = _talentechAdminServiceBusClient.SendTenantChangedEvent("events");
            //var sendUserEventsToTopicTask = _talentechAdminServiceBusClient.SendUserChangedEvent("events");
            //var sendRequestAndWaitForResponse = ServiceBusRequestResponseTest();
            

            await Task.WhenAll(
                sendTenantEventsToTopicTask
            );

            _logger.LogInformation("{ServiceName} finished work", nameof(ServiceA));
            _cancellationTokenSource.Cancel();
        }

        private async Task ServiceBusRequestResponseTest()
        {
            var test = new Stopwatch();
            test.Start();
            
            for (var i = 0; i < 1; i++)
            {
                await _talentechAdminServiceBusClient.SendRequestAndWaitForResponse(AppConstants.ServiceBus.Publish.RequestQueue1, AppConstants.ServiceBus.Receive.ResponseQueue1);
            }
            
            test.Stop();
            _logger.LogInformation("{Function} run time is {Time} seconds", nameof(ServiceBusRequestResponseTest), test.Elapsed.TotalSeconds / 10.0);
        }
    }
}