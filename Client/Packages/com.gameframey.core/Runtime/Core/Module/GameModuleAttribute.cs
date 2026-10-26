using System;

namespace GameFrameY.Core.Module
{
    /// <summary>模块启动阶段：Bootstrap 在框架自举时装配；Deferred 由业务流程（如 ProcedureEnterGame）显式装配。</summary>
    public enum ModuleStartup
    {
        Bootstrap = 0,
        Deferred = 1,
    }

    /// <summary>
    /// 标记一个类型为 GameFrameY 模块。模块由 <see cref="ModuleScanner"/> 自动发现，
    /// 由 <see cref="GameModuleManager"/> 按依赖拓扑序装配为 VContainer 子作用域。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class GameModuleAttribute : Attribute
    {
        public GameModuleAttribute(string name)
        {
            Name = name;
        }

        /// <summary>模块唯一标识（依赖声明引用此名字，不区分大小写）。</summary>
        public string Name { get; }

        /// <summary>同层无依赖关系时的装配顺序参考，小者先装。</summary>
        public int Priority { get; set; }

        /// <summary>依赖的其他模块名，用于拓扑排序和装配期依赖存在性校验。</summary>
        public string[] Dependencies { get; set; } = Array.Empty<string>();

        /// <summary>模块的 VContainer 注册器（实现 VContainer.IInstaller）。可为空（纯代码模块）。</summary>
        public Type InstallerType { get; set; }

        /// <summary>启动阶段，默认 Bootstrap。</summary>
        public ModuleStartup Startup { get; set; } = ModuleStartup.Bootstrap;
    }
}
