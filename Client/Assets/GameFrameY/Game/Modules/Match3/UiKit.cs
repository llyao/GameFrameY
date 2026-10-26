using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameY.Game.Modules.Match3
{
    /// <summary>运行时 UGUI 构建工具（无 prefab 依赖）。</summary>
    internal static class UiKit
    {
        private static Font _font;

        public static Font DefaultFont
        {
            get
            {
                if (_font != null)
                {
                    return _font;
                }

                try
                {
                    _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                catch
                {
                    _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }

                return _font;
            }
        }

        public static RectTransform Rect(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        /// <summary>居中锚点元素：anchoredPosition + sizeDelta 定位。</summary>
        public static RectTransform Centered(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            return rect;
        }

        public static Image Image(RectTransform parent, string name, Sprite sprite, Color color)
        {
            var rect = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            var image = rect.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        public static Text Text(RectTransform parent, string name, string content, int fontSize,
            Color color, TextAnchor alignment = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
        {
            var rect = new GameObject(name, typeof(RectTransform), typeof(Text)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600, fontSize + 16f);
            var text = rect.GetComponent<Text>();
            text.font = DefaultFont;
            text.text = content;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        public static Button Button(RectTransform parent, string name, string label, Sprite bg,
            Vector2 pos, Vector2 size, Action onClick)
        {
            var rect = Centered(parent, name, pos, size);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = bg;
            image.color = Color.white;
            image.raycastTarget = true;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var labelRect = Centered(rect, "Label", Vector2.zero, size);
            var text = labelRect.gameObject.AddComponent<Text>();
            text.font = DefaultFont;
            text.text = label;
            text.fontSize = 44;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            button.onClick.AddListener(() => onClick?.Invoke());
            return button;
        }

        /// <summary>简易补间：每帧回调 t∈[0,1]。对象销毁后自动停止。</summary>
        public static async UniTask Tween(GameObject context, float duration, Action<float> onUpdate,
            CancellationToken ct = default)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                await UniTask.Yield(ct);
                if (context == null)
                {
                    return;
                }

                elapsed += Time.deltaTime;
                onUpdate(Mathf.Clamp01(elapsed / duration));
            }

            if (context != null)
            {
                onUpdate(1f);
            }
        }

        public static void DestroyAllChildren(RectTransform rect)
        {
            for (var i = rect.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(rect.GetChild(i).gameObject);
            }
        }
    }
}
