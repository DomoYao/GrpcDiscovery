using Microsoft.Extensions.Logging;

namespace Consul.Provider.Grpc.Registrer
{
    public class ConsulBuilder
    {
        private readonly ConsulClient _client;
        private readonly List<AgentServiceCheck> _checks = new List<AgentServiceCheck>();
        private readonly ILogger<ConsulBuilder> _logger;

        public ConsulBuilder(ConsulClient client, ILogger<ConsulBuilder> logger)
        {
            _client = client;
            _logger = logger;
        }

        public ConsulBuilder AddHealthCheck(AgentServiceCheck check)
        {
            _checks.Add(check);
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout">unit: second</param>
        /// <param name="interval">check interval. unit: second</param>
        /// <returns></returns>
        public ConsulBuilder AddHttpHealthCheck(string url, int timeout = 10, int interval = 10)
        {
            _checks.Add(new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(timeout * 3),
                Interval = TimeSpan.FromSeconds(interval),
                HTTP = url,
                Timeout = TimeSpan.FromSeconds(timeout)
            });

            _logger.LogInformation($"[Consul]Add Http Healthcheck Success! CheckUrl:{url}");

            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="endpoint">GPRC service address.</param>
        /// <param name="grpcUseTls"></param>
        /// <param name="timeout">unit: second</param>
        /// <param name="interval">check interval. unit: second</param>
        /// <returns></returns>
        public ConsulBuilder AddGRPCHealthCheck(string endpoint, bool grpcUseTls = false, int timeout = 10, int interval = 10)
        {
            _checks.Add(new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20),
                Interval = TimeSpan.FromSeconds(interval),
                GRPC = endpoint,
                GRPCUseTLS = grpcUseTls,
                Timeout = TimeSpan.FromSeconds(timeout)
            });

            _logger.LogInformation($"[Consul]Add GRPC HealthCheck Success! Endpoint:{endpoint}");

            return this;
        }

        public async Task RegisterService(string name, string host, int port, string[] tags)
        {
            var registration = new AgentServiceRegistration()
            {
                Checks = _checks.ToArray(),
                ID = $"{name}_{host}:{port}",
                Name = name,
                Address = host,
                Port = port,
                Tags = tags
            };

            await _client.Agent.ServiceRegister(registration);
            _logger.LogInformation($"[Consul]Register Service Success! Name:{name} ID:{registration.ID}");

            AppDomain.CurrentDomain.ProcessExit += async (sender, e) =>
            {
                _logger.LogInformation($"[Consul] Service Deregisting ....  ID:{registration.ID}");

                await _client.Agent.ServiceDeregister(registration.ID);
            };
        }

        /// <summary>
        /// 移除服务
        /// </summary>
        /// <param name="serviceId"></param>
        public async Task Deregister(string serviceId)
        {
            await _client?.Agent?.ServiceDeregister(serviceId);
        }
    }
}
