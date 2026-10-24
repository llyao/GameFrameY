using System;
using System.Collections.Generic;

namespace GameFrameY.Core.Module
{
    /// <summary>模块描述符：由特性声明生成，装配过程由管理器回写状态。</summary>
    public sealed class GameModuleDescriptor
    {
        public GameModuleDescriptor(Type moduleType, GameModuleAttribute attribute)
        {
            ModuleType = moduleType;
            Name = ResolveName(moduleType, attribute);
            Priority = attribute.Priority;
            Dependencies = attribute.Dependencies;
            InstallerType = attribute.InstallerType;
            Startup = attribute.Startup;
        }

        public Type ModuleType { get; }
        public string Name { get; }
        public int Priority { get; }
        public IReadOnlyList<string> Dependencies { get; }
        public Type InstallerType { get; }
        public ModuleStartup Startup { get; }

        /// <summary>解析后的装配顺序（依赖优先，同级按 Priority、名字稳定排序）。</summary>
        public int InstallOrder { get; internal set; }

        public GameModuleState State { get; internal set; } = GameModuleState.Discovered;

        /// <summary>模块所在的 VContainer 子作用域（模块实例即注册于此）。</summary>
        public VContainer.Unity.LifetimeScope Scope { get; internal set; }

        /// <summary>模块实例（装配后可用）。</summary>
        public IGameModule ModuleInstance { get; internal set; }

        private static string ResolveName(Type moduleType, GameModuleAttribute attribute)
        {
            var name = attribute?.Name;
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            var typeName = moduleType.Name;
            return typeName.EndsWith("Module", StringComparison.Ordinal)
                ? typeName.Substring(0, typeName.Length - "Module".Length)
                : typeName;
        }
    }
}
