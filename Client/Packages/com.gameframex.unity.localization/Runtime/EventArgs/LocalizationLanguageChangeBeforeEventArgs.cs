// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using GameFrameX.Event.Runtime;
using GameFrameX.Runtime;

namespace GameFrameX.Localization.Runtime
{
    /// <summary>
    /// 本地化语言改变前事件。
    /// </summary>
    /// <remarks>Event triggered before localization language changes.</remarks>
    [UnityEngine.Scripting.Preserve]
    public sealed class LocalizationLanguageChangeBeforeEventArgs : GameEventArgs
    {
        /// <summary>
        /// 本地化语言改变事件编号。
        /// </summary>
        /// <remarks>Event ID for localization language change.</remarks>
        [UnityEngine.Scripting.Preserve] public static readonly string EventId = typeof(LocalizationLanguageChangeBeforeEventArgs).FullName;

        /// <summary>
        /// 当前语言。
        /// </summary>
        /// <remarks>Current language.</remarks>
        [UnityEngine.Scripting.Preserve]
        public string Language { get; private set; }

        /// <summary>
        /// 未知本地化
        /// </summary>
        /// <remarks>Unknown localization.</remarks>
        const string UnknownLocalization = LocalizationManager.UnknownLocalization;

        /// <summary>
        /// 旧的语言。
        /// </summary>
        /// <remarks>Previous language.</remarks>
        [UnityEngine.Scripting.Preserve]
        public string OldLanguage { get; private set; }

        /// <summary>
        /// 初始化本地化语言改变前事件的新实例。
        /// </summary>
        /// <remarks>Initializes a new instance of the localization language change before event.</remarks>
        [UnityEngine.Scripting.Preserve]
        public LocalizationLanguageChangeBeforeEventArgs()
        {
            OldLanguage = UnknownLocalization;
            Language = UnknownLocalization;
        }

        /// <summary>
        /// 创建本地化语言改变前事件。
        /// </summary>
        /// <remarks>Creates a localization language change before event.</remarks>
        /// <param name="oldLanguage">旧的语言 / Previous language.</param>
        /// <param name="language">当前语言 / Current language.</param>
        /// <returns>创建的本地化语言改变前事件 / Created localization language change before event.</returns>
        [UnityEngine.Scripting.Preserve]
        public static LocalizationLanguageChangeBeforeEventArgs Create(string oldLanguage, string language)
        {
            var localizationLanguageChangeEventArgs = ReferencePool.Acquire<LocalizationLanguageChangeBeforeEventArgs>();
            localizationLanguageChangeEventArgs.OldLanguage = oldLanguage;
            localizationLanguageChangeEventArgs.Language = language;
            return localizationLanguageChangeEventArgs;
        }

        /// <summary>
        /// 清除事件参数。
        /// </summary>
        /// <remarks>Clears event parameters.</remarks>
        [UnityEngine.Scripting.Preserve]
        public override void Clear()
        {
            OldLanguage = UnknownLocalization;
            Language = UnknownLocalization;
        }

        /// <summary>
        /// 获取事件编号。
        /// </summary>
        /// <remarks>Gets the event ID.</remarks>
        /// <returns>事件编号 / Event ID.</returns>
        [UnityEngine.Scripting.Preserve]
        public override string Id
        {
            get { return EventId; }
        }
    }
}