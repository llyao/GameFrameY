using GameFrameY.Core.Module;
using GameFrameY.Core.Service;
using GameFrameY.Core.Startup;
using VContainer;
using VContainer.Unity;

namespace GameFrameY.Bridge
{
    /// <summary>根作用域注册：服务注册表 / 模块管理器 / 启动协调器 / 启动配置。</summary>
    public sealed class GameFrameYRootInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IServiceRegistry>(resolver => new ServiceRegistry(resolver), Lifetime.Singleton);
            builder.RegisterInstance(new GameFrameYLaunchSettings());
            builder.RegisterInstance(new GameFrameYStartupStatus());
            builder.RegisterEntryPoint<GameModuleManager>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<GameFrameYGameLauncher>(Lifetime.Singleton);
        }
    }
}
