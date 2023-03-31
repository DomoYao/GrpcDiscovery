﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enterprise.Core.Messages;

namespace Enterprise.Core.Internal
{
    /// <summary>
     /// A context for consumers, it used to be provider wrapper of method description and received message.
     /// </summary>
    public class ConsumerContext
    {
        public ConsumerContext(ConsumerContext context)
        : this(context.ConsumerDescriptor, context.DeliverMessage)
        {

        }

        /// <summary>
        /// create a new instance of  <see cref="ConsumerContext" /> .
        /// </summary>
        /// <param name="descriptor">consumer method descriptor. </param>
        /// <param name="message"> received message.</param>
        public ConsumerContext(ConsumerExecutorDescriptor descriptor, Message message)
        {
            ConsumerDescriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            DeliverMessage = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// a descriptor of consumer information need to be performed.
        /// </summary>
        public ConsumerExecutorDescriptor ConsumerDescriptor { get; }

        /// <summary>
        /// consumer received message.
        /// </summary>
        public Message DeliverMessage { get; }
    }
}
