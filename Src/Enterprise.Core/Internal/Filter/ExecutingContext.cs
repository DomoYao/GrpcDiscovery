
namespace Enterprise.Core.Internal.Filter
{
    public class ExecutingContext : FilterContext
    {
        public ExecutingContext(ConsumerContext context, object?[] arguments) : base(context)
        {
            Arguments = arguments;
        }

        public object?[] Arguments { get; set; }
    }
}