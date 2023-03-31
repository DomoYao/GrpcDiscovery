using Consul.Provider.Grpc.Registrer;
using Enterprise.Core.Exceptions;
using Etcd.Provider;
using GrpcServiceTest.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddHealthChecks();

// 服务注册中心DI
var serviceRegistryType = builder.Configuration.GetValue<string>("ServiceRegistryType");
if (serviceRegistryType == "Consul")
{
    // Consul 服务DI注册
    builder.Services.AddConsul(builder.Configuration.GetSection("Consul"));
}
else if (serviceRegistryType == "Etcd")
{
    // Etcd 服务DI注册
    builder.Services.AddEtcd(builder.Configuration.GetSection("Etcd"));
}

builder.Services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(30));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, o => o.Protocols = HttpProtocols.Http2);
    options.Limits.Http2.MaxStreamsPerConnection = 100 * 10;
    options.Limits.Http2.InitialConnectionWindowSize = 131072 * 10;
});



var app = builder.Build();
app.MapGrpcService<HealthCheckService>();
app.MapGrpcService<GreeterService>();

if (serviceRegistryType == "Consul")
{
    // Consul 服务注册
    app.RegisterGrpcToConsul(5000, $"grpc-1");
}
else if (serviceRegistryType == "Etcd")
{
    // Etcd 服务注册
    app.RegisterToEtcd("GrpcTest", 5000);
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
