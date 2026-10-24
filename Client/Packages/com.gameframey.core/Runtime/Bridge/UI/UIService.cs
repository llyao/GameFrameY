using System.Threading.Tasks;
using GameFrameX.Runtime;
using GameFrameX.UI.Runtime;

namespace GameFrameY.Bridge.UI
{
    /// <summary>UI 服务：包装 GameFrameX UIComponent（UGUI 后端）。具体 UI 面板按 GameFrameX UI 规范实现 IUIForm。</summary>
    public interface IUIService
    {
        Task<T> OpenUIAsync<T>(string uiFormAssetPath, object userData = null) where T : class, IUIForm;

        void CloseUI<T>(object userData = null) where T : IUIForm;
    }

    public sealed class UIService : IUIService
    {
        private static UIComponent Component => GameEntry.GetComponent<UIComponent>();

        public Task<T> OpenUIAsync<T>(string uiFormAssetPath, object userData = null) where T : class, IUIForm
        {
            return Component.OpenAsync<T>(uiFormAssetPath, userData);
        }

        public void CloseUI<T>(object userData = null) where T : IUIForm
        {
            Component.CloseUIForm<T>(userData);
        }
    }
}
