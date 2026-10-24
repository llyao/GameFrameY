using System;

namespace GameFrameY.Core.Service
{
    /// <summary>服务发现（注册/解析/等待）相关异常。</summary>
    public class ServiceRegistryException : Exception
    {
        public ServiceRegistryException(string message) : base(message)
        {
        }
    }
}
