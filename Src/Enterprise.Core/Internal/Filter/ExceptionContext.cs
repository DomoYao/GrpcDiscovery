
namespace Enterprise.Core.Internal.Filter
{
    public class ExceptionContext : FilterContext
    {
        public ExceptionContext(ConsumerContext context, Exception e)
            : base(context)
        {
            Exception = e;
        }

        public Exception Exception { get; set; }

        public bool ExceptionHandled { get; set; }

        public object? Result { get; set; }
    }
}