using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Core.Exceptions
{
    public class BrokerConnectionException : Exception
    {
        public BrokerConnectionException(Exception innerException)
            : base("Broker Unreachable", innerException)
        {

        }
    }
}
