using System.Text.RegularExpressions;
using GameFrameX.Runtime;
using GameFrameX.Localization.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFrameX.Localization.Tests
{
    // C9 启动期型参数拒绝用例：EditMode 下 AddComponent 不会执行 Awake，模拟“运行期”调用 setter
    internal class StartupOnlySetterRejectTests
    {
        [SetUp]
        public void SetUp()
        {
            // EditMode 下框架日志门面尚未接线，需先注入 helper 才能捕获 Log.Warning
            GameFrameworkLog.SetLogHelper(new DefaultLogHelper());
        }

        [TearDown]
        public void TearDown()
        {
            GameFrameworkLog.SetLogHelper(null); // 还原日志门面，避免跨用例泄漏
            GameFrameXOptions.Reset(); // 清除 Options 注册，避免跨用例静态泄漏
        }

        [Test]
        public void DefaultLanguage_Setter_AtRuntime_WarnsAndRejects()
        {
            var component = new GameObject().AddComponent<LocalizationComponent>();
            try
            {
                // 期望 setter 发出含参数名与拒绝语义的中文警告
                LogAssert.Expect(LogType.Warning, new Regex("DefaultLanguage[\\s\\S]*启动期参数[\\s\\S]*已被拒绝[\\s\\S]*GameFrameXOptions.Configure"));

                component.DefaultLanguage = "en_US";

                // 拒绝后私有字段应保持默认值，未被静默写入
                var field = typeof(LocalizationComponent).GetField("m_DefaultLanguage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.IsNotNull(field, "应存在私有字段 m_DefaultLanguage");
                Assert.AreEqual("zh_CN", field.GetValue(component), "DefaultLanguage 为启动期参数，运行期修改应被拒绝且字段保持默认值");
            }
            finally
            {
                Object.DestroyImmediate(component.gameObject);
            }
        }

        [Test]
        public void EditorLanguage_Setter_AtRuntime_WarnsAndRejects()
        {
            var component = new GameObject().AddComponent<LocalizationComponent>();
            try
            {
                // 期望 setter 发出含参数名与拒绝语义的中文警告
                LogAssert.Expect(LogType.Warning, new Regex("EditorLanguage[\\s\\S]*启动期参数[\\s\\S]*已被拒绝[\\s\\S]*GameFrameXOptions.Configure"));

                component.EditorLanguage = "en_US";

                // 拒绝后私有字段应保持默认值，未被静默写入
                var field = typeof(LocalizationComponent).GetField("m_EditorLanguage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.IsNotNull(field, "应存在私有字段 m_EditorLanguage");
                Assert.AreEqual("zh_CN", field.GetValue(component), "EditorLanguage 为启动期参数，运行期修改应被拒绝且字段保持默认值");
            }
            finally
            {
                Object.DestroyImmediate(component.gameObject);
            }
        }
    }
}
