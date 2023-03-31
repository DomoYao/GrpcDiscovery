using Consul.Provider.Grpc.Balancers;
using Enterprise.Core.Exceptions;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;

namespace Consul.Provider.Grpc
{
    public sealed class ConsulGrpcResolver : PollingResolver
    {
        private readonly Uri _address;
        private readonly int _port;
        private readonly ConsulClient _client;
        private Timer? _timer;
        private readonly TimeSpan _refreshInterval;
        private readonly ILogger _logger;

        public ConsulGrpcResolver(Uri address, int defaultPort, ConsulClient client, ILoggerFactory loggerFactory, TimeSpan refreshInterval)
            : base(loggerFactory)
        {
            _address = address;
            _port = defaultPort;
            _client = client;
            _logger = loggerFactory.CreateLogger<ConsulGrpcResolver>();
            _refreshInterval = refreshInterval;
        }

        protected override async Task ResolveAsync(CancellationToken cancellationToken)
        {
            try
            {
                var address = _address.Host.Replace("consul://", string.Empty);
                var _consulServiceProvider = new DiscoverProviderBuilder(_client).WithServiceName(address).WithCacheSeconds(5).Build();
                var results = await _consulServiceProvider.GetAllHealthServicesAsync();
                var balancerAddresses = new List<BalancerAddress>();
                results.ForEach(result =>
                {
                    var addressArray = result.Split(":");
                    var host = addressArray[0];
                    var port = int.Parse(addressArray[1]);
                    balancerAddresses.Add(new BalancerAddress(host, port));
                });
                // Pass the results back to the channel.
                Listener(ResolverResult.ForResult(balancerAddresses));
            }
            catch (Exception ex)
            {
                _logger.LogError("ConsulGrpcResolver.ResolveAsync", ex);
            }
        }

        protected override void OnStarted()
        {
            base.OnStarted();

            if (_refreshInterval != Timeout.InfiniteTimeSpan)
            {
                _timer = new Timer(OnTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                _timer.Change(_refreshInterval, _refreshInterval);
            }
        }

        private void OnTimerCallback(object? state)
        {
            try
            {
                Refresh();
            }
            catch (Exception ex)
            {
                _logger.LogError("ConsulGrpcResolver.OnTimerCallback", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _timer?.Dispose();
        }
    }

    public class ConsulGrpcResolverFactory : ResolverFactory
    {
        private readonly TimeSpan _refreshInterval;
        private readonly ConsulClient _consulClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulGrpcResolverFactory"/> class with a refresh interval.
        /// </summary>
        /// <param name="refreshInterval">An interval for automatically refreshing.</param>
        public ConsulGrpcResolverFactory(ConsulClient consulClient, TimeSpan refreshInterval)
        {
            _refreshInterval = refreshInterval;
            _consulClient = consulClient;
        }

        public override string Name => "consul";

        public override Resolver Create(ResolverOptions options) => new ConsulGrpcResolver(options.Address, options.DefaultPort, _consulClient, options.LoggerFactory, _refreshInterval);
    }
}
