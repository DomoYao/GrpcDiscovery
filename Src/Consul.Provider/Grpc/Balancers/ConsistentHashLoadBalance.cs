using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consul.Provider.Grpc.Balancers
{
    /// <summary>
    /// 一致Hash，负载均衡.
    /// </summary>
    public class ConsistentHashLoadBalance : SubchannelsLoadBalancer
    {
        private readonly ILogger _logger;

        public ConsistentHashLoadBalance(IChannelControlHelper controller, ILoggerFactory loggerFactory)
         : base(controller, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConsistentHashLoadBalance>();
        }

        protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
        {
            return new ConsistentHashLoadPicker(readySubchannels.OrderBy(p => p.CurrentAddress).ToList());
        }

        //private Dictionary<ServiceNode, int> GenerateNodeToWeight(IReadOnlyList<Subchannel> subchannels)
        //{
        //    Dictionary<ServiceNode, int> dic = new Dictionary<ServiceNode, int>();
        //    for (var i = 0; i < 10; i++)
        //    {
        //        var weight = 1;
        //        foreach (var it in subchannels)
        //        {
        //            dic.Add(it.GetAddresses);
        //        }
        //    }
        //}

        private class ConsistentHashLoadPicker : SubchannelPicker
        {
            internal readonly List<Subchannel> _subchannels;
            private static readonly BalancerAttributesKey<ServiceNode> serviceNodeName = new BalancerAttributesKey<ServiceNode>("ServiceNodeName");

            public ConsistentHashLoadPicker(IReadOnlyList<Subchannel> subchannels)
            {
                // 每个服务增加服务名称特性.
                int nodeIndex = 1;
                foreach (var subchannel in subchannels)
                {
                    var serviceNode = new ServiceNode($"Node_{subchannel.CurrentAddress?.EndPoint.Host.GetHashCode()}");
                    if (!subchannel.Attributes.TryGetValue(serviceNodeName, out _))
                    {
                        subchannel.Attributes.Set(serviceNodeName, serviceNode);
                    }

                    //Enumerable.Range(0, 10).Select(index => (name: new ServiceNode("Node" + index.ToString()), weight: index * 100))

                    nodeIndex++;
                }

                _subchannels = subchannels.ToList();
            }

            public override PickResult Pick(PickContext context)
            {
                // Pick a random subchannel.
                return PickResult.ForSubchannel(_subchannels[Random.Shared.Next(0, _subchannels.Count)]);
            }
        }

        /// <summary>
        /// 服务节点
        /// </summary>
        private class ServiceNode : IEquatable<ServiceNode>, IComparable<ServiceNode>
        {
            private string fName;

            public ServiceNode(string name)
            {
                fName = name;
            }
            public int CompareTo(ServiceNode other)
            {
                return StringComparer.Ordinal.Compare(fName, other?.fName);
            }

            public bool Equals(ServiceNode other)
            {
                return fName.Equals(other?.fName, StringComparison.Ordinal);
            }

            public override bool Equals(object obj)
            {
                if (obj is ServiceNode other)
                {
                    return Equals(other);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return fName.GetHashCode();
            }

            public override string ToString()
            {
                return fName;
            }
        }
    }

    public class ConsistentHashLoadBalanceFactory : LoadBalancerFactory
    {
        // Create a RandomBalancer when the name is 'ConsistentHash'.
        public override string Name => "consistenthash";

        public override LoadBalancer Create(LoadBalancerOptions option)
        {
            return new ConsistentHashLoadBalance(option.Controller, option.LoggerFactory);
        }
    }
}
