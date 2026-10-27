using Cysharp.Threading.Tasks;
using GameFrameY.Bridge.Config;
using GameFrameY.Core;
using GameFrameY.Core.Module;
using UnityEngine;

namespace GameFrameY.Game.Modules.Demo
{
    /// <summary>
    /// 端到端验证模块（Deferred 阶段装配，依赖 Config/Asset）：
    /// 启动后读取 Luban 配置打印日志，并演示动态服务登记与发现、模块 Tick。
    /// </summary>
    [GameModule("Demo", Startup = ModuleStartup.Deferred, Priority = 100, Dependencies = new[] { "Config", "Asset" })]
    public sealed class DemoModule : GameModuleBase, ITickableModule
    {
        private float _tickTimer;
        private int _tickCount;

        public override UniTask OnStartAsync()
        {
            var config = Services.GetService<IConfigService>();

            // 1) 经资源服务加载的 Luban 配置读取（YooAsset → bytes → cfg.Tables）
            var item = config.Tables.TbItem.GetOrDefault(10001);
            var global = config.Tables.TbGlobal;
            GfyLog.Info(
                $"[Demo] TbItem[10001] = {item?.Name}（{item?.Desc}）价格 {item?.Price} 品质 {item?.Quality}；" +
                $"TbGlobal.HotfixEnabled = {global.HotfixEnabled}，PatchUrl = {global.PatchUrl}");

            // 2) 动态服务登记（中央注册表 + 变更事件 → 其他模块可 WaitServiceAsync 发现）
            RegisterService<IDemoGreetingService>(new DemoGreetingService());
            var greeting = Services.GetService<IDemoGreetingService>().SayHello("GameFrameY");
            GfyLog.Info($"[Demo] {greeting}");

            return UniTask.CompletedTask;
        }

        public void OnTick(float elapseSeconds, float realElapseSeconds)
        {
            // 3) 模块 Tick 泵验证（VContainer PlayerLoop → GameModuleManager → 本模块）
            _tickTimer += realElapseSeconds;
            if (_tickTimer < 3f)
            {
                return;
            }

            _tickTimer = 0f;
            GfyLog.Info($"[Demo] Tick 心跳 #{++_tickCount}");
        }
    }
}
