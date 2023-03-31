using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Core.Exceptions
{
    internal class SubscriberExecutionFailedException : Exception
    {
        private readonly Exception _originException;

        public SubscriberExecutionFailedException(string message, Exception ex) : base(message, ex)
        {
            _originException = ex;
        }

        public override string StackTrace => _originException.StackTrace;

        public override string Source => _originException.Source;
    }
}
