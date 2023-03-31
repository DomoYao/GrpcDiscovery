using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Core.Exceptions
{
    public class PublisherSentFailedException : Exception
    {
        public PublisherSentFailedException(string message) : base(message)
        {
        }

        public PublisherSentFailedException(string message, Exception? ex) : base(message, ex)
        {
        }
    }
}
