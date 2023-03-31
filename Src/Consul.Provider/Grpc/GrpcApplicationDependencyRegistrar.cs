using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Consul.Provider.Grpc
{
    /// <summary>
    /// Gprc DI 注册
    /// </summary>
    public static class GrpcApplicationDependencyRegistrar
    {
        /// <summary>
        /// 注册Grpc服务(跨微服务之间的同步通讯)
        /// </summary>
        /// <typeparam name="consulAddress">Consul服务地址</typeparam>
        /// <param name="serviceName">在注册中心注册的服务名称，或者服务的Url</param>
        public static IServiceCollection AddConsulGrpcClient<TGrpcClient>(this IServiceCollection services,string consulAddress, string serviceName)
         where TGrpcClient : class
        {
            var consulClient = services.BuildServiceProvider().GetRequiredService<ConsulClient>();
            var baseAddress = consulAddress.Replace("http://", "consul://").Replace("https://", "consul://");
            services.TryAddSingleton<ResolverFactory>(_ => new ConsulGrpcResolverFactory(consulClient, TimeSpan.FromSeconds(30)));
            services.AddGrpcClient<TGrpcClient>(options => options.Address = new Uri(baseAddress))
                         .ConfigureChannel(options =>
                         {
                             options.Credentials = ChannelCredentials.Insecure;
                             options.ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } };
                             //options.HttpHandler = new SocketsHttpHandler
                             //{
                             //    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                             //    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                             //    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                             //    EnableMultipleHttp2Connections = true
                             //};
                         });

            return services;
        }
    }
}
