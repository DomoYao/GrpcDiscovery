using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Core.Exceptions
{
    public class SubscriberNotFoundException : Exception
    {
        public SubscriberNotFoundException(string message) : base(message)
        {
        }
    }
}
