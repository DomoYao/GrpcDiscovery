using Enterprise.Core;
using Enterprise.Core.Messages;

namespace Enterprise.Core.Internal
{
    public interface IMessageSender
    {
        Task<OperateResult> SendAsync(MediumMessage message);
    }
}