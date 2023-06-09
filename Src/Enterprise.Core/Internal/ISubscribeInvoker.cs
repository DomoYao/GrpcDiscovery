﻿using System.Threading;
using System.Threading.Tasks;

namespace Enterprise.Core.Internal
{
    /// <summary>
    /// Perform user definition method of consumers.
    /// </summary>
    public interface ISubscribeInvoker
    {
        /// <summary>
        /// Invoke subscribe method with the consumer context.
        /// </summary>
        /// <param name="context">consumer execute context</param>
        /// <param name="cancellationToken">The object of <see cref="CancellationToken"/>.</param>
        Task<ConsumerExecutedResult> InvokeAsync(ConsumerContext context, CancellationToken cancellationToken = default);
    }
}