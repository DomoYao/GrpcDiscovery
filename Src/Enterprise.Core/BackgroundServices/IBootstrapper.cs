using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprise.Core.BackgroundServices
{
    /// <summary>
    /// 引导程序入口,例如，将初始状态添加到存储或查询某些实体。.
    /// </summary>
    public interface IBootstrapper
    {
        Task BootstrapAsync();
    }
}
