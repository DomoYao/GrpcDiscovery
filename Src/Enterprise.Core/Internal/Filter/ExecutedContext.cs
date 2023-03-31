
namespace Enterprise.Core.Internal.Filter
{
    public class ExecutedContext : FilterContext
    {
        public ExecutedContext(ConsumerContext context, object? result) : base(context)
        {
            Result = result;
        }

        public object? Result { get; set; }
    }
}