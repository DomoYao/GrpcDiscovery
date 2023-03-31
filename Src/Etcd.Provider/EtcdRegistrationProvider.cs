using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Etcd.Provider
{
    /// <summary>
    /// Etcd 服务注册.
    /// 根据etcd的v3 API，当启动一个服务时候，我们把服务的地址写进etcd，注册服务。同时绑定租约（lease），并以续租约（keep leases alive）的方式检测服务是否正常运行，从而实现健康检查
    /// </summary>
    public class EtcdRegistrationProvider
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<EtcdRegistrationProvider> _logger;
        private readonly EtcdClient _etcdClient;
        private Timer _timer;
        private long? leaseId = null; // 租约ID
        private string serviceKeyId = string.Empty;  // 服务注册Key名
        private string serviceEndpointUrl = string.Empty;// 服务注册地址

        public EtcdRegistrationProvider(EtcdClient etcdClient, IHostApplicationLifetime hostApplicationLifetime, ILogger<EtcdRegistrationProvider> logger)
        {
            _etcdClient = etcdClient;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        public void Register( Uri serviceAddress, string serviceName, string? serviceId = null)
        {
            if (serviceAddress is null)
            {
                throw new ArgumentNullException(nameof(serviceAddress));
            }

            if (string.IsNullOrEmpty(serviceId))
            {
                serviceKeyId = $"/{serviceName}/{serviceAddress.Host}:{serviceAddress.Port}";
            }
            else
            {
                serviceKeyId = $"/{serviceName}/{serviceId}";
            }

            serviceEndpointUrl = $"{serviceAddress.Scheme}://{serviceAddress.Host}:{serviceAddress.Port}";
            _logger.LogInformation(@$"register {serviceId} to Etcd ({serviceEndpointUrl})");
            _hostApplicationLifetime.ApplicationStarted.Register(async () =>
            {
                leaseId = await LeaseGrant(); 

                // await _etcdClient.LeaseKeepAlive(grantRes.ID, default(CancellationToken)); // 直接采用这个发现服务运行时间久后，存在续约失败，原因还未知（猜想可能线程被释放）. 
                _timer = new Timer(LeaseKeepAlive, leaseId, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)); // 采用定时器进行保活续约.
            });

            _hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                try
                {
                    if (leaseId != null)
                    {
                        _etcdClient.LeaseRevoke(new Etcdserverpb.LeaseRevokeRequest { ID = leaseId.Value });
                    }

                    _etcdClient.Delete(serviceId);
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }, true);
        }

        /// <summary>
        /// 注册租约.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="url"></param>
        /// <param name="id">租约ID 即leaseId </param>
        /// <returns></returns>
        private async Task<long> LeaseGrant(long id = 0)
        {
            await _etcdClient.DeleteAsync(serviceKeyId);
            var grantRes = await _etcdClient.LeaseGrantAsync(new Etcdserverpb.LeaseGrantRequest { ID = id, TTL = 20 });
            await _etcdClient.PutAsync(new Etcdserverpb.PutRequest { Lease = grantRes.ID, Key = ByteString.CopyFromUtf8(serviceKeyId), Value = ByteString.CopyFromUtf8(serviceEndpointUrl), IgnoreLease = false });
            return grantRes.ID;
        }

        private void LeaseKeepAlive(object obj)
        {
            var leaseId = (long)obj;
            var keepAliveReq = new LeaseKeepAliveRequest { ID = leaseId };
            var task = _etcdClient.LeaseKeepAlive(keepAliveReq, (res) =>
            {
                if (res.ID != leaseId || res.TTL == 0L)
                {
                    Console.WriteLine($"{DateTime.Now}  LeaseKeepAlive Error {res.ID}!={leaseId},{res.TTL},register again.");
                    LeaseGrant(leaseId).ConfigureAwait(false).GetAwaiter().GetResult();
                }

            }, default(CancellationToken)).ConfigureAwait(false).GetAwaiter();
        }

        /// <summary>
        /// get all ip address
        /// </summary>
        /// <param name="netType">"InterNetwork":ipv4，"InterNetworkV6":ipv6</param>
        public string GetLocalIpAddress(string netType)
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            var IPList = new List<string>();
            if (netType == string.Empty)
            {
                for (int i = 0; i < addresses.Length; i++)
                {
                    IPList.Add(addresses[i].ToString());
                }
            }
            else
            {
                //AddressFamily.InterNetwork = IPv4,
                //AddressFamily.InterNetworkV6= IPv6
                for (int i = 0; i < addresses.Length; i++)
                {
                    if (addresses[i].AddressFamily.ToString() == netType)
                    {
                        IPList.Add(addresses[i].ToString());
                    }
                }
            }

            if (IPList.Count == 1)
            {
                return IPList[0].ToString();
            }
            else
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    var endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
        }
    }
}
