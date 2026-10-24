using System;
using System.Reflection;
using GameFrameX.Mono.Runtime;
using NUnit.Framework;

namespace GameFrameX.Mono.Tests
{
    [TestFixture]
    internal class MonoManagerTests
    {
        private MonoManager _manager;

        [SetUp]
        public void SetUp()
        {
            _manager = new MonoManager();
        }

        [TearDown]
        public void TearDown()
        {
            InvokeShutdown(_manager);
            _manager = null;
        }

        #region Null Argument Guard

        [Test]
        public void AddUpdateListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.AddUpdateListener(null));
        }

        [Test]
        public void RemoveUpdateListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.RemoveUpdateListener(null));
        }

        [Test]
        public void AddFixedUpdateListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.AddFixedUpdateListener(null));
        }

        [Test]
        public void RemoveFixedUpdateListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.RemoveFixedUpdateListener(null));
        }

        [Test]
        public void AddLateUpdateListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.AddLateUpdateListener(null));
        }

        [Test]
        public void RemoveLateUpdateListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.RemoveLateUpdateListener(null));
        }

        [Test]
        public void AddDestroyListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.AddDestroyListener(null));
        }

        [Test]
        public void RemoveDestroyListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.RemoveDestroyListener(null));
        }

        [Test]
        public void AddOnApplicationFocusListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.AddOnApplicationFocusListener(null));
        }

        [Test]
        public void RemoveOnApplicationFocusListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.RemoveOnApplicationFocusListener(null));
        }

        [Test]
        public void AddOnApplicationPauseListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.AddOnApplicationPauseListener(null));
        }

        [Test]
        public void RemoveOnApplicationPauseListener_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _manager.RemoveOnApplicationPauseListener(null));
        }

        #endregion

        #region Dispatch

        [Test]
        public void Update_DispatchesToListeners()
        {
            float receivedA = -1f;
            float receivedB = -1f;
            _manager.AddUpdateListener((a, b) =>
            {
                receivedA = a;
                receivedB = b;
            });

            ((IMonoManager)_manager).Update(1.5f, 2.5f);

            Assert.AreEqual(1.5f, receivedA);
            Assert.AreEqual(2.5f, receivedB);
        }

        [Test]
        public void FixedUpdate_DispatchesToListeners()
        {
            bool called = false;
            _manager.AddFixedUpdateListener((a, b) => { called = true; });

            _manager.FixedUpdate();

            Assert.IsTrue(called);
        }

        [Test]
        public void LateUpdate_DispatchesToListeners()
        {
            bool called = false;
            _manager.AddLateUpdateListener((a, b) => { called = true; });

            _manager.LateUpdate();

            Assert.IsTrue(called);
        }

        [Test]
        public void OnDestroy_DispatchesToListeners()
        {
            bool called = false;
            _manager.AddDestroyListener(() => { called = true; });

            _manager.OnDestroy();

            Assert.IsTrue(called);
        }

        [Test]
        public void OnApplicationFocus_DispatchesTrue()
        {
            bool? received = null;
            _manager.AddOnApplicationFocusListener(f => { received = f; });

            _manager.OnApplicationFocus(true);

            Assert.IsTrue(received);
        }

        [Test]
        public void OnApplicationFocus_DispatchesFalse()
        {
            bool? received = null;
            _manager.AddOnApplicationFocusListener(f => { received = f; });

            _manager.OnApplicationFocus(false);

            Assert.IsFalse(received);
        }

        [Test]
        public void OnApplicationPause_DispatchesTrue()
        {
            bool? received = null;
            _manager.AddOnApplicationPauseListener(p => { received = p; });

            _manager.OnApplicationPause(true);

            Assert.IsTrue(received);
        }

        [Test]
        public void OnApplicationPause_DispatchesFalse()
        {
            bool? received = null;
            _manager.AddOnApplicationPauseListener(p => { received = p; });

            _manager.OnApplicationPause(false);

            Assert.IsFalse(received);
        }

        #endregion

        #region Add/Remove Behavior

        [Test]
        public void RemoveUpdateListener_StopsDispatch()
        {
            int callCount = 0;
            Action<float, float> listener = (a, b) => callCount++;

            _manager.AddUpdateListener(listener);
            ((IMonoManager)_manager).Update(1f, 1f);

            _manager.RemoveUpdateListener(listener);
            ((IMonoManager)_manager).Update(1f, 1f);

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void RemoveNonExistentListener_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _manager.RemoveUpdateListener((a, b) => { }));
        }

        [Test]
        public void MultipleListeners_AllCalled()
        {
            int call1 = 0;
            int call2 = 0;
            _manager.AddUpdateListener((a, b) => call1++);
            _manager.AddUpdateListener((a, b) => call2++);

            ((IMonoManager)_manager).Update(1f, 1f);

            Assert.AreEqual(1, call1);
            Assert.AreEqual(1, call2);
        }

        [Test]
        public void ExceptionInListener_DoesNotStopOthers()
        {
            bool secondCalled = false;
            _manager.AddUpdateListener((a, b) => { throw new InvalidOperationException("test"); });
            _manager.AddUpdateListener((a, b) => { secondCalled = true; });

            ((IMonoManager)_manager).Update(1f, 1f);

            Assert.IsTrue(secondCalled);
        }

        #endregion

        #region Shutdown

        [Test]
        public void Shutdown_ClearsAllListeners()
        {
            bool called = false;
            _manager.AddUpdateListener((a, b) => { called = true; });
            _manager.AddDestroyListener(() => { called = true; });

            InvokeShutdown(_manager);

            ((IMonoManager)_manager).Update(1f, 1f);
            _manager.OnDestroy();

            Assert.IsFalse(called);
        }

        #endregion

        private static void InvokeShutdown(MonoManager manager)
        {
            if (manager == null)
            {
                return;
            }

            var method = typeof(MonoManager).GetMethod("Shutdown",
                BindingFlags.Instance | BindingFlags.Public);
            method?.Invoke(manager, null);
        }
    }

    [TestFixture]
    internal class OnApplicationFocusChangedEventArgsTests
    {
        [Test]
        public void Create_SetsIsFocus_True()
        {
            var args = OnApplicationFocusChangedEventArgs.Create(true);
            Assert.IsTrue(args.IsFocus);
        }

        [Test]
        public void Create_SetsIsFocus_False()
        {
            var args = OnApplicationFocusChangedEventArgs.Create(false);
            Assert.IsFalse(args.IsFocus);
        }

        [Test]
        public void Clear_ResetsIsFocus()
        {
            var args = OnApplicationFocusChangedEventArgs.Create(true);
            args.Clear();
            Assert.IsFalse(args.IsFocus);
        }

        [Test]
        public void Id_MatchesEventId()
        {
            var args = new OnApplicationFocusChangedEventArgs();
            Assert.AreEqual(OnApplicationFocusChangedEventArgs.EventId, args.Id);
        }
    }

    [TestFixture]
    internal class OnApplicationPauseChangedEventArgsTests
    {
        [Test]
        public void Create_SetsIsPause_True()
        {
            var args = OnApplicationPauseChangedEventArgs.Create(true);
            Assert.IsTrue(args.IsPause);
        }

        [Test]
        public void Create_SetsIsPause_False()
        {
            var args = OnApplicationPauseChangedEventArgs.Create(false);
            Assert.IsFalse(args.IsPause);
        }

        [Test]
        public void Clear_ResetsIsPause()
        {
            var args = OnApplicationPauseChangedEventArgs.Create(true);
            args.Clear();
            Assert.IsFalse(args.IsPause);
        }

        [Test]
        public void Id_MatchesEventId()
        {
            var args = new OnApplicationPauseChangedEventArgs();
            Assert.AreEqual(OnApplicationPauseChangedEventArgs.EventId, args.Id);
        }
    }
}
