using GameFrameY.Core.Module;
using VContainer;

namespace GameFrameY.Bridge.Scene
{
    [GameModule("Scene", Priority = 990, InstallerType = typeof(SceneModuleInstaller))]
    public sealed class SceneModule : GameModuleBase
    {
    }

    public sealed class SceneModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<ISceneService>(_ => new SceneService(), Lifetime.Singleton);
        }
    }
}
