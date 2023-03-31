using Consul.Provider.Grpc;
using Consul.Provider.Grpc.Registrer;
using Etcd.Provider;
using GrpcClientTest.Base;
using static GrpcServiceTest.Greeter;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddScoped<TestDelegatingHandler>();
// Add services to the container.
var serviceRegistryType = builder.Configuration.GetValue<string>("ServiceRegistryType");
if (serviceRegistryType == "Consul")
{

    // Consul ����DIע��
    builder.Services.AddConsul(builder.Configuration.GetSection("Consul"));
    // ע�������
    builder.Services.AddConsulGrpcClient<GreeterClient>("http://grpc-Test", "grpc-Test");
}
else if (serviceRegistryType == "Etcd")
{
    // Etcd ����DIע��
    builder.Services.AddEtcd(builder.Configuration.GetSection("Etcd"));
    // ע�������
    //builder.Services.AddEtcdGrpcClient<GreeterClient>("http://GrpcTest");
    builder.Services.AddEtcdGrpcClientAndAddMessageHandler<GreeterClient>("http://GrpcTest");
}

var app = builder.Build();
app.MapControllers();

app.Run();

