namespace Consul.Provider.Grpc.Balancers;

public interface ILoadBalancer
{
    string Resolve(IList<string> services);
}
