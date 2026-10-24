using Cysharp.Threading.Tasks;
using GameFrameX.Asset.Runtime;
using GameFrameX.Runtime;
using UnityEngine;
using YooAsset;

namespace GameFrameY.Bridge.Asset
{
    /// <summary>资源服务：包装 GameFrameX AssetComponent（底层为 YooAsset）。</summary>
    public interface IAssetService
    {
        /// <summary>
        /// 初始化默认资源包。
        /// EditorSimulateMode 下组件自管理（启动时已按 AssetOptions 初始化，跳过避免重复初始化）；
        /// Offline/HostPlayMode 传入主/备 CDN 地址走 InitPackageAsync。
        /// </summary>
        UniTask<bool> InitializeAsync(YooAsset.EPlayMode playMode, string hostUrl = null, string fallbackHostUrl = null);

        /// <summary>按地址异步加载资源，返回 YooAsset 句柄（用毕 ReleaseHandle / handle.Release()）。</summary>
        UniTask<AssetHandle> LoadAssetAsync<T>(string location) where T : UnityEngine.Object;

        AssetHandle LoadAssetSync(string location);

        /// <summary>统一释放句柄（内部转发 AssetComponent.UnloadAssetHandle）。</summary>
        void ReleaseHandle(object handle);
    }

    public sealed class AssetService : IAssetService
    {
        private static AssetComponent Component => GameEntry.GetComponent<AssetComponent>();

        public async UniTask<bool> InitializeAsync(YooAsset.EPlayMode playMode, string hostUrl = null, string fallbackHostUrl = null)
        {
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                return true;
            }

            var ok = await Component.InitPackageAsync(
                AssetComponent.BuildInPackageName,
                hostUrl ?? string.Empty,
                fallbackHostUrl ?? string.Empty,
                true);
            return ok;
        }

        public async UniTask<AssetHandle> LoadAssetAsync<T>(string location) where T : UnityEngine.Object
        {
            var handle = await Component.LoadAssetAsync<T>(location);
            return handle;
        }

        public AssetHandle LoadAssetSync(string location)
        {
            return Component.LoadAssetSync(location);
        }

        public void ReleaseHandle(object handle)
        {
            Component.UnloadAssetHandle(handle);
        }
    }
}
