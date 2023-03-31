using Etcd.Provider;
using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GrpcClientTest.Base
{
    public static class AppBaseService
    {
        /// <summary>
        /// 注册Grpc服务(跨微服务之间的同步通讯)
        /// </summary>
        /// <typeparam name="etcdKeyPrefix">Etcd前缀，http://{前缀}</typeparam>
        public static IServiceCollection AddEtcdGrpcClientAndAddMessageHandler<TGrpcClient>(this IServiceCollection services, string etcdKeyPrefix)
         where TGrpcClient : class
        {
            var baseAddress = etcdKeyPrefix.Replace("http://", "etcd://").Replace("https://", "etcd://");
            services.TryAddSingleton<ResolverFactory, EtcdGrpcResolverFactory>();
            services.AddGrpcClient<TGrpcClient>(options => options.Address = new Uri(baseAddress))
                         .ConfigureChannel(options =>
                         {
                             options.Credentials = ChannelCredentials.Insecure;
                             options.ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } };
                             options.HttpHandler = new SocketsHttpHandler
                             {
                                 PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                                 KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                                 KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                                 EnableMultipleHttp2Connections = true
                             };
                         }).AddHttpMessageHandler<TestDelegatingHandler>();

            return services;
        }
    }
}
