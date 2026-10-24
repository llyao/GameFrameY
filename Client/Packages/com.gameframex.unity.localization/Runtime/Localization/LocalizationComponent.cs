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
using GameFrameX.Runtime;
using GameFrameX.Setting.Runtime;
using UnityEngine;
using System;

namespace GameFrameX.Localization.Runtime
{
    /// <summary>
    /// 本地化组件。
    /// </summary>
    /// <remarks>
    /// Localization component.
    /// </remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("GameFrameX/Localization")]
    [HelpURL("https://datatracker.ietf.org/doc/html/rfc5646")]
    [UnityEngine.Scripting.Preserve]
    [RequireComponent(typeof(GameFrameXLocalizationCroppingHelper))]
    [GameFrameXAutoComponent(700)]
    public sealed class LocalizationComponent : GameFrameworkComponent
    {
        private ILocalizationManager m_LocalizationManager = null;

        private IEventManager m_EventManager = null;
        private SettingComponent m_SettingComponent = null;

        /// <summary>
        /// 未知本地化
        /// </summary>
        /// <remarks>
        /// Unknown localization
        /// </remarks>
        const string UnknownLocalization = LocalizationManager.UnknownLocalization;

        private string m_EditorLanguage = "zh_CN";

        private string m_DefaultLanguage = "zh_CN";

        private bool m_EnableLoadDictionaryUpdateEvent = false;
        private bool m_IsEnableEditorMode = false;

        private Func<LocalizationHelperBase> m_LocalizationHelperFactory = () => new GameObject().AddComponent<DefaultLocalizationHelper>();

        [SerializeField] private LocalizationHelperBase m_CustomLocalizationHelper = null;


        /// <summary>
        /// 获取或设置编辑器语言（仅编辑器内有效）。
        /// </summary>
        /// <remarks>
        /// Gets or sets the editor language (only effective in editor).
        /// </remarks>
        /// <value>
        /// 编辑器语言（仅编辑器内有效） / Editor language (only effective in editor)
        /// </value>
        [UnityEngine.Scripting.Preserve]
        public string EditorLanguage
        {
            get { return m_EditorLanguage; }
            set
            {
                // 启动期型参数：仅模块首次实例化前经 GameFrameXOptions.Configure 生效，运行期修改响亮拒绝
                Log.Warning("EditorLanguage 是启动期参数，运行期修改已被拒绝，请在启动前经 GameFrameXOptions.Configure 配置。");
            }
        }

        /// <summary>
        /// 获取或设置 默认本地化语言。当加载本地化失败时使用。
        /// </summary>
        /// <remarks>
        /// Gets or sets the default localization language, used when localization loading fails.
        /// </remarks>
        /// <value>
        /// 默认本地化语言 / Default localization language
        /// </value>
        [UnityEngine.Scripting.Preserve]
        public string DefaultLanguage
        {
            get
            {
                if (m_LocalizationManager.DefaultLanguage == UnknownLocalization)
                {
                    var value = m_SettingComponent.GetString(nameof(LocalizationComponent) + "." + nameof(DefaultLanguage));
                    if (value.IsNotNullOrWhiteSpace())
                    {
                        m_LocalizationManager.DefaultLanguage = value;
                    }
                }

                return m_LocalizationManager.DefaultLanguage;
            }
            set
            {
                // 启动期型参数：仅模块首次实例化前经 GameFrameXOptions.Configure 生效，运行期修改响亮拒绝
                Log.Warning("DefaultLanguage 是启动期参数，运行期修改已被拒绝，请在启动前经 GameFrameXOptions.Configure 配置。");
            }
        }

        /// <summary>
        /// 获取或设置本地化语言。
        /// </summary>
        /// <remarks>
        /// Gets or sets the localization language.
        /// </remarks>
        /// <value>
        /// 本地化语言 / Localization language
        /// </value>
        [UnityEngine.Scripting.Preserve]
        public string Language
        {
            get
            {
                if (m_LocalizationManager.Language == UnknownLocalization)
                {
                    var value = m_SettingComponent.GetString(nameof(LocalizationComponent) + "." + nameof(Language));
                    if (value.IsNotNullOrWhiteSpace())
                    {
                        m_LocalizationManager.Language = value;
                    }
                }

                return m_LocalizationManager.Language;
            }
            set
            {
                if (m_LocalizationManager.Language != value)
                {
                    var oldLanguage = m_LocalizationManager.Language;
                    m_LocalizationManager.Language = value;
                    m_SettingComponent.SetString(nameof(LocalizationComponent) + "." + nameof(Language), value);
                    m_SettingComponent.Save();
                    var localizationLanguageChangeBeforeEventArgs = LocalizationLanguageChangeBeforeEventArgs.Create(oldLanguage, value);
                    m_EventManager.Fire(this, localizationLanguageChangeBeforeEventArgs);
                    var localizationLanguageChangeEventArgs = LocalizationLanguageChangeEventArgs.Create(oldLanguage, value);
                    m_EventManager.Fire(this, localizationLanguageChangeEventArgs);
                    var localizationLanguageChangeAfterEventArgs = LocalizationLanguageChangeAfterEventArgs.Create(oldLanguage, value);
                    m_EventManager.Fire(this, localizationLanguageChangeAfterEventArgs);
                }
            }
        }

