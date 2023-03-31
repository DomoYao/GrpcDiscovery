
using Enterprise.Core.Internal;
using Enterprise.Core.Messages;

namespace Enterprise.Core.Transport
{
    /// <summary>
    /// 处理服务调度员.
    /// </summary>
    public interface IDispatcher : IProcessingServer
    {
        void EnqueueToPublish(MediumMessage message);

        void EnqueueToExecute(MediumMessage message, ConsumerExecutorDescriptor descriptor);
    }
}