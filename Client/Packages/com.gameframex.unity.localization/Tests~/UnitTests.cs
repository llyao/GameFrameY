using System;
using GameFrameX.Localization.Runtime;
using GameFrameX.Runtime;
using NUnit.Framework;

namespace GameFrameX.Localization.Tests
{
    internal class UnitTests
    {
        private LocalizationManager _manager;

        [SetUp]
        public void Setup()
        {
            _manager = new LocalizationManager();
        }

        [TearDown]
        public void Teardown()
        {
            _manager.RemoveAllRawStrings();
            GameFrameXOptions.Reset(); // 清除 Options 注册，避免跨用例静态泄漏
        }

        [Test]
        public void GetRawString_ReturnsNull_WhenKeyIsNull()
        {
            Assert.IsNull(_manager.GetRawString(null));
        }

        [Test]
        public void GetRawString_ReturnsNull_WhenKeyIsEmpty()
        {
            Assert.IsNull(_manager.GetRawString(string.Empty));
        }

        [Test]
        public void GetRawString_ReturnsNull_WhenKeyNotFound()
        {
            Assert.IsNull(_manager.GetRawString("nonexistent_key"));
        }

        [Test]
        public void GetRawString_ReturnsValue_WhenKeyExists()
        {
            _manager.AddRawString("test_key", "test_value");
            Assert.AreEqual("test_value", _manager.GetRawString("test_key"));
        }

        [Test]
        public void GetString_ReturnsNoKey_WhenKeyNotFound()
        {
            var result = _manager.GetString("missing_key");
            Assert.IsTrue(result.StartsWith("<NoKey>"));
        }

        [Test]
        public void GetString_WithArgs_ReturnsNoKey_WhenKeyNotFound()
        {
            var result = _manager.GetString("missing_key", "arg1");
            Assert.IsTrue(result.StartsWith("<NoKey>"));
        }

        [Test]
        public void GetString_WithArgs_ReturnsError_WhenFormatMismatch()
        {
            _manager.AddRawString("bad_format", "{0} {1} {2} {3}");
            var result = _manager.GetString("bad_format", "only_one_arg");
            Assert.IsTrue(result.StartsWith("<Error>"));
        }

        [Test]
        public void AddRawString_ReturnsTrue_WhenKeyIsValid()
        {
            Assert.IsTrue(_manager.AddRawString("key1", "value1"));
        }

        [Test]
        public void AddRawString_ReturnsFalse_WhenKeyIsNull()
        {
            Assert.IsFalse(_manager.AddRawString(null, "value"));
        }

        [Test]
        public void AddRawString_ReturnsFalse_WhenKeyAlreadyExists()
        {
            _manager.AddRawString("key1", "value1");
            Assert.IsFalse(_manager.AddRawString("key1", "value2"));
        }

        [Test]
        public void HasRawString_ReturnsTrue_WhenKeyExists()
        {
            _manager.AddRawString("key1", "value1");
            Assert.IsTrue(_manager.HasRawString("key1"));
        }

        [Test]
        public void HasRawString_ReturnsFalse_WhenKeyDoesNotExist()
        {
            Assert.IsFalse(_manager.HasRawString("nonexistent"));
        }

        [Test]
        public void RemoveRawString_RemovesKey()
        {
            _manager.AddRawString("key1", "value1");
            Assert.IsTrue(_manager.RemoveRawString("key1"));
            Assert.IsNull(_manager.GetRawString("key1"));
        }

        [Test]
        public void RemoveAllRawStrings_ClearsAll()
        {
            _manager.AddRawString("key1", "value1");
            _manager.AddRawString("key2", "value2");
            _manager.RemoveAllRawStrings();
            Assert.AreEqual(0, _manager.DictionaryCount);
        }

        [Test]
        public void LocalizationOptions_ConfigureAndTryGet_RoundTripsDiffs()
        {
            Func<LocalizationHelperBase> localizationHelperFactory = () => null;
            GameFrameXOptions.Configure(new LocalizationOptions { DefaultLanguage = "en_US", EditorLanguage = "ja_JP", EnableLoadDictionaryUpdateEvent = true, IsEnableEditorMode = true, LocalizationHelperFactory = localizationHelperFactory });

            Assert.IsTrue(GameFrameXOptions.TryGet<LocalizationOptions>(out var options));
            Assert.That(options.DefaultLanguage, Is.EqualTo("en_US"));
            Assert.That(options.EditorLanguage, Is.EqualTo("ja_JP"));
            Assert.That(options.EnableLoadDictionaryUpdateEvent, Is.True);
            Assert.That(options.IsEnableEditorMode, Is.True);
            Assert.That(options.LocalizationHelperFactory, Is.SameAs(localizationHelperFactory));

            GameFrameXOptions.Reset();
            var defaults = new LocalizationOptions();
            Assert.That(defaults.DefaultLanguage, Is.EqualTo("zh_CN"));
            Assert.That(defaults.EditorLanguage, Is.EqualTo("zh_CN"));
            Assert.That(defaults.EnableLoadDictionaryUpdateEvent, Is.False);
            Assert.That(defaults.IsEnableEditorMode, Is.False);
            var localizationHelper = defaults.LocalizationHelperFactory();
            Assert.That(localizationHelper, Is.InstanceOf<DefaultLocalizationHelper>());
            UnityEngine.Object.DestroyImmediate(localizationHelper);
        }
    }
}