        /// <summary>
        /// 获取系统语言。
        /// </summary>
        /// <remarks>
        /// Gets the system language.
        /// </remarks>
        /// <value>
        /// 系统语言 / System language
        /// </value>
        [UnityEngine.Scripting.Preserve]
        public string SystemLanguage
        {
            get { return m_LocalizationManager.SystemLanguage; }
        }

        /// <summary>
        /// 获取字典数量。
        /// </summary>
        /// <remarks>
        /// Gets the dictionary count.
        /// </remarks>
        /// <value>
        /// 字典数量 / Dictionary count
        /// </value>
        [UnityEngine.Scripting.Preserve]
        public int DictionaryCount
        {
            get { return m_LocalizationManager.DictionaryCount; }
        }


        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        /// <remarks>
        /// Game framework component initialization.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        protected override void Awake()
        {
            ImplementationComponentType = Utility.Assembly.GetType(componentType);
            InterfaceComponentType = typeof(ILocalizationManager);
            base.Awake();
            m_LocalizationManager = GameFrameworkEntry.GetModule<ILocalizationManager>();
            if (m_LocalizationManager == null)
            {
                Log.Fatal("Localization manager is invalid.");
                return;
            }

            if (GameFrameXOptions.TryGet<LocalizationOptions>(out var localizationOptions))
            {
                m_EditorLanguage = localizationOptions.EditorLanguage;
                m_DefaultLanguage = localizationOptions.DefaultLanguage;
                m_EnableLoadDictionaryUpdateEvent = localizationOptions.EnableLoadDictionaryUpdateEvent;
                m_IsEnableEditorMode = localizationOptions.IsEnableEditorMode;
                m_LocalizationHelperFactory = localizationOptions.LocalizationHelperFactory;
            }
        }

        [UnityEngine.Scripting.Preserve]
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

            m_SettingComponent = GameEntry.GetComponent<SettingComponent>();
            if (m_SettingComponent == null)
            {
                Log.Fatal("Setting component is invalid.");
                return;
            }


            LocalizationHelperBase localizationHelper = Helper.CreateHelper(m_LocalizationHelperFactory, m_CustomLocalizationHelper);
            if (localizationHelper == null)
            {
                Log.Fatal("Can not create localization helper.");
                return;
            }

