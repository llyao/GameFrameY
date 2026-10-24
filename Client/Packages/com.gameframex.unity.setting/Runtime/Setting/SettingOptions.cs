// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and other related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and other international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   complete license text please see the LICENSE file in the source code root directory.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe any legal rights and interests of others as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   shall be borne solely by the developer of the project; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB 仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.Setting.Runtime
{
    /// <summary>
    /// 配置模块启动配置（模块级 Options）。
    /// </summary>
    /// <remarks>
    /// 默认值即开箱值（与原 SettingComponent 序列化字段默认值同源）；游戏在首场景加载前（BeforeSceneLoad）经 <c>GameFrameXOptions.Configure(new SettingOptions{...})</c> 注入差异值，
    /// 由 <see cref="SettingComponent"/> 在 Awake 中一次性消费。
    /// </remarks>
    public sealed class SettingOptions
    {
        /// <summary>
        /// 配置辅助器工厂（默认 PlayerPrefs 实现）。
        /// </summary>
        /// <remarks>启动期型：仅在组件 Awake 创建辅助器时消费，运行期不可修改。</remarks>
        [GameFrameXStartupOnly]
        public Func<SettingHelperBase> SettingHelperFactory = () => new GameObject().AddComponent<PlayerPrefsSettingHelper>();
    }
}
