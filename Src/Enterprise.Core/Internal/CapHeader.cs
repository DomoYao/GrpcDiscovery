using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Core.Internal
{
    public class CapHeader : ReadOnlyDictionary<string, string?>
    {
        public CapHeader(IDictionary<string, string?> dictionary) : base(dictionary)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromCapAttribute : Attribute
    {

    }
}
