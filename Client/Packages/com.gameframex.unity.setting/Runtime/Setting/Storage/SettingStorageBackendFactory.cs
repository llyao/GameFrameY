using System;
using GameFrameX.Runtime;

namespace GameFrameX.Setting.Runtime
{
    /// <summary>
    /// 创建当前运行环境使用的配置存储后端。小游戏渠道包在启动阶段注册平台后端，Editor 与未注册任何渠道包的场景回落 Unity PlayerPrefs。
    /// </summary>
    /// <remarks>
    /// Creates the setting storage backend for the current runtime environment. Mini-game channel packages register their platform backend during startup; Editor builds and projects without a registered channel package fall back to Unity PlayerPrefs.
    /// </remarks>
    public static class SettingStorageBackendFactory
    {
        private static Func<ISettingStorageBackend> s_MiniGameBackendFactory;

        /// <summary>
        /// 注册小游戏平台配置存储后端工厂。渠道包通过 <c>RuntimeInitializeOnLoadMethod(SubsystemRegistration)</c> 自注册（引用即注册），注册时机早于 <see cref="SettingComponent"/> 首次创建后端。
        /// 重复注册时保留首个并输出 Warning——存储后端是"每运行环境至多一个"的互斥语义，若工程同时编译了两个渠道包，首个注册生效，后注册被忽略。
        /// </summary>
        /// <remarks>
        /// Registers a mini-game setting storage backend factory. Channel packages self-register via <c>RuntimeInitializeOnLoadMethod(SubsystemRegistration)</c> (reference implies registration), before <see cref="SettingComponent"/> first creates a backend.
        /// Duplicate registrations keep the first and log a warning: at most one storage backend exists per runtime environment, so if a project compiles two channel packages, the first registration wins.
        /// </remarks>
        /// <param name="backendFactory">后端工厂，返回平台存储后端实例</param>
        /// <exception cref="GameFrameworkException">后端工厂为 null 时抛出</exception>
        [UnityEngine.Scripting.Preserve]
        public static void RegisterMiniGameBackend(Func<ISettingStorageBackend> backendFactory)
        {
            if (backendFactory == null)
            {
                throw new GameFrameworkException("Storage backend factory is invalid.");
            }

            if (s_MiniGameBackendFactory != null)
            {
                Log.Warning("Mini-game setting storage backend is already registered. The duplicate registration is ignored.");
                return;
            }

            s_MiniGameBackendFactory = backendFactory;
        }

        /// <summary>
        /// 创建当前运行环境使用的存储后端。Editor 直接使用 Unity PlayerPrefs（小游戏 SDK 在 Editor 内不可用）；真机未注册任何渠道包时同样回落 Unity PlayerPrefs。
        /// </summary>
        /// <remarks>
        /// Creates the storage backend for the current runtime environment. Editor builds use Unity PlayerPrefs directly (mini-game SDKs are unavailable in the Editor); on device, projects without a registered channel package also fall back to Unity PlayerPrefs.
        /// </remarks>
        /// <returns>存储后端实例</returns>
        [UnityEngine.Scripting.Preserve]
        public static ISettingStorageBackend Create()
        {
#if UNITY_EDITOR
            return new UnityPlayerPrefsSettingStorage();
#else
            return s_MiniGameBackendFactory == null ? new UnityPlayerPrefsSettingStorage() : s_MiniGameBackendFactory();
#endif
        }

        /// <summary>
        /// 重置已注册的小游戏后端工厂。仅供单元测试在用例间清理注册状态使用。
        /// </summary>
        /// <remarks>
        /// Resets the registered mini-game backend factory. Test-only hook for cleaning registration state between test cases.
        /// </remarks>
        internal static void ResetMiniGameBackendForTests()
        {
            s_MiniGameBackendFactory = null;
        }

        /// <summary>
        /// 获取当前已注册的小游戏后端工厂（未注册时为 null）。仅供单元测试断言"保留首个"语义使用。
        /// </summary>
        /// <remarks>
        /// Gets the currently registered mini-game backend factory (null when unregistered). Test-only hook for asserting the keep-first registration semantics.
        /// </remarks>
        internal static Func<ISettingStorageBackend> MiniGameBackendFactoryForTests => s_MiniGameBackendFactory;
    }
}
