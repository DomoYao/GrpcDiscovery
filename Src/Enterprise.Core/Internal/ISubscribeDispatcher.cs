using System.Threading;
using System.Threading.Tasks;
using Enterprise.Core.Messages;

namespace Enterprise.Core.Internal
{
    /// <summary>
    /// Consumer executor
    /// </summary>
    public interface ISubscribeDispatcher
    {
        Task<OperateResult> DispatchAsync(MediumMessage message, CancellationToken cancellationToken = default);

        Task<OperateResult> DispatchAsync(MediumMessage message, ConsumerExecutorDescriptor descriptor, CancellationToken cancellationToken = default);
    }
}