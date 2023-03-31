using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etcd.Provider
{
    /// <summary>
    /// Etcd grpc 客户端注册.
    /// </summary>
    public static class EtcdGrpcClientRegistrar
    {
        /// <summary>
        /// 注册Grpc服务(跨微服务之间的同步通讯)
        /// </summary>
        /// <typeparam name="etcdKeyPrefix">Etcd前缀，http://{前缀}</typeparam>
        public static IServiceCollection AddEtcdGrpcClient<TGrpcClient>(this IServiceCollection services, string etcdKeyPrefix)
         where TGrpcClient : class
        {
            var baseAddress = etcdKeyPrefix.Replace("http://", "etcd://").Replace("https://", "etcd://");
            services.TryAddSingleton<ResolverFactory, EtcdGrpcResolverFactory>();
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
