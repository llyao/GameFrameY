using System;
using GameFrameX.Runtime;
using GameFrameX.Scene.Runtime;
using NUnit.Framework;

namespace GameFrameX.Scene.Tests
{
    internal class UnitTests
    {
        // Here is an example of a unit test for the IsUnixSameDay method
        [Test]
        public void TestIsUnixSameDay()
        {
            // Arrange
            // long timestamp1 = 1617842400; // April 7, 2021 12:00:00 AM UTC
            // long timestamp2 = 1617896400; // April 7, 2021 12:00:00 PM UTC

            // Act
        }


        [Test]
        public void Test1()
        {
            var dateTime = new DateTime(2026, 5, 30, 10, 0, 0);
            var dateTime1 = dateTime.AddHours(1);

            Assert.That(dateTime1.Year, Is.EqualTo(dateTime.Year));
            Assert.That(dateTime1.Month, Is.EqualTo(dateTime.Month));
            Assert.That(dateTime1.Day, Is.EqualTo(dateTime.Day));
        }
    }

    internal class SceneOptionsTests
    {
        [TearDown]
        public void TearDown()
        {
            GameFrameXOptions.Reset();
        }

        [Test]
        public void SceneOptions_ConfigureAndTryGet_RoundTripsDifferentialValues()
        {
            GameFrameXOptions.Reset();
            GameFrameXOptions.Configure(new SceneOptions { EnableLoadSceneUpdateEvent = false, EnableLoadSceneDependencyAssetEvent = false });

            // 组件 Awake 路径在 EditMode 测试中不可达，退化为直接验证注册表消费语义（TryGet 即组件 Awake 的读取入口）
            bool hit = GameFrameXOptions.TryGet<SceneOptions>(out var sceneOptions);

            Assert.IsTrue(hit, "Configure 后 TryGet 应命中");
            Assert.IsFalse(sceneOptions.EnableLoadSceneUpdateEvent, "差异值应到达消费侧");
            Assert.IsFalse(sceneOptions.EnableLoadSceneDependencyAssetEvent, "差异值应到达消费侧");
        }
    }
}