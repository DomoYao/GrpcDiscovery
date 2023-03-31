// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Enterprise.Core.Messages
{
    public static class Headers
    {
        /// <summary>
        /// Id of the message. Either set the ID explicitly when sending a message, or assign one to the message.
        /// </summary>
        public const string MessageId = "msg-id";

        public const string MessageName = "msg-name";

        public const string Group = "msg-group";

        /// <summary>
        /// Message value .NET type
        /// </summary>
        public const string Type = "msg-type";

        public const string CorrelationId = "corr-id";

        public const string CorrelationSequence = "corr-seq";

        public const string CallbackName = "callback-name";

        public const string SentTime = "senttime";

        public const string Exception = "exception";
    }
}
