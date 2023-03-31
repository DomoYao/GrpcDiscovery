using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enterprise.Core.Messages;

namespace Enterprise.Core.Persistence
{
    public interface IDataStorage
    {
        Task ChangePublishStateAsync(MediumMessage message, StatusName state);

        Task ChangeReceiveStateAsync(MediumMessage message, StatusName state);

        MediumMessage StoreMessage(string name, Message content, object? dbTransaction = null);

        void StoreReceivedExceptionMessage(string name, string group, string content);

        MediumMessage StoreReceivedMessage(string name, string group, Message content);

        Task<int> DeleteExpiresAsync(string table, DateTime timeout, int batchCount = 1000,
            CancellationToken token = default);

        Task<IEnumerable<MediumMessage>> GetPublishedMessagesOfNeedRetry();

        Task<IEnumerable<MediumMessage>> GetReceivedMessagesOfNeedRetry();

        //dashboard api
        //IMonitoringApi GetMonitoringApi();
    }

    public enum StatusName
    {
        Failed = -1,
        Scheduled,
        Succeeded
    }
}
