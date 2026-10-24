using GameFrameX.Fsm.Runtime;
using GameFrameX.Runtime;
using NUnit.Framework;

namespace GameFrameX.FSM.Tests
{
    internal class UnitTests
    {
        private FsmManager m_FsmManager;

        [SetUp]
        public void Setup()
        {
            m_FsmManager = new FsmManager();
        }

        [TearDown]
        public void Teardown()
        {
            m_FsmManager = null;
        }

        [Test]
        public void TestCreateFsm()
        {
            var owner = new object();
            IFsm<object> fsm = m_FsmManager.CreateFsm(owner, new TestIdleState());
            Assert.IsNotNull(fsm);
            Assert.AreEqual(1, m_FsmManager.Count);
            Assert.IsFalse(fsm.IsRunning);
        }

        [Test]
        public void TestStartAndDestroyFsm()
        {
            var owner = new object();
            IFsm<object> fsm = m_FsmManager.CreateFsm(owner, new TestIdleState());
            fsm.Start<TestIdleState>();
            Assert.IsTrue(fsm.IsRunning);
            Assert.AreEqual(nameof(TestIdleState), fsm.CurrentState.GetType().Name);

            bool destroyed = m_FsmManager.DestroyFsm<object>();
            Assert.IsTrue(destroyed);
            Assert.AreEqual(0, m_FsmManager.Count);
        }

        [Test]
        public void TestHasFsm()
        {
            Assert.IsFalse(m_FsmManager.HasFsm<object>());

            var owner = new object();
            m_FsmManager.CreateFsm(owner, new TestIdleState());
            Assert.IsTrue(m_FsmManager.HasFsm<object>());
        }

        [Test]
        public void TestResetFsm()
        {
            var owner = new object();
            IFsm<object> fsm = m_FsmManager.CreateFsm(owner, new TestIdleState());
            fsm.Start<TestIdleState>();
            Assert.IsTrue(fsm.IsRunning);

            fsm.Reset();
            Assert.IsFalse(fsm.IsRunning);
            Assert.IsNull(fsm.CurrentState);
            Assert.AreEqual(0f, fsm.CurrentStateTime);
            Assert.IsFalse(fsm.IsDestroyed);
            Assert.AreEqual(1, fsm.FsmStateCount);

            fsm.Start<TestIdleState>();
            Assert.IsTrue(fsm.IsRunning);
            Assert.AreEqual(nameof(TestIdleState), fsm.CurrentState.GetType().Name);
        }

        [Test]
        public void TestAddState()
        {
            var owner = new object();
            IFsm<object> fsm = m_FsmManager.CreateFsm(owner, new TestIdleState());
            Assert.AreEqual(1, fsm.FsmStateCount);
            Assert.IsFalse(fsm.HasState<TestRunState>());

            fsm.AddState(new TestRunState());
            Assert.AreEqual(2, fsm.FsmStateCount);
            Assert.IsTrue(fsm.HasState<TestRunState>());

            fsm.Start<TestRunState>();
            Assert.AreEqual(nameof(TestRunState), fsm.CurrentState.GetType().Name);
        }

        [Test]
        public void TestRemoveState()
        {
            var owner = new object();
            IFsm<object> fsm = m_FsmManager.CreateFsm(owner, new TestIdleState(), new TestRunState());
            Assert.AreEqual(2, fsm.FsmStateCount);

            bool removed = fsm.RemoveState<TestRunState>();
            Assert.IsTrue(removed);
            Assert.AreEqual(1, fsm.FsmStateCount);
            Assert.IsFalse(fsm.HasState<TestRunState>());

            bool removedAgain = fsm.RemoveState<TestRunState>();
            Assert.IsFalse(removedAgain);
        }

        [Test]
        public void TestRemoveCurrentStateThrows()
        {
            var owner = new object();
            IFsm<object> fsm = m_FsmManager.CreateFsm(owner, new TestIdleState());
            fsm.Start<TestIdleState>();

            Assert.Throws<GameFrameworkException>(() => fsm.RemoveState<TestIdleState>());
        }

        [Test]
        public void TestAddDuplicateStateThrows()
        {
            var owner = new object();
            IFsm<object> fsm = m_FsmManager.CreateFsm(owner, new TestIdleState());

            Assert.Throws<GameFrameworkException>(() => fsm.AddState(new TestIdleState()));
        }

        private class TestIdleState : FsmState<object>
        {
        }

        private class TestRunState : FsmState<object>
        {
        }
    }
}
