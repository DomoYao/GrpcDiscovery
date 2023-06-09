﻿
namespace Enterprise.Core.Internal.Filter
{
    /// <summary>
    /// A filter that surrounds execution of the subscriber.
    /// </summary>
    public interface ISubscribeFilter
    {
        /// <summary>
        /// Called before the subscriber executes.
        /// </summary>
        /// <param name="context">The <see cref="ExecutingContext"/>.</param>
        void OnSubscribeExecuting(ExecutingContext context);

        /// <summary>
        /// Called after the subscriber executes.
        /// </summary>
        /// <param name="context">The <see cref="ExecutedContext"/>.</param>
        void OnSubscribeExecuted(ExecutedContext context);

        /// <summary>
        /// Called after the subscriber has thrown an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="context">The <see cref="ExceptionContext"/>.</param>
        void OnSubscribeException(ExceptionContext context);
    }
}
