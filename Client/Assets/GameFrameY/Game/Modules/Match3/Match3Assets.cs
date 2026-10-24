using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFrameY.Bridge.Asset;
using GameFrameY.Core;
using UnityEngine;
using YooAsset;

namespace GameFrameY.Game.Modules.Match3
{
    /// <summary>
    /// 消消乐资源：优先经 YooAsset（IAssetService）加载 Atlases 组的程序生成贴图；
    /// 任一加载失败则回退为运行时程序化生成（保证游戏可玩，同时给出警告日志）。
    /// </summary>
    public sealed class Match3Assets : IDisposable
    {
        private static readonly string[] GemLocations =
        {
            "", "gem_red", "gem_orange", "gem_yellow", "gem_green", "gem_blue", "gem_purple",
        };

        private static readonly Color32[] GemColors =
        {
            default, new(255, 86, 86), new(255, 158, 62), new(255, 214, 72),
            new(98, 220, 90), new(84, 158, 255), new(176, 106, 255),
        };

        private readonly IAssetService _assetService;
        private readonly List<AssetHandle> _handles = new();

        public Sprite[] GemSprites { get; } = new Sprite[7];

        public Sprite CellBg { get; private set; }

        public Sprite PanelBg { get; private set; }

        public Sprite ButtonBg { get; private set; }

        public Match3Assets(IAssetService assetService)
        {
            _assetService = assetService;
        }

        public async UniTask LoadAsync()
        {
            for (var kind = 1; kind <= 6; kind++)
            {
                GemSprites[kind] = await LoadSprite(GemLocations[kind], fallbackColor: GemColors[kind]);
            }

            CellBg = await LoadSprite("cell_bg", fallbackColor: new Color32(255, 255, 255, 26));
            PanelBg = await LoadSprite("panel_bg", fallbackColor: new Color32(30, 34, 52, 242));
            ButtonBg = await LoadSprite("btn_bg", fallbackColor: new Color32(66, 104, 220, 235));
        }

        private async UniTask<Sprite> LoadSprite(string location, Color32 fallbackColor)
        {
            try
            {
                var handle = await _assetService.LoadAssetAsync<Texture2D>(location);
                if (handle != null && handle.AssetObject is Texture2D texture)
                {
                    _handles.Add(handle);
                    return Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f), 100f);
                }

                GfyLog.Warn($"[Match3] 贴图 {location} 加载失败（YooAsset 无该地址或状态失败），使用程序化兜底");
            }
            catch (Exception e)
            {
                GfyLog.Warn($"[Match3] 贴图 {location} 加载异常: e={e.Message}，使用程序化兜底");
            }

            return CreateProceduralSprite(fallbackColor);
        }

        private static Sprite CreateProceduralSprite(Color32 color)
        {
            const int size = 64;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var cx = (size - 1) * 0.5f;
            var radius = size * 0.46f;
            var pixels = new Color32[size * size];
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - cx;
                    var dy = y - cx;
                    var d = Mathf.Sqrt(dx * dx + dy * dy);
                    var edge = Mathf.Clamp01((radius - d) / 1.5f);
                    var shade = 1f - Mathf.Clamp01(d / radius) * 0.25f;
                    pixels[y * size + x] = new Color(
                        color.r / 255f * shade, color.g / 255f * shade, color.b / 255f * shade, edge);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(true, false);
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        public void Dispose()
        {
            foreach (var handle in _handles)
            {
                _assetService.ReleaseHandle(handle);
            }

            _handles.Clear();
        }
    }
}
