using Enterprise.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Consul.Provider.Grpc.Registrer
{
    public static class ApplicationBuilderConsulExtension
    {
        public static IHost RegisterToConsul(this IHost host, string serviceId, int port, string scheme = "http")
        {
            var registration = ActivatorUtilities.CreateInstance<RegistrationProvider>(host.Services);
            var ipAddresses = registration.GetLocalIpAddress("InterNetwork");
            if (ipAddresses.IsNullOrEmpty())
                throw new NotImplementedException("ipAddresses");

            var serviceAddress = new Uri($"{scheme}://{ipAddresses.FirstOrDefault()}:{port}");
            registration.Register(serviceAddress, serviceId);
            return host;
        }

        public static IHost RegisterGrpcToConsul(this IHost host, int port, string serviceId="",  string scheme = "http")
        {
            var registration = ActivatorUtilities.CreateInstance<RegistrationProvider>(host.Services);
            var ipAddresses = registration.GetLocalIpAddress("InterNetwork");
            if (ipAddresses.IsNullOrEmpty())
                throw new NotImplementedException("ipAddresses");

            var serviceAddress = new Uri($"{scheme}://{ipAddresses.FirstOrDefault()}:{port}");
            registration.RegisterGrpc(serviceAddress, serviceId);
            return host;
        }

        public static IHost RegisterToConsul(this IHost host, Uri serviceAddress, string? serviceId = null)
        {
            if (serviceAddress is null)
                throw new ArgumentNullException(nameof(serviceAddress));

            var registration = ActivatorUtilities.CreateInstance<RegistrationProvider>(host.Services);
            registration.Register(serviceAddress, serviceId);
            return host;
        }

        public static IHost RegisterToConsul(this IHost host, AgentServiceRegistration instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            var registration = ActivatorUtilities.CreateInstance<RegistrationProvider>(host.Services);
            registration.Register(instance);
            return host;
        }

        /// <summary>
        /// 注册AddConsul
        /// </summary>
        /// <param name="services"></param>
        /// <param name="consulSection"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfigurationSection consulSection)
        {
            if (services.HasRegistered(nameof(AddConsul)))
                return services;

            return services
                .Configure<ConsulOptions>(consulSection)
                .AddSingleton(provider =>
                {
                    var configOptions = provider.GetService<IOptions<ConsulOptions>>();
                    if (configOptions is null)
                        throw new NullReferenceException(nameof(configOptions));
                    return new ConsulClient(x => x.Address = new Uri(configOptions.Value.ConsulUrl));
                })
                ;
        }
    }
}