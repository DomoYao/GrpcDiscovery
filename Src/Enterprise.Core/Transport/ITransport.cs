using System.Threading.Tasks;
using Enterprise.Core.Messages;

namespace Enterprise.Core.Transport
{
    public interface ITransport
    {
        BrokerAddress BrokerAddress { get; }

        Task<OperateResult> SendAsync(TransportMessage message);
    }
}
