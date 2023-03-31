using dotnet_etcd;
using Enterprise.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Etcd.Provider
{
    public static class EtcdExtension
    {
        /// <summary>
        /// 注册AddEtcd
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEtcd(this IServiceCollection services, IConfigurationSection consulSection)
        {
            services.AddSingleton<EtcdRegistrationProvider>();
            if (services.HasRegistered(nameof(AddEtcd)))
                return services;

            return services
                .Configure<EtcdOptions>(consulSection)
                .AddSingleton(provider =>
                {
                    var configOptions = provider.GetService<IOptions<EtcdOptions>>();
                    if (configOptions is null)
                        throw new NullReferenceException(nameof(configOptions));
                    return new EtcdClient(configOptions.Value.Url);
                })
                ;
        }

        /// <summary>
        /// 注册服务.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="serviceName"></param>
        /// <param name="port"></param>
        /// <param name="serviceId"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IHost RegisterToEtcd(this IHost host, string serviceName, int port, string serviceId = "", string scheme = "http")
        {
            //var registration = ActivatorUtilities.CreateInstance<EtcdRegistrationProvider>(host.Services); 如果没有注册，可以直接用该语句辅助实例化.
            var registration = host.Services.GetRequiredService<EtcdRegistrationProvider>();
            var ipAddresses = registration.GetLocalIpAddress("InterNetwork");
            if (ipAddresses.IsNullOrEmpty())
                throw new NotImplementedException("ipAddresses");

            var serviceAddress = new Uri($"{scheme}://{ipAddresses}:{port}");
            registration.Register(serviceAddress, serviceName, serviceId);
            return host;
        }
    }
}
