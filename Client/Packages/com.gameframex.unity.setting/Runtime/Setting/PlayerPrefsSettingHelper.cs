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

using System;
using System.Collections.Generic;
using GameFrameX.Runtime;

namespace GameFrameX.Setting.Runtime
{
    /// <summary>
    /// PlayerPrefs 游戏配置辅助器，基于 Unity PlayerPrefs 实现游戏配置的读写操作。
    /// </summary>
    /// <remarks>
    /// PlayerPrefs game setting helper that implements game setting read/write operations based on Unity PlayerPrefs.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public class PlayerPrefsSettingHelper : SettingHelperBase
    {
        private ISettingStorageBackend m_StorageBackend;

        private ISettingStorageBackend StorageBackend
        {
            get
            {
                if (m_StorageBackend == null)
                {
                    m_StorageBackend = SettingStorageBackendFactory.Create();
                }

                return m_StorageBackend;
            }
        }

        internal void SetStorageBackendForTests(ISettingStorageBackend storageBackend)
        {
            m_StorageBackend = storageBackend ?? throw new GameFrameworkException("Storage backend is invalid.");
        }

        /// <summary>
        /// 获取游戏配置项数量。PlayerPrefs 不支持获取配置项数量，始终返回 -1。
        /// </summary>
        /// <remarks>
        /// Gets the number of game settings. PlayerPrefs does not support counting settings, always returns -1.
        /// </remarks>
        /// <value>始终返回 -1 / Always returns -1</value>
        [UnityEngine.Scripting.Preserve]
        public override int Count
        {
            get { return -1; }
        }

        /// <summary>
        /// 加载游戏配置。PlayerPrefs 不需要显式加载，始终返回成功。
        /// </summary>
        /// <remarks>
        /// Loads game settings. PlayerPrefs does not require explicit loading, always returns success.
        /// </remarks>
        /// <returns>始终返回 <c>true</c> / Always returns <c>true</c></returns>
        [UnityEngine.Scripting.Preserve]
        public override bool Load()
        {
            return true;
        }

        /// <summary>
        /// 保存游戏配置。
        /// </summary>
        /// <remarks>
        /// Saves game settings. Mini-game storage adapters may return success without an explicit flush when the platform SDK writes synchronously.
        /// </remarks>
        /// <returns>是否保存游戏配置成功 / Whether the settings were saved successfully</returns>
        [UnityEngine.Scripting.Preserve]
        public override bool Save()
        {
            return StorageBackend.Save();
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。PlayerPrefs 不支持此操作。
        /// </summary>
        /// <remarks>
        /// Gets all game setting names. PlayerPrefs does not support this operation.
        /// </remarks>
        /// <returns>始终返回 <c>null</c> / Always returns <c>null</c></returns>
        [UnityEngine.Scripting.Preserve]
        public override string[] GetAllSettingNames()
        {
            Log.Warning("GetAllSettingNames is not supported.");
            return null;
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。PlayerPrefs 不支持此操作。
        /// </summary>
        /// <remarks>
        /// Gets all game setting names. PlayerPrefs does not support this operation.
        /// </remarks>
        /// <param name="results">所有游戏配置项的名称列表 / List to store all game setting names</param>
        /// <exception cref="GameFrameworkException">当 <paramref name="results"/> 为 null 时抛出 / Thrown when <paramref name="results"/> is null</exception>
        [UnityEngine.Scripting.Preserve]
        public override void GetAllSettingNames(List<string> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            Log.Warning("GetAllSettingNames is not supported.");
        }

        /// <summary>
        /// 检查是否存在指定游戏配置项。
        /// </summary>
        /// <remarks>
        /// Checks whether the specified game setting exists.
        /// </remarks>
        /// <param name="settingName">要检查游戏配置项的名称 / Name of the game setting to check</param>
        /// <returns>指定的游戏配置项是否存在 / Whether the specified game setting exists</returns>
        [UnityEngine.Scripting.Preserve]
        public override bool HasSetting(string settingName)
        {
            return StorageBackend.HasKey(settingName);
        }

        /// <summary>
        /// 移除指定游戏配置项。
        /// </summary>
        /// <remarks>
        /// Removes the specified game setting.
        /// </remarks>
        /// <param name="settingName">要移除游戏配置项的名称 / Name of the game setting to remove</param>
        /// <returns>是否移除指定游戏配置项成功 / Whether the specified game setting was removed successfully</returns>
        [UnityEngine.Scripting.Preserve]
        public override bool RemoveSetting(string settingName)
        {
            return StorageBackend.DeleteKey(settingName);
        }

        /// <summary>
        /// 清空所有游戏配置项。
        /// </summary>
        /// <remarks>
        /// Removes all game settings.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public override void RemoveAllSettings()
        {
            StorageBackend.DeleteAll();
        }

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <remarks>
        /// Reads a boolean value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的布尔值 / The read boolean value</returns>
        [UnityEngine.Scripting.Preserve]
        public override bool GetBool(string settingName)
        {
            return StorageBackend.GetInt(settingName, 0) != 0;
        }

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <remarks>
        /// Reads a boolean value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值 / Default value returned when the specified setting does not exist</param>
        /// <returns>读取的布尔值 / The read boolean value</returns>
        [UnityEngine.Scripting.Preserve]
        public override bool GetBool(string settingName, bool defaultValue)
        {
            return StorageBackend.GetInt(settingName, defaultValue ? 1 : 0) != 0;
        }

        /// <summary>
        /// 向指定游戏配置项写入布尔值。
        /// </summary>
        /// <remarks>
        /// Writes a boolean value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的布尔值 / Boolean value to write</param>
        [UnityEngine.Scripting.Preserve]
        public override void SetBool(string settingName, bool value)
        {
            StorageBackend.SetInt(settingName, value ? 1 : 0);
        }

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <remarks>
        /// Reads an integer value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的整数值 / The read integer value</returns>
        [UnityEngine.Scripting.Preserve]
        public override int GetInt(string settingName)
        {
            return StorageBackend.GetInt(settingName, 0);
        }

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <remarks>
        /// Reads an integer value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值 / Default value returned when the specified setting does not exist</param>
        /// <returns>读取的整数值 / The read integer value</returns>
        [UnityEngine.Scripting.Preserve]
        public override int GetInt(string settingName, int defaultValue)
        {
            return StorageBackend.GetInt(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入整数值。
        /// </summary>
        /// <remarks>
        /// Writes an integer value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的整数值 / Integer value to write</param>
        [UnityEngine.Scripting.Preserve]
        public override void SetInt(string settingName, int value)
        {
            StorageBackend.SetInt(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <remarks>
        /// Reads a float value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的浮点数值 / The read float value</returns>
        [UnityEngine.Scripting.Preserve]
        public override float GetFloat(string settingName)
        {
            return StorageBackend.GetFloat(settingName, 0f);
        }

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <remarks>
        /// Reads a float value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值 / Default value returned when the specified setting does not exist</param>
        /// <returns>读取的浮点数值 / The read float value</returns>
        [UnityEngine.Scripting.Preserve]
        public override float GetFloat(string settingName, float defaultValue)
        {
            return StorageBackend.GetFloat(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入浮点数值。
        /// </summary>
        /// <remarks>
        /// Writes a float value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的浮点数值 / Float value to write</param>
        [UnityEngine.Scripting.Preserve]
        public override void SetFloat(string settingName, float value)
        {
            StorageBackend.SetFloat(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <remarks>
        /// Reads a string value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的字符串值 / The read string value</returns>
        [UnityEngine.Scripting.Preserve]
        public override string GetString(string settingName)
        {
            return StorageBackend.GetString(settingName, string.Empty);
        }

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <remarks>
        /// Reads a string value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值 / Default value returned when the specified setting does not exist</param>
        /// <returns>读取的字符串值 / The read string value</returns>
        [UnityEngine.Scripting.Preserve]
        public override string GetString(string settingName, string defaultValue)
        {
            return StorageBackend.GetString(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入字符串值。
        /// </summary>
        /// <remarks>
        /// Writes a string value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的字符串值 / String value to write</param>
        [UnityEngine.Scripting.Preserve]
        public override void SetString(string settingName, string value)
        {
            StorageBackend.SetString(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <remarks>
        /// Reads an object from the specified game setting.
        /// </remarks>
        /// <typeparam name="T">要读取对象的类型 / Type of the object to read</typeparam>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的对象 / The read object</returns>
        [UnityEngine.Scripting.Preserve]
        public override T GetObject<T>(string settingName)
        {
            return Utility.Json.ToObject<T>(GetString(settingName));
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <remarks>
        /// Reads an object from the specified game setting.
        /// </remarks>
        /// <param name="objectType">要读取对象的类型 / Type of the object to read</param>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的对象 / The read object</returns>
        [UnityEngine.Scripting.Preserve]
        public override object GetObject(Type objectType, string settingName)
        {
            return Utility.Json.ToObject(objectType, GetString(settingName));
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <remarks>
        /// Reads an object from the specified game setting.
        /// </remarks>
        /// <typeparam name="T">要读取对象的类型 / Type of the object to read</typeparam>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <param name="defaultObj">当指定的游戏配置项不存在时，返回此默认对象 / Default object returned when the specified setting does not exist</param>
        /// <returns>读取的对象 / The read object</returns>
        [UnityEngine.Scripting.Preserve]
        public override T GetObject<T>(string settingName, T defaultObj)
        {
            string json = GetString(settingName, null);
            if (json.IsNullOrWhiteSpace())
            {
                return defaultObj;
            }

            return Utility.Json.ToObject<T>(json);
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <remarks>
        /// Reads an object from the specified game setting.
        /// </remarks>
        /// <param name="objectType">要读取对象的类型 / Type of the object to read</param>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <param name="defaultObj">当指定的游戏配置项不存在时，返回此默认对象 / Default object returned when the specified setting does not exist</param>
        /// <returns>读取的对象 / The read object</returns>
        [UnityEngine.Scripting.Preserve]
        public override object GetObject(Type objectType, string settingName, object defaultObj)
        {
            string json = GetString(settingName, null);
            if (json.IsNullOrWhiteSpace())
            {
                return defaultObj;
            }

            return Utility.Json.ToObject(objectType, json);
        }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <remarks>
        /// Writes an object to the specified game setting.
        /// </remarks>
        /// <typeparam name="T">要写入对象的类型 / Type of the object to write</typeparam>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="obj">要写入的对象 / Object to write</param>
        [UnityEngine.Scripting.Preserve]
        public override void SetObject<T>(string settingName, T obj)
        {
            SetString(settingName, Utility.Json.ToJson(obj));
        }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <remarks>
        /// Writes an object to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="obj">要写入的对象 / Object to write</param>
        [UnityEngine.Scripting.Preserve]
        public override void SetObject(string settingName, object obj)
        {
            SetString(settingName, Utility.Json.ToJson(obj));
        }
    }
}
