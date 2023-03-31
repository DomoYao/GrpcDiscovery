namespace Consul.Provider.Grpc
{
    public interface IDiscoverProvider
    {
        Task<IList<string>> GetAllHealthServicesAsync();
        Task<string> GetSingleHealthServiceAsync();
    }
}
