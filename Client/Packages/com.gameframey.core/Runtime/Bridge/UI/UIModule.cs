using Cysharp.Threading.Tasks;
using GameFrameY.Core.Module;
using VContainer;

namespace GameFrameY.Bridge.UI
{
    [GameModule("UI", Priority = 800, InstallerType = typeof(UIModuleInstaller))]
    public sealed class UIModule : GameModuleBase
    {
        private readonly IUIService _service;

        public UIModule(IUIService service)
        {
            _service = service;
        }

        public override UniTask OnInitializeAsync()
        {
            RegisterService(_service);
            return UniTask.CompletedTask;
        }
    }

    public sealed class UIModuleInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IUIService>(_ => new UIService(), Lifetime.Singleton);
        }
    }
}
