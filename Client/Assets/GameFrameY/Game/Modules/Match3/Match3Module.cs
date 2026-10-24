using Cysharp.Threading.Tasks;
using GameFrameY.Bridge.Asset;
using GameFrameY.Bridge.Config;
using GameFrameY.Bridge.Event;
using GameFrameY.Core;
using GameFrameY.Core.Module;

namespace GameFrameY.Game.Modules.Match3
{
    /// <summary>
    /// 消消乐模块（Deferred 装配，依赖 Asset/Config）：
    /// Luban 关卡表 + YooAsset 程序生成贴图 + 运行时 UGUI + IEventService 事件，全链路走框架服务。
    /// </summary>
    [GameModule("Match3", Startup = ModuleStartup.Deferred, Priority = 90,
        Dependencies = new[] { "Asset", "Config" })]
    public sealed class Match3Module : GameModuleBase
    {
        private Match3Assets _assets;
        private Match3UI _ui;
        private Match3GameController _controller;

        public override async UniTask OnStartAsync()
        {
            var config = Services.GetService<IConfigService>();
            var asset = Services.GetService<IAssetService>();

            _assets = new Match3Assets(asset);
            await _assets.LoadAsync();

            _ui = new Match3UI(_assets);
            _ui.Build();

            _controller = new Match3GameController(config.Tables, _ui, Services.GetService<IEventService>());
            RegisterService<IMatch3Service>(_controller);

            _controller.StartLevel(1);
        }

        public override UniTask OnShutdownAsync()
        {
            Services?.Unregister<IMatch3Service>(_controller);
            _ui?.Dispose();
            _assets?.Dispose();
            return UniTask.CompletedTask;
        }
    }
}
