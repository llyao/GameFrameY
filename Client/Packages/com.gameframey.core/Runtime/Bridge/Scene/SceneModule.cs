using Cysharp.Threading.Tasks;
using GameFrameY.Core.Module;
using VContainer;

namespace GameFrameY.Bridge.Scene
{
    [GameModule("Scene", Priority = 990, InstallerType = typeof(SceneModuleInstaller))]
    public sealed class SceneModule : GameModuleBase
    {
        private readonly ISceneService _service;

        public SceneModule(ISceneService service)
        {
            _service = service;
        }

        public override UniTask OnInitializeAsync()
        {
            RegisterService(_service);
            return UniTask.CompletedTask;
        }
    }

    public sealed class SceneModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<ISceneService>(_ => new SceneService(), Lifetime.Singleton);
        }
    }
}
