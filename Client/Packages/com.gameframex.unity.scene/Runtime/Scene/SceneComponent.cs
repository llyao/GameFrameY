// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
// 
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
// 
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
// 
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
// 
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.Event.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFrameX.Asset.Runtime;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.Scene.Runtime
{
    /// <summary>
    /// 场景组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("GameFrameX/Scene")]
    [GameFrameXAutoComponent(-4000)]
    public sealed class SceneComponent : GameFrameworkComponent
    {
        private const int DefaultPriority = 0;

        private IGameSceneManager _gameSceneManager = null;
        private IAssetManager _assetManager = null;
        private IEventManager m_EventManager = null;

        private readonly SortedDictionary<string, int> m_SceneOrder = new SortedDictionary<string, int>(StringComparer.Ordinal);

        private Camera m_MainCamera = null;
        private UnityEngine.SceneManagement.Scene m_GameFrameworkScene = default(UnityEngine.SceneManagement.Scene);
        private Coroutine m_RefreshSceneOrderCo = null;

        private bool m_EnableLoadSceneUpdateEvent = true;

        private bool m_EnableLoadSceneDependencyAssetEvent = true;

        /// <summary>
        /// 获取当前场景主摄像机。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public Camera MainCamera
        {
            get { return m_MainCamera; }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            ImplementationComponentType = Utility.Assembly.GetType(componentType);
            InterfaceComponentType = typeof(IGameSceneManager);
            base.Awake();
            if (GameFrameXOptions.TryGet<SceneOptions>(out var sceneOptions))
            {
                m_EnableLoadSceneUpdateEvent = sceneOptions.EnableLoadSceneUpdateEvent;
                m_EnableLoadSceneDependencyAssetEvent = sceneOptions.EnableLoadSceneDependencyAssetEvent;
            }

            _gameSceneManager = GameFrameworkEntry.GetModule<IGameSceneManager>();
            if (_gameSceneManager == null)
            {
                Log.Fatal("Scene manager is invalid.");
                return;
            }

            _gameSceneManager.LoadSceneSuccess += OnLoadGameSceneSuccess;
            _gameSceneManager.LoadSceneFailure += OnLoadGameSceneFailure;

            if (m_EnableLoadSceneUpdateEvent)
            {
                _gameSceneManager.LoadSceneUpdate += OnLoadGameSceneUpdate;
            }

            if (m_EnableLoadSceneDependencyAssetEvent)
            {
                // _gameSceneManager.LoadSceneDependencyAsset += OnLoadGameSceneDependencyAsset;
            }

            _gameSceneManager.UnloadSceneSuccess += OnUnloadGameSceneSuccess;
            _gameSceneManager.UnloadSceneFailure += OnUnloadGameSceneFailure;

            m_GameFrameworkScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(GameEntry.GameFrameworkSceneId);
            if (!m_GameFrameworkScene.IsValid())
            {
                Log.Fatal("GameFrameX scene is invalid.");
                return;
            }
        }

        private void Start()
        {
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            m_EventManager = GameFrameworkEntry.GetModule<IEventManager>();
            if (m_EventManager == null)
            {
                Log.Fatal("Event component is invalid.");
                return;
            }

            _assetManager = GameFrameworkEntry.GetModule<IAssetManager>();
            if (_assetManager == null)
            {
                Log.Fatal("Asset Manager is invalid.");
                return;
            }

            _gameSceneManager.SetResourceManager(_assetManager);
        }

        /// <summary>
        /// 获取场景名称。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景名称。</returns>
        [UnityEngine.Scripting.Preserve]
        public static string GetSceneName(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return null;
            }

            int sceneNamePosition = sceneAssetName.LastIndexOf('/');
            if (sceneNamePosition + 1 >= sceneAssetName.Length)
            {
                Log.Error("Scene asset name '{0}' is invalid.", sceneAssetName);
                return null;
            }

            string sceneName = sceneAssetName.Substring(sceneNamePosition + 1);
            sceneNamePosition = sceneName.LastIndexOf(".unity");
            if (sceneNamePosition > 0)
            {
                sceneName = sceneName.Substring(0, sceneNamePosition);
            }

            return sceneName;
        }

        /// <summary>
        /// 获取场景是否已加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否已加载。</returns>
        [UnityEngine.Scripting.Preserve]
        public bool SceneIsLoaded(string sceneAssetName)
        {
            return _gameSceneManager.SceneIsLoaded(sceneAssetName);
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <returns>已加载场景的资源名称。</returns>
        [UnityEngine.Scripting.Preserve]
        public string[] GetLoadedSceneAssetNames()
        {
            return _gameSceneManager.GetLoadedSceneAssetNames();
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <param name="results">已加载场景的资源名称。</param>
        [UnityEngine.Scripting.Preserve]
        public void GetLoadedSceneAssetNames(List<string> results)
        {
            _gameSceneManager.GetLoadedSceneAssetNames(results);
        }

        /// <summary>
        /// 获取场景是否正在加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在加载。</returns>
        [UnityEngine.Scripting.Preserve]
        public bool SceneIsLoading(string sceneAssetName)
        {
            return _gameSceneManager.SceneIsLoading(sceneAssetName);
        }

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <returns>正在加载场景的资源名称。</returns>
        [UnityEngine.Scripting.Preserve]
        public string[] GetLoadingSceneAssetNames()
        {
            return _gameSceneManager.GetLoadingSceneAssetNames();
        }

        /// <summary>
        /// 获取正在加载场景的资源名称。
        /// </summary>
        /// <param name="results">正在加载场景的资源名称。</param>
        [UnityEngine.Scripting.Preserve]
        public void GetLoadingSceneAssetNames(List<string> results)
        {
            _gameSceneManager.GetLoadingSceneAssetNames(results);
        }

        /// <summary>
        /// 获取场景是否正在卸载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在卸载。</returns>
        [UnityEngine.Scripting.Preserve]
        public bool SceneIsUnloading(string sceneAssetName)
        {
            return _gameSceneManager.SceneIsUnloading(sceneAssetName);
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <returns>正在卸载场景的资源名称。</returns>
        [UnityEngine.Scripting.Preserve]
        public string[] GetUnloadingSceneAssetNames()
        {
            return _gameSceneManager.GetUnloadingSceneAssetNames();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <param name="results">正在卸载场景的资源名称。</param>
        [UnityEngine.Scripting.Preserve]
        public void GetUnloadingSceneAssetNames(List<string> results)
        {
            _gameSceneManager.GetUnloadingSceneAssetNames(results);
        }

        /// <summary>
        /// 检查场景资源是否存在。
        /// </summary>
        /// <param name="sceneAssetName">要检查场景资源的名称。</param>
        /// <returns>场景资源是否存在。</returns>
        [UnityEngine.Scripting.Preserve]
        public bool HasScene(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return false;
            }

            if (!sceneAssetName.StartsWith("Assets/", StringComparison.Ordinal) ||
                !sceneAssetName.EndsWith(".unity", StringComparison.Ordinal))
            {
                Log.Error("Scene asset name '{0}' is invalid.", sceneAssetName);
                return false;
            }

            return _gameSceneManager.HasScene(sceneAssetName);
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        [UnityEngine.Scripting.Preserve]
        public async Task<YooAsset.SceneHandle> LoadScene(string sceneAssetName)
        {
            return await LoadScene(sceneAssetName, UnityEngine.SceneManagement.LoadSceneMode.Single, null);
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="sceneMode">加载场景资源的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        [UnityEngine.Scripting.Preserve]
        public async Task<YooAsset.SceneHandle> LoadScene(string sceneAssetName, UnityEngine.SceneManagement.LoadSceneMode sceneMode, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                throw new ArgumentNullException(nameof(sceneAssetName));
            }

            if (!sceneAssetName.StartsWith("Assets/", StringComparison.Ordinal) ||
                !sceneAssetName.EndsWith(".unity", StringComparison.Ordinal))
            {
                Log.Error("Scene asset name '{0}' is invalid.", sceneAssetName);
                throw new ArgumentException(nameof(sceneAssetName));
            }

            return await _gameSceneManager.LoadScene(sceneAssetName, sceneMode, userData);
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        [UnityEngine.Scripting.Preserve]
        public void UnloadScene(string sceneAssetName, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new ArgumentNullException(nameof(sceneAssetName));
            }

            if (!sceneAssetName.StartsWith("Assets/", StringComparison.Ordinal) ||
                !sceneAssetName.EndsWith(".unity", StringComparison.Ordinal))
            {
                throw new ArgumentException(string.Format("Scene asset name '{0}' is invalid.", sceneAssetName), nameof(sceneAssetName));
            }

            _gameSceneManager.UnloadScene(sceneAssetName, userData);
        }

        /// <summary>
        /// 设置场景顺序。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="sceneOrder">要设置的场景顺序。</param>
        [UnityEngine.Scripting.Preserve]
        public void SetSceneOrder(string sceneAssetName, int sceneOrder)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return;
            }

            if (!sceneAssetName.StartsWith("Assets/", StringComparison.Ordinal) ||
                !sceneAssetName.EndsWith(".unity", StringComparison.Ordinal))
            {
                Log.Error("Scene asset name '{0}' is invalid.", sceneAssetName);
                return;
            }

            if (SceneIsLoading(sceneAssetName))
            {
                m_SceneOrder[sceneAssetName] = sceneOrder;
                return;
            }

            if (SceneIsLoaded(sceneAssetName))
            {
                m_SceneOrder[sceneAssetName] = sceneOrder;
                RefreshSceneOrder();
                return;
            }

            Log.Error("Scene '{0}' is not loaded or loading.", sceneAssetName);
        }

        /// <summary>
        /// 刷新当前场景主摄像机。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void RefreshMainCamera()
        {
            m_MainCamera = Camera.main;
        }

        private void RefreshSceneOrder()
        {
            if (m_SceneOrder.Count > 0)
            {
                string maxSceneName = null;
                int maxSceneOrder = 0;
                foreach (var sceneOrder in m_SceneOrder)
                {
                    if (SceneIsLoading(sceneOrder.Key))
                    {
                        continue;
                    }

                    if (maxSceneName == null)
                    {
                        maxSceneName = sceneOrder.Key;
                        maxSceneOrder = sceneOrder.Value;
                        continue;
                    }

                    if (sceneOrder.Value > maxSceneOrder)
                    {
                        maxSceneName = sceneOrder.Key;
                        maxSceneOrder = sceneOrder.Value;
                    }
                }

                if (maxSceneName == null)
                {
                    SetActiveScene(m_GameFrameworkScene);
                    return;
                }

                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(GetSceneName(maxSceneName));
                if (!scene.IsValid() || !scene.isLoaded)
                {
                    // YooAsset 的 await 完成时机可能早于 Unity 场景 isLoaded=true，此时 SetActiveScene 会抛 ArgumentException。
                    // 推迟到下一帧场景实际加载完成后再激活。多次连续切换时，旧协程必须先停掉，否则会与新协程争抢 SetActiveScene。
                    if (m_RefreshSceneOrderCo != null)
                    {
                        StopCoroutine(m_RefreshSceneOrderCo);
                        m_RefreshSceneOrderCo = null;
                    }
                    m_RefreshSceneOrderCo = StartCoroutine(RefreshSceneOrderWhenLoadedCo(maxSceneName));
                    return;
                }

                SetActiveScene(scene);
            }
            else
            {
                SetActiveScene(m_GameFrameworkScene);
            }
        }

        private void SetActiveScene(UnityEngine.SceneManagement.Scene activeScene)
        {
            if (!activeScene.IsValid())
            {
                // activeScene 可能因 Single 模式多次切换而过期——Awake 时缓存的 m_GameFrameworkScene
                // 在第一次 Single 加载后即被 Unity 卸载，handle 失效；新场景在异步加载窗口期也可能尚未 valid。
                // 此时 Unity 自己会管理 active scene（Single 新场景加载完成后自动激活），框架跳过本次设置即可，
                // 不需要抛 ArgumentException 也不需要警告污染日志。
                RefreshMainCamera();
                return;
            }

            var lastActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (lastActiveScene != activeScene)
            {
                try
                {
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(activeScene);
                }
                catch (ArgumentException ex)
                {
                    // YooAsset 的 await 完成时机可能早于 Unity 场景完全激活，导致 SetActiveScene 抛 ArgumentException。
                    // 此时 active scene 已是目标场景或很快会被 Unity 自动切换，吞掉异常避免污染调用方；只吞 SetActiveScene 自己的异常，订阅者回调里抛的异常仍会正常冒泡。
                    Log.Warning("SetActiveScene failed (will be retried by Unity internally): {0}", ex.Message);
                    return;
                }

                m_EventManager.Fire(this, ActiveSceneChangedEventArgs.Create(lastActiveScene, activeScene));
            }

            RefreshMainCamera();
        }

        private System.Collections.IEnumerator RefreshSceneOrderWhenLoadedCo(string sceneAssetName)
        {
            try
            {
                var sceneName = GetSceneName(sceneAssetName);
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);

                int retry = 0;
                while ((!scene.IsValid() || !scene.isLoaded) && retry < 60)
                {
                    yield return null;
                    scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
                    retry++;
                }

                if (scene.IsValid() && scene.isLoaded)
                {
                    SetActiveScene(scene);
                }
                else
                {
                    Log.Warning("Scene '{0}' did not become loaded within timeout, fallback to framework scene.", sceneAssetName);
                    SetActiveScene(m_GameFrameworkScene);
                }
            }
            finally
            {
                // 协程结束（正常完成或被 StopCoroutine 终止）时清理句柄，避免下次 RefreshSceneOrder 误判旧协程还在。
                m_RefreshSceneOrderCo = null;
            }
        }

        private void OnLoadGameSceneSuccess(object sender, LoadSceneSuccessEventArgs eventArgs)
        {
            if (!m_SceneOrder.ContainsKey(eventArgs.SceneAssetName))
            {
                m_SceneOrder.Add(eventArgs.SceneAssetName, 0);
            }

            m_EventManager.Fire(this, LoadSceneSuccessEventArgs.Create(eventArgs.SceneAssetName, eventArgs.Duration, eventArgs.UserData));
            RefreshSceneOrder();
        }

        private void OnLoadGameSceneFailure(object sender, LoadSceneFailureEventArgs eventArgs)
        {
            Log.Warning("Load scene failure, scene asset name '{0}', error message '{1}'.", eventArgs.SceneAssetName,
                        eventArgs.ErrorMessage);
            m_EventManager.Fire(this, LoadSceneFailureEventArgs.Create(eventArgs.SceneAssetName, eventArgs.Status, eventArgs.ErrorMessage, eventArgs.UserData));
        }

        private void OnLoadGameSceneUpdate(object sender, LoadSceneUpdateEventArgs eventArgs)
        {
            m_EventManager.Fire(this, LoadSceneUpdateEventArgs.Create(eventArgs.SceneAssetName, eventArgs.Progress, eventArgs.UserData));
        }

        private void OnUnloadGameSceneSuccess(object sender, UnloadSceneSuccessEventArgs eventArgs)
        {
            m_EventManager.Fire(this, UnloadSceneSuccessEventArgs.Create(eventArgs.SceneAssetName, eventArgs.UserData));
            m_SceneOrder.Remove(eventArgs.SceneAssetName);
            RefreshSceneOrder();
        }

        private void OnUnloadGameSceneFailure(object sender, UnloadSceneFailureEventArgs eventArgs)
        {
            Log.Warning("Unload scene failure, scene asset name '{0}'.", eventArgs.SceneAssetName);
            m_EventManager.Fire(this, UnloadSceneFailureEventArgs.Create(eventArgs.SceneAssetName, eventArgs.UserData));
        }
    }
}
