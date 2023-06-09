
namespace Enterprise.Core.Internal.Filter
{
    /// <summary>
    /// Abstract base class for ISubscribeFilter for use when implementing a subset of the interface methods.
    /// </summary>
    public abstract class SubscribeFilter : ISubscribeFilter
    {
        /// <summary>
        /// Called before the subscriber executes.
        /// </summary>
        /// <param name="context">The <see cref="ExecutingContext"/>.</param>
        public virtual void OnSubscribeExecuting(ExecutingContext context)
        {
        }

        /// <summary>
        /// Called after the subscriber executes.
        /// </summary>
        /// <param name="context">The <see cref="ExecutedContext"/>.</param>
        public virtual void OnSubscribeExecuted(ExecutedContext context)
        {
        }

        /// <summary>
        /// Called after the subscriber has thrown an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="context">The <see cref="ExceptionContext"/>.</param>
        public virtual void OnSubscribeException(ExceptionContext context)
        {
        }
    }
}