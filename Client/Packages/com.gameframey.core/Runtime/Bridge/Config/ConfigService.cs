using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using GameFrameY.Bridge.Asset;
using Luban;
using UnityEngine;
using YooAsset;

namespace GameFrameY.Bridge.Config
{
    /// <summary>配置服务：暴露 Luban 生成的 cfg.Tables，并负责经资源服务预载全部表数据。</summary>
    public interface IConfigService
    {
        /// <summary>Luban 生成的表集合（LoadAsync 完成前为 null）。</summary>
        cfg.Tables Tables { get; }

        /// <summary>预载 Bundles/Config 下全部表 bytes 并构造 Tables（幂等）。</summary>
        UniTask LoadAsync();
    }

    public sealed class ConfigService : IConfigService
    {
        private readonly IAssetService _assetService;

        public ConfigService(IAssetService assetService)
        {
            _assetService = assetService;
        }

        public cfg.Tables Tables { get; private set; }

        public async UniTask LoadAsync()
        {
            if (Tables != null)
            {
                return;
            }

            // cfg.Tables 的构造委托按表逻辑名取 ByteBuf（如 "tbitem"，见生成的 Tables.cs）。
            // 这里按属性名反射预载（属性名小写 == bytes 文件名 == YooAsset 地址）。
            var properties = typeof(cfg.Tables).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var bytesByName = new Dictionary<string, byte[]>(properties.Length, System.StringComparer.OrdinalIgnoreCase);
            var handles = new List<AssetHandle>(properties.Length);
            try
            {
                foreach (var property in properties)
                {
                    var tableFileName = property.Name.ToLowerInvariant();
                    var handle = await _assetService.LoadAssetAsync<TextAsset>(tableFileName);
                    if (handle == null || handle.Status != EOperationStatus.Succeed || handle.AssetObject is not TextAsset textAsset)
                    {
                        var error = handle?.LastError ?? "asset handle is null";
                        if (handle != null)
                        {
                            _assetService.ReleaseHandle(handle);
                        }
                        throw new System.Exception(
                            $"Luban 配置资源加载失败: location={tableFileName}, error={error}。" +
                            "请确认 YooAsset Bundle Collector 已开启 EnableAddressable 并重新执行模拟构建。");
                    }

                    handles.Add(handle);
                    bytesByName[tableFileName] = textAsset.bytes;
                }

                Tables = new cfg.Tables(name => new ByteBuf(bytesByName[name]));
            }
            finally
            {
                foreach (var handle in handles)
                {
                    _assetService.ReleaseHandle(handle);
                }
            }
        }
    }
}
