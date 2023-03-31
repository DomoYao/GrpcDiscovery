using System;

namespace Enterprise.Core.Transport
{
    public enum MqLogType
    {
        //RabbitMQ
        ConsumerCancelled,
        ConsumerRegistered,
        ConsumerUnregistered,
        ConsumerShutdown,

        //Kafka
        ConsumeError,
        ConsumeRetries,
        ServerConnError,

        //AzureServiceBus
        ExceptionReceived,

        //NATS
        AsyncErrorEvent,
        ConnectError,

        //Amazon SQS
        InvalidIdFormat,
        MessageNotInflight
    }

    public class LogMessageEventArgs : EventArgs
    {
        public string? Reason { get; set; }

        public MqLogType LogType { get; set; }
    }
}