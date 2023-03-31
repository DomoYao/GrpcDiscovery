using System;
using System.Reflection;
using Enterprise.Core.Messages;
using Enterprise.Core.Transport;

namespace Enterprise.Core.Diagnostics
{
    public class CapEventDataSubStore
    {
        public long? OperationTimestamp { get; set; }

        public string Operation { get; set; } = default!;

        public TransportMessage TransportMessage { get; set; } = default!;

        public BrokerAddress BrokerAddress { get; set; }

        public long? ElapsedTimeMs { get; set; }

        public Exception? Exception { get; set; }
    }

    public class CapEventDataSubExecute
    {
        public long? OperationTimestamp { get; set; }

        public string Operation { get; set; } = default!;

        public Message Message { get; set; } = default!;

        public MethodInfo? MethodInfo { get; set; }

        public long? ElapsedTimeMs { get; set; }

        public Exception? Exception { get; set; }
    }
}
