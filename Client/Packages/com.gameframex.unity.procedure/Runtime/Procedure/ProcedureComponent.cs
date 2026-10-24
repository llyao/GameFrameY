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

using GameFrameX.Procedure;
using System;
using System.Collections;
using GameFrameX.Fsm.Runtime;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.Procedure.Runtime
{
    /// <summary>
    /// 流程组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("GameFrameX/Procedure")]
    [GameFrameXAutoComponent(-1000)]
    public sealed class ProcedureComponent : GameFrameworkComponent
    {
        private IProcedureManager m_ProcedureManager = null;
        private ProcedureBase m_EntranceProcedure = null;

        [SerializeField] private string[] m_AvailableProcedureTypeNames = null;

        private string m_EntranceProcedureTypeName = null;

        /// <summary>
        /// 是否使用 StartupRunner 接管启动流程。
        /// </summary>
        /// <remarks>
        /// 为 true 时，本组件仅在 Awake 中注册 IProcedureManager，
        /// 不调用 Initialize 和 StartProcedure；由 ApplicationStartupEntry 通过
        /// StartupRunner.Run 接管整个流程的初始化与启动。
        /// 为 false 时（默认），使用 Inspector 配置的 m_AvailableProcedureTypeNames
        /// 启动流程（旧行为）。
        /// </remarks>
        private bool m_UseStartupRunner = false;

        /// <summary>
        /// 获取当前流程。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public IProcedureManager Procedure
        {
            get { return m_ProcedureManager; }
        }

        /// <summary>
        /// 获取当前流程。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public ProcedureBase CurrentProcedure
        {
            get { return m_ProcedureManager.CurrentProcedure; }
        }

        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public float CurrentProcedureTime
        {
            get { return m_ProcedureManager.CurrentProcedureTime; }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            ImplementationComponentType = Utility.Assembly.GetType(componentType);
            InterfaceComponentType = typeof(IProcedureManager);
            base.Awake();
            if (!IsRuntimeComponentReady)
            {
                return;
            }

            m_ProcedureManager = GameFrameworkEntry.GetModule<IProcedureManager>();
            if (m_ProcedureManager == null)
            {
                Log.Fatal("Procedure manager is invalid.");
                return;
            }

            if (GameFrameXOptions.TryGet<ProcedureOptions>(out var procedureOptions))
            {
                m_EntranceProcedureTypeName = procedureOptions.EntranceProcedureTypeName;
                m_UseStartupRunner = procedureOptions.UseStartupRunner;
            }
        }

        private IEnumerator Start()
        {
            if (m_ProcedureManager == null)
            {
                yield break;
            }

            if (m_UseStartupRunner)
            {
                // 由 ApplicationStartupEntry + StartupRunner.Run 接管 Initialize 和 StartProcedure，
                // 本组件仅承担 Awake 注册 IProcedureManager 的职责，避免与新入口重复 Initialize 报 Already exist FSM。
                yield break;
            }

            if (m_AvailableProcedureTypeNames == null || m_AvailableProcedureTypeNames.Length <= 0)
            {
                Log.Warning("Available procedure type names is empty. Procedure auto start skipped.");
                yield break;
            }

            ProcedureBase[] procedures = new ProcedureBase[m_AvailableProcedureTypeNames.Length];
            for (int i = 0; i < m_AvailableProcedureTypeNames.Length; i++)
            {
                Type procedureType = Utility.Assembly.GetType(m_AvailableProcedureTypeNames[i]);
                if (procedureType == null)
                {
                    Log.Error("Can not find procedure type '{0}'.", m_AvailableProcedureTypeNames[i]);
                    yield break;
                }

                procedures[i] = (ProcedureBase) Activator.CreateInstance(procedureType);
                if (procedures[i] == null)
                {
                    Log.Error("Can not create procedure instance '{0}'.", m_AvailableProcedureTypeNames[i]);
                    yield break;
                }

                if (m_EntranceProcedureTypeName == m_AvailableProcedureTypeNames[i])
                {
                    m_EntranceProcedure = procedures[i];
                }
            }

            if (m_EntranceProcedure == null)
            {
                Log.Error("Entrance procedure is invalid.");
                yield break;
            }

            m_ProcedureManager.Initialize(GameFrameworkEntry.GetModule<IFsmManager>(), procedures);

            yield return new WaitForEndOfFrame();

            m_ProcedureManager.StartProcedure(m_EntranceProcedure.GetType());
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        [UnityEngine.Scripting.Preserve]
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            return m_ProcedureManager.HasProcedure<T>();
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <param name="procedureType">要检查的流程类型。</param>
        /// <returns>是否存在流程。</returns>
        [UnityEngine.Scripting.Preserve]
        public bool HasProcedure(Type procedureType)
        {
            return m_ProcedureManager.HasProcedure(procedureType);
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        [UnityEngine.Scripting.Preserve]
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            return m_ProcedureManager.GetProcedure<T>();
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <param name="procedureType">要获取的流程类型。</param>
        /// <returns>要获取的流程。</returns>
        [UnityEngine.Scripting.Preserve]
        public ProcedureBase GetProcedure(Type procedureType)
        {
            return m_ProcedureManager.GetProcedure(procedureType);
        }

        /// <summary>
        /// 销毁当前流程状态机，清空所有已注册的流程。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void DestroyProcedures()
        {
            m_ProcedureManager.DestroyProcedures();
        }

        /// <summary>
        /// 销毁当前流程状态机，并使用新的流程重新初始化。
        /// </summary>
        /// <param name="procedures">新注册的流程。</param>
        /// <param name="entranceProcedure">入口流程。</param>
        [UnityEngine.Scripting.Preserve]
        public void ReinitializeProcedures(ProcedureBase[] procedures, ProcedureBase entranceProcedure)
        {
            m_ProcedureManager.ReinitializeProcedures(procedures);
            m_EntranceProcedure = entranceProcedure;
        }
    }
}
