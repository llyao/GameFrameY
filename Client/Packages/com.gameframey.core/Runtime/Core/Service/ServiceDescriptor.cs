using System;

namespace GameFrameY.Core.Service
{
    public enum ServiceChangeKind
    {
        Added = 0,
        Removed = 1,
    }

    public sealed class ServiceChangeEventArgs : EventArgs
    {
        public ServiceChangeEventArgs(ServiceChangeKind kind, ServiceDescriptor descriptor)
        {
            Kind = kind;
            Descriptor = descriptor;
        }

        public ServiceChangeKind Kind { get; }
        public ServiceDescriptor Descriptor { get; }
    }

    /// <summary>一条登记在中央注册表里的服务记录（动态登记的实例；容器类型注册不经过此对象）。</summary>
    public sealed class ServiceDescriptor
    {
        public ServiceDescriptor(Type serviceType, object instance, string ownerModuleId)
        {
            ServiceType = serviceType;
            Instance = instance;
            OwnerModuleId = ownerModuleId;
        }

        public Type ServiceType { get; }
        public object Instance { get; }
        public string OwnerModuleId { get; }
    }
}
