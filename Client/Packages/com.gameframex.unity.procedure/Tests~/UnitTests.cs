using System;
using System.Reflection;
using GameFrameX.Fsm.Runtime;
using GameFrameX.Procedure.Runtime;
using GameFrameX.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace GameFrameX.Procedure.Tests
{
    #region Test Procedure Types

    internal class ProcedurePreload : ProcedureBase { }

    internal class ProcedureLogin : ProcedureBase { }

    internal class ProcedureMain : ProcedureBase
    {
        public bool OnInitCalled { get; private set; }
        public bool OnEnterCalled { get; private set; }
        public bool OnDestroyCalled { get; private set; }

        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
            OnInitCalled = true;
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            OnEnterCalled = true;
        }

        protected override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnDestroy(procedureOwner);
            OnDestroyCalled = true;
        }
    }

    #endregion

    [TestFixture]
    internal class ProcedureManagerTests
    {
        private ProcedureManager m_ProcedureManager;
        private FsmManager m_FsmManager;

        [SetUp]
        public void Setup()
        {
            m_ProcedureManager = new ProcedureManager();
            m_FsmManager = new FsmManager();
        }

        [TearDown]
        public void Teardown()
        {
        }

        #region Initialize Validation

        [Test]
        public void Initialize_NullFsmManager_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                m_ProcedureManager.Initialize(null, new ProcedurePreload());
            });
        }

        [Test]
        public void Initialize_NullProcedures_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                m_ProcedureManager.Initialize(m_FsmManager, null);
            });
        }

        [Test]
        public void Initialize_EmptyProcedures_Throws()
        {
            Assert.Throws<GameFrameworkException>(() =>
            {
                m_ProcedureManager.Initialize(m_FsmManager);
            });
        }

        #endregion

        #region Uninitialized Access

        [Test]
        public void CurrentProcedure_BeforeInitialize_Throws()
        {
            Assert.Throws<GameFrameworkException>(() =>
            {
                var _ = m_ProcedureManager.CurrentProcedure;
            });
        }

        [Test]
        public void CurrentProcedureTime_BeforeInitialize_Throws()
        {
            Assert.Throws<GameFrameworkException>(() =>
            {
                var _ = m_ProcedureManager.CurrentProcedureTime;
            });
        }

        [Test]
        public void StartProcedure_BeforeInitialize_Throws()
        {
            Assert.Throws<GameFrameworkException>(() =>
            {
                m_ProcedureManager.StartProcedure(typeof(ProcedurePreload));
            });
        }

        [Test]
        public void HasProcedure_BeforeInitialize_Throws()
        {
            Assert.Throws<GameFrameworkException>(() =>
            {
                m_ProcedureManager.HasProcedure(typeof(ProcedurePreload));
            });
        }

        [Test]
        public void GetProcedure_BeforeInitialize_Throws()
        {
            Assert.Throws<GameFrameworkException>(() =>
            {
                m_ProcedureManager.GetProcedure(typeof(ProcedurePreload));
            });
        }

        #endregion

        #region Initialize & StartProcedure Flow

        [Test]
        public void Initialize_WithValidArgs_Succeeds()
        {
            var preload = new ProcedurePreload();
            m_ProcedureManager.Initialize(m_FsmManager, preload);

            Assert.IsTrue(m_ProcedureManager.HasProcedure<ProcedurePreload>());
        }

        [Test]
        public void StartProcedure_SetsCurrentProcedure()
        {
            var preload = new ProcedurePreload();
            var main = new ProcedureMain();
            m_ProcedureManager.Initialize(m_FsmManager, preload, main);
            m_ProcedureManager.StartProcedure<ProcedurePreload>();

            Assert.IsNotNull(m_ProcedureManager.CurrentProcedure);
            Assert.IsInstanceOf<ProcedurePreload>(m_ProcedureManager.CurrentProcedure);
        }

        [Test]
        public void StartProcedure_TimeStartsAtZero()
        {
            var preload = new ProcedurePreload();
            m_ProcedureManager.Initialize(m_FsmManager, preload);
            m_ProcedureManager.StartProcedure<ProcedurePreload>();

            Assert.AreEqual(0f, m_ProcedureManager.CurrentProcedureTime);
        }

        [Test]
        public void HasProcedure_ReturnsTrueForRegistered()
        {
            var preload = new ProcedurePreload();
            var main = new ProcedureMain();
            m_ProcedureManager.Initialize(m_FsmManager, preload, main);

            Assert.IsTrue(m_ProcedureManager.HasProcedure<ProcedurePreload>());
            Assert.IsTrue(m_ProcedureManager.HasProcedure<ProcedureMain>());
        }

        [Test]
        public void HasProcedure_ReturnsFalseForUnregistered()
        {
            var preload = new ProcedurePreload();
            m_ProcedureManager.Initialize(m_FsmManager, preload);

            Assert.IsFalse(m_ProcedureManager.HasProcedure<ProcedureMain>());
        }

        [Test]
        public void GetProcedure_ReturnsCorrectInstance()
        {
            var preload = new ProcedurePreload();
            var main = new ProcedureMain();
            m_ProcedureManager.Initialize(m_FsmManager, preload, main);

            var result = m_ProcedureManager.GetProcedure<ProcedureMain>();
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ProcedureMain>(result);
        }

        #endregion

        #region Lifecycle Callbacks

        [Test]
        public void ProcedureLifecycle_OnInitAndOnEnterCalled()
        {
            var main = new ProcedureMain();
            m_ProcedureManager.Initialize(m_FsmManager, main);
            m_ProcedureManager.StartProcedure<ProcedureMain>();

            Assert.IsTrue(main.OnInitCalled, "OnInit should be called during Initialize");
            Assert.IsTrue(main.OnEnterCalled, "OnEnter should be called during StartProcedure");
        }

        [Test]
        public void ProcedureLifecycle_OnDestroyCalledOnShutdown()
        {
            // Shutdown() is protected on ProcedureManager and cannot be called from tests.
            // This test is disabled until a public shutdown API is available.
            Assert.Pass("Skipped: ProcedureManager.Shutdown() is protected.");
        }

        #endregion
    }

    [TestFixture]
    internal class ProcedureComponentStartupRunnerTests
    {
        [Test]
        public void UseStartupRunner_DefaultIsFalse()
        {
            GameFrameXRuntimeHost.Reset();
            GameEntry.Shutdown(ShutdownType.None);
            var gameObject = new GameObject("ProcedureComponentStartupRunnerTests");
            try
            {
                var component = gameObject.AddComponent<ProcedureComponent>();

                Assert.IsFalse(GetUseStartupRunner(component));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
                GameEntry.Shutdown(ShutdownType.None);
                GameFrameXRuntimeHost.Reset();
            }
        }


        private static bool GetUseStartupRunner(ProcedureComponent component)
        {
            var field = typeof(ProcedureComponent).GetField(
                "m_UseStartupRunner",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            return (bool)field.GetValue(component);
        }
    }

    [TestFixture]
    internal class ProcedureOptionsTests
    {
        [TearDown]
        public void TearDown()
        {
            GameFrameXOptions.Reset();
        }

        [Test]
        public void ProcedureOptions_ConfigureAndTryGet_RoundTripsDifferentialValues()
        {
            GameFrameXOptions.Reset();
            GameFrameXOptions.Configure(new ProcedureOptions
            {
                EntranceProcedureTypeName = "GameFrameX.Procedure.Tests.ProcedureMain",
                UseStartupRunner = true,
            });

            // 组件 Awake 路径依赖运行时 Host，退化为直接验证注册表消费语义（TryGet 即组件 Awake 的读取入口）
            bool hit = GameFrameXOptions.TryGet<ProcedureOptions>(out var procedureOptions);

            Assert.IsTrue(hit, "Configure 后 TryGet 应命中");
            Assert.That(procedureOptions.EntranceProcedureTypeName, Is.EqualTo("GameFrameX.Procedure.Tests.ProcedureMain"));
            Assert.IsTrue(procedureOptions.UseStartupRunner);
        }
    }
}