            localizationHelper.name = "Localization Helper";
            Transform localizationHelperTransform = localizationHelper.transform;
            localizationHelperTransform.SetParent(this.transform);
            localizationHelperTransform.localScale = Vector3.one;
            m_LocalizationManager.SetLocalizationHelper(localizationHelper);
#if UNITY_EDITOR
            if (m_IsEnableEditorMode)
            {
                Language = EditorLanguage != UnknownLocalization ? EditorLanguage : m_LocalizationManager.SystemLanguage;
            }
#else
            if (Language == UnknownLocalization)
            {
                Language = m_LocalizationManager.SystemLanguage;
            }
#endif
            // 启动期消费路径直接写管理器，绕开已被改为运行期拒绝的 DefaultLanguage setter
            m_LocalizationManager.DefaultLanguage = m_DefaultLanguage != UnknownLocalization ? m_DefaultLanguage : m_LocalizationManager.SystemLanguage;
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString(string key)
        {
            return m_LocalizationManager.GetString(key);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T">字典参数的类型 / Type of dictionary parameter</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="args">字典参数列表 / Dictionary parameter list</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString(string key, params object[] args)
        {
            return m_LocalizationManager.GetString(key, args);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T">字典参数的类型 / Type of dictionary parameter</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg">字典参数 / Dictionary parameter</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T>(string key, T arg)
        {
            return m_LocalizationManager.GetString(key, arg);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2>(string key, T1 arg1, T2 arg2)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3>(string key, T1 arg1, T2 arg2, T3 arg3)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型 / Type of dictionary parameter 9</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <param name="arg9">字典参数 9 / Dictionary parameter 9</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型 / Type of dictionary parameter 9</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型 / Type of dictionary parameter 10</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <param name="arg9">字典参数 9 / Dictionary parameter 9</param>
        /// <param name="arg10">字典参数 10 / Dictionary parameter 10</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型 / Type of dictionary parameter 9</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型 / Type of dictionary parameter 10</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型 / Type of dictionary parameter 11</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <param name="arg9">字典参数 9 / Dictionary parameter 9</param>
        /// <param name="arg10">字典参数 10 / Dictionary parameter 10</param>
        /// <param name="arg11">字典参数 11 / Dictionary parameter 11</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型 / Type of dictionary parameter 9</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型 / Type of dictionary parameter 10</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型 / Type of dictionary parameter 11</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型 / Type of dictionary parameter 12</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <param name="arg9">字典参数 9 / Dictionary parameter 9</param>
        /// <param name="arg10">字典参数 10 / Dictionary parameter 10</param>
        /// <param name="arg11">字典参数 11 / Dictionary parameter 11</param>
        /// <param name="arg12">字典参数 12 / Dictionary parameter 12</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型 / Type of dictionary parameter 9</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型 / Type of dictionary parameter 10</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型 / Type of dictionary parameter 11</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型 / Type of dictionary parameter 12</typeparam>
        /// <typeparam name="T13">字典参数 13 的类型 / Type of dictionary parameter 13</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <param name="arg9">字典参数 9 / Dictionary parameter 9</param>
        /// <param name="arg10">字典参数 10 / Dictionary parameter 10</param>
        /// <param name="arg11">字典参数 11 / Dictionary parameter 11</param>
        /// <param name="arg12">字典参数 12 / Dictionary parameter 12</param>
        /// <param name="arg13">字典参数 13 / Dictionary parameter 13</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型 / Type of dictionary parameter 9</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型 / Type of dictionary parameter 10</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型 / Type of dictionary parameter 11</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型 / Type of dictionary parameter 12</typeparam>
        /// <typeparam name="T13">字典参数 13 的类型 / Type of dictionary parameter 13</typeparam>
        /// <typeparam name="T14">字典参数 14 的类型 / Type of dictionary parameter 14</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <param name="arg9">字典参数 9 / Dictionary parameter 9</param>
        /// <param name="arg10">字典参数 10 / Dictionary parameter 10</param>
        /// <param name="arg11">字典参数 11 / Dictionary parameter 11</param>
        /// <param name="arg12">字典参数 12 / Dictionary parameter 12</param>
        /// <param name="arg13">字典参数 13 / Dictionary parameter 13</param>
        /// <param name="arg14">字典参数 14 / Dictionary parameter 14</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型 / Type of dictionary parameter 9</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型 / Type of dictionary parameter 10</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型 / Type of dictionary parameter 11</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型 / Type of dictionary parameter 12</typeparam>
        /// <typeparam name="T13">字典参数 13 的类型 / Type of dictionary parameter 13</typeparam>
        /// <typeparam name="T14">字典参数 14 的类型 / Type of dictionary parameter 14</typeparam>
        /// <typeparam name="T15">字典参数 15 的类型 / Type of dictionary parameter 15</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <param name="arg9">字典参数 9 / Dictionary parameter 9</param>
        /// <param name="arg10">字典参数 10 / Dictionary parameter 10</param>
        /// <param name="arg11">字典参数 11 / Dictionary parameter 11</param>
        /// <param name="arg12">字典参数 12 / Dictionary parameter 12</param>
        /// <param name="arg13">字典参数 13 / Dictionary parameter 13</param>
        /// <param name="arg14">字典参数 14 / Dictionary parameter 14</param>
        /// <param name="arg15">字典参数 15 / Dictionary parameter 15</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <remarks>
        /// Gets the localized string by dictionary key.
        /// </remarks>
        /// <typeparam name="T1">字典参数 1 的类型 / Type of dictionary parameter 1</typeparam>
        /// <typeparam name="T2">字典参数 2 的类型 / Type of dictionary parameter 2</typeparam>
        /// <typeparam name="T3">字典参数 3 的类型 / Type of dictionary parameter 3</typeparam>
        /// <typeparam name="T4">字典参数 4 的类型 / Type of dictionary parameter 4</typeparam>
        /// <typeparam name="T5">字典参数 5 的类型 / Type of dictionary parameter 5</typeparam>
        /// <typeparam name="T6">字典参数 6 的类型 / Type of dictionary parameter 6</typeparam>
        /// <typeparam name="T7">字典参数 7 的类型 / Type of dictionary parameter 7</typeparam>
        /// <typeparam name="T8">字典参数 8 的类型 / Type of dictionary parameter 8</typeparam>
        /// <typeparam name="T9">字典参数 9 的类型 / Type of dictionary parameter 9</typeparam>
        /// <typeparam name="T10">字典参数 10 的类型 / Type of dictionary parameter 10</typeparam>
        /// <typeparam name="T11">字典参数 11 的类型 / Type of dictionary parameter 11</typeparam>
        /// <typeparam name="T12">字典参数 12 的类型 / Type of dictionary parameter 12</typeparam>
        /// <typeparam name="T13">字典参数 13 的类型 / Type of dictionary parameter 13</typeparam>
        /// <typeparam name="T14">字典参数 14 的类型 / Type of dictionary parameter 14</typeparam>
        /// <typeparam name="T15">字典参数 15 的类型 / Type of dictionary parameter 15</typeparam>
        /// <typeparam name="T16">字典参数 16 的类型 / Type of dictionary parameter 16</typeparam>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="arg1">字典参数 1 / Dictionary parameter 1</param>
        /// <param name="arg2">字典参数 2 / Dictionary parameter 2</param>
        /// <param name="arg3">字典参数 3 / Dictionary parameter 3</param>
        /// <param name="arg4">字典参数 4 / Dictionary parameter 4</param>
        /// <param name="arg5">字典参数 5 / Dictionary parameter 5</param>
        /// <param name="arg6">字典参数 6 / Dictionary parameter 6</param>
        /// <param name="arg7">字典参数 7 / Dictionary parameter 7</param>
        /// <param name="arg8">字典参数 8 / Dictionary parameter 8</param>
        /// <param name="arg9">字典参数 9 / Dictionary parameter 9</param>
        /// <param name="arg10">字典参数 10 / Dictionary parameter 10</param>
        /// <param name="arg11">字典参数 11 / Dictionary parameter 11</param>
        /// <param name="arg12">字典参数 12 / Dictionary parameter 12</param>
        /// <param name="arg13">字典参数 13 / Dictionary parameter 13</param>
        /// <param name="arg14">字典参数 14 / Dictionary parameter 14</param>
        /// <param name="arg15">字典参数 15 / Dictionary parameter 15</param>
        /// <param name="arg16">字典参数 16 / Dictionary parameter 16</param>
        /// <returns>要获取的字典内容字符串 / The localized string to get</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string key, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return m_LocalizationManager.GetString(key, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }

        /// <summary>
        /// 是否存在字典。
        /// </summary>
        /// <remarks>
        /// Whether the dictionary exists.
        /// </remarks>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <returns>是否存在字典 / Whether the dictionary exists</returns>
        [UnityEngine.Scripting.Preserve]
        public bool HasRawString(string key)
        {
            return m_LocalizationManager.HasRawString(key);
        }

        /// <summary>
        /// 根据字典主键获取字典值。
        /// </summary>
        /// <remarks>
        /// Gets the dictionary value by key.
        /// </remarks>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <returns>字典值 / Dictionary value</returns>
        [UnityEngine.Scripting.Preserve]
        public string GetRawString(string key)
        {
            return m_LocalizationManager.GetRawString(key);
        }

        /// <summary>
        /// 添加字典。
        /// </summary>
        /// <remarks>
        /// Adds a dictionary entry.
        /// </remarks>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <param name="value">字典值 / Dictionary value</param>
        /// <returns>是否添加字典成功 / Whether the dictionary was added successfully</returns>
        [UnityEngine.Scripting.Preserve]
        public bool AddRawString(string key, string value)
        {
            return m_LocalizationManager.AddRawString(key, value);
        }

        /// <summary>
        /// 移除字典。
        /// </summary>
        /// <remarks>
        /// Removes a dictionary entry.
        /// </remarks>
        /// <param name="key">字典主键 / Dictionary key</param>
        /// <returns>是否移除字典成功 / Whether the dictionary was removed successfully</returns>
        [UnityEngine.Scripting.Preserve]
        public bool RemoveRawString(string key)
        {
            return m_LocalizationManager.RemoveRawString(key);
        }

        /// <summary>
        /// 清空所有字典。
        /// </summary>
        /// <remarks>
        /// Removes all dictionary entries.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void RemoveAllRawStrings()
        {
            m_LocalizationManager.RemoveAllRawStrings();
        }
    }
}
