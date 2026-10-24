using Cysharp.Threading.Tasks;

namespace GameFrameY.Core.Module
{
    /// <summary>
    /// 模块契约。模块实例由所在子作用域创建（支持构造注入），生命周期由
    /// <see cref="GameModuleManager"/> 统一驱动；帧驱动实现 <c>ITickableModule</c> 等可选接口。
    /// </summary>
    public interface IGameModule
    {
        /// <summary>模块描述符，装配时由管理器回填。</summary>
        GameModuleDescriptor Descriptor { get; set; }

        /// <summary>子作用域构建完成、构造注入结束后调用（按装配顺序串行 await）。</summary>
        UniTask OnInitializeAsync();

        /// <summary>自身及全部依赖模块 Initialize 完成后调用。</summary>
        UniTask OnStartAsync();

        /// <summary>卸载前调用（按装配逆序）。随后子作用域被 Dispose。</summary>
        UniTask OnShutdownAsync();
    }

    /// <summary>可选：模块需要每帧 Update 时机回调。</summary>
    public interface ITickableModule
    {
        void OnTick(float elapseSeconds, float realElapseSeconds);
    }

    /// <summary>可选：模块需要 FixedUpdate 时机回调。</summary>
    public interface IFixedTickableModule
    {
        void OnFixedTick(float elapseSeconds, float realElapseSeconds);
    }

    /// <summary>可选：模块需要 LateUpdate 时机回调。</summary>
    public interface ILateTickableModule
    {
        void OnLateTick(float elapseSeconds, float realElapseSeconds);
    }
}
