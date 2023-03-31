using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Core.Diagnostics
{
    /// <summary>
    /// 诊断名称.
    /// </summary>
    public static class DiagnosticListenerNames
    {
        private const string Prefix = "Core.Diagnostics.";

        public const string DiagnosticListenerName = "DiagnosticListener";

        public const string BeforePublishMessageStore = Prefix + "WritePublishMessageStoreBefore";
        public const string AfterPublishMessageStore = Prefix + "WritePublishMessageStoreAfter";
        public const string ErrorPublishMessageStore = Prefix + "WritePublishMessageStoreError";

        public const string BeforePublish = Prefix + "WritePublishBefore";
        public const string AfterPublish = Prefix + "WritePublishAfter";
        public const string ErrorPublish = Prefix + "WritePublishError";

        public const string BeforeConsume = Prefix + "WriteConsumeBefore";
        public const string AfterConsume = Prefix + "WriteConsumeAfter";
        public const string ErrorConsume = Prefix + "WriteConsumeError";

        public const string BeforeSubscriberInvoke = Prefix + "WriteSubscriberInvokeBefore";
        public const string AfterSubscriberInvoke = Prefix + "WriteSubscriberInvokeAfter";
        public const string ErrorSubscriberInvoke = Prefix + "WriteSubscriberInvokeError";
    }
}
