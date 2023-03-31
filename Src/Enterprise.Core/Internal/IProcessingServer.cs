using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Core.Internal
{
    /// <inheritdoc />
    /// <summary>
    /// A process thread abstract of message process.
    /// </summary>
    public interface IProcessingServer : IDisposable
    {
        void Pulse() { }

        void Start(CancellationToken stoppingToken);
    }
}
