using GameFrameY.Core.Module;
using VContainer;

namespace GameFrameY.Bridge.Asset
{
    /// <summary>资源模块：把 IAssetService 注册进模块作用域（Bootstrap 阶段装配，服务惰性解析组件）。</summary>
    [GameModule("Asset", Priority = 1000, InstallerType = typeof(AssetModuleInstaller))]
    public sealed class AssetModule : GameModuleBase
    {
    }

    public sealed class AssetModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IAssetService>(_ => new AssetService(), Lifetime.Singleton);
        }
    }
}
