
namespace Etcd.Provider
{
    /// <summary>
    /// Etcd 配置
    /// </summary>
    public class EtcdOptions
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 刷新间隔秒
        /// </summary>
        public int SecondsToReload { get; set; }

        /// <summary>
        /// 将监视前缀列表
        /// </summary>
        public List<string> PrefixListUsedToWatch { get; set; }
    }
}