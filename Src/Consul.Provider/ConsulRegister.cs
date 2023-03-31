using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Consul.Provider
{
    /// <summary>
    /// Consul注册
    /// </summary>
    public static class ConsulRegister
    {
        //服务注册
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            // 获取主机生命周期管理接口
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri(configuration["Consul:consulAddress"]);
                c.Datacenter = "dc1";
            });
            string ip = configuration["ip"]; //优先接收变量的值
            string port = configuration["port"]; //优先接收变量的值
            string currentIp = configuration["Consul:currentIP"];
            string currentPort = configuration["Consul:currentPort"];

            ip = string.IsNullOrEmpty(ip) ? currentIp : ip; //当前程序的IP
            port = string.IsNullOrEmpty(port) ? currentPort : port; //当前程序的端口
            string serviceId = $"service:{ip}:{port}";//服务ID，一个服务是唯一的
            //服务注册
            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = serviceId, //唯一的
                Name = configuration["Consul:serviceName"], //组名称-Group
                //Address = ip, //ip地址
                Port = int.Parse(port), //端口
                Tags = new string[] { "api站点" },
                //Checks = new AgentServiceCheck[] { // 注册多个check
                //    new AgentServiceCheck  // http Check
                //    {
                //        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5), // 服务启动多久后注册
                //        Interval = TimeSpan.FromSeconds(10), // 健康检查时间间隔
                //        HTTP = $"http://{ip}:9081/api/Check", // 健康检查地址
                //        Timeout = TimeSpan.FromSeconds(5) // 超时时间
                //    },
                //    new AgentServiceCheck() // grpc check
                //    {
                //        Interval = TimeSpan.FromSeconds(10),//多久检查一次心跳
                //        GRPC = $"{ip}:{port}", //gRPC注册特有
                //        GRPCUseTLS = false,//支持http
                //        Timeout = TimeSpan.FromSeconds(5),//超时时间
                //        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20) //服务停止多久后注销服务
                //    }
                //},
                Check = new AgentServiceCheck  // http Check
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5), // 服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10), // 健康检查时间间隔
                    HTTP = $"http://{ip}:{port}/api/Check", // 健康检查地址
                    Timeout = TimeSpan.FromSeconds(5) // 超时时间
                },
                //Check = new AgentServiceCheck() // grpc check
                //{
                //    Interval = TimeSpan.FromSeconds(10),//多久检查一次心跳
                //    GRPC = $"{ip}:{port}", //gRPC注册特有
                //    GRPCUseTLS = false,//支持http
                //    Timeout = TimeSpan.FromSeconds(5),//超时时间
                //    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20) //服务停止多久后注销服务
                //}

            }).Wait();

            //应用程序终止时,注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                client.Agent.ServiceDeregister(serviceId).Wait();
            });
            return app;
        }

        /// <summary>
        /// 注册redis
        /// </summary>
        public static void RegisterRedis()
        {
            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri("http://127.0.0.1:8500");
                c.Datacenter = "dc1";
            });

            var agent = new AgentServiceRegistration()
            {
                ID = "127.0.0.1:6379", //唯一的
                Name = "Redis-Dev", //组名称-Group
                Address = "127.0.0.1", //ip地址
                Port = 6379, //端口
                Tags = new string[] { "master-test-6379" },
                Check = new AgentServiceCheck  // redis Check
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5), // 服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10), // 健康检查时间间隔
                },
            };

            client.Agent.ServiceRegister(agent).Wait();
        }
    }
}