using dotnet_etcd;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Etcd.Provider
{
    public sealed class EtcdGrpcResolver : PollingResolver
    {
        private readonly Uri _address;
        private readonly EtcdClient _etcdClient;
        private readonly ILogger _logger;
        private string keyPrefix; // etcd 服务组前缀.区分大小写
        private string serviceAddressCacheKey; // etcd 服务组前缀.区分大小写

        public EtcdGrpcResolver(Uri address, EtcdClient client, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            _address = address;
            _etcdClient = client;
            _logger = loggerFactory.CreateLogger<EtcdGrpcResolver>();
            keyPrefix = $"/{_address.OriginalString.Replace("etcd://", string.Empty).Trim('/')}"; // etdc://{服务名}，如，etcd://grpcTest
            serviceAddressCacheKey = $"service_etcd_{keyPrefix}";
        }

        protected override async Task ResolveAsync(CancellationToken cancellationToken)
        {
            try
            {
                var balancerAddresses = await GetBalancerAddressesAsync();
                // Pass the results back to the channel.
                Listener(ResolverResult.ForResult(balancerAddresses));
            }
            catch (Exception ex)
            {
                _logger.LogError("EtcdGrpcResolver.ResolveAsync", ex);
            }
        }

        private static readonly SemaphoreSlim _slimlock = new(1, 1);
        private static readonly IMemoryCache _memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions
        {
            CompactionPercentage = 0.05,
            ExpirationScanFrequency = new TimeSpan(0, 0, 1),
        }));

        /// <summary>
        /// 获取服务地址列表的变化
        /// </summary>
        /// <returns></returns>
        private async Task<List<BalancerAddress>> GetBalancerAddressesAsync()
        {
            var healthAddresses = _memoryCache.Get<List<BalancerAddress>>(serviceAddressCacheKey);
            if (healthAddresses != null && healthAddresses.Any())
            {
                return healthAddresses;
            }

            await _slimlock.WaitAsync();
            try
            {
                healthAddresses = _memoryCache.Get<List<BalancerAddress>>(serviceAddressCacheKey);
                if (healthAddresses != null && healthAddresses.Any())
                {
                    return healthAddresses;
                }

                var balancerAddresses = new List<BalancerAddress>();
                var res = await _etcdClient.GetRangeAsync(keyPrefix);
                if (res?.Count > 0)
                {
                    foreach (var item in res.Kvs)
                    {
                        Uri itemUri = new Uri(item.Value.ToStringUtf8());
                        balancerAddresses.Add(new BalancerAddress(itemUri.Host, itemUri.Port));
                    }

                    var entryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                    };
                    _memoryCache.Set(serviceAddressCacheKey, healthAddresses, entryOptions);
                }

                return balancerAddresses;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetBalancerAddressesAsync.Error", ex);
                throw;
            }
            finally
            {
                _slimlock.Release();
            }
        }


        protected override void OnStarted()
        {
            base.OnStarted();
            Watch().ConfigureAwait(false).GetAwaiter();
        }

        /// <summary>
        /// watch机制：监听etcd中某个key前缀的服务地址列表的变化
        /// 这里只要变化就重新刷新获取最新地址.
        /// </summary>
        private async Task Watch()
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                    await _etcdClient.WatchRangeAsync(keyPrefix, new Action<Etcdserverpb.WatchResponse>(target =>
                    {
                        if (target.Events.Any())
                        {
                            _memoryCache.Remove(serviceAddressCacheKey);
                            Refresh();
                        }
                    })), default(CancellationToken), TaskCreationOptions.LongRunning, TaskScheduler.Default
                ); 
            }
            catch (Exception ex)
            {
                _logger.LogError("EtcdGrpcResolver.OnStarted", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public class EtcdGrpcResolverFactory : ResolverFactory
    {
        private readonly EtcdClient _etcdClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtcdGrpcResolverFactory"/> class with a refresh interval.
        /// </summary>
        public EtcdGrpcResolverFactory(EtcdClient etcdClient)
        {
            _etcdClient = etcdClient;
        }

        public override string Name => "etcd";

        public override Resolver Create(ResolverOptions options) => new EtcdGrpcResolver(options.Address, _etcdClient, options.LoggerFactory);
    }
}
