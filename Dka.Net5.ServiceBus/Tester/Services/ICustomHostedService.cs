using System.Threading;

namespace ServiceBusTester.Services
{
    public interface ICustomHostedService
    {
        CancellationToken CancellationToken { get; }
    }
}