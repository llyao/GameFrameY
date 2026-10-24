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
    /// 游戏配置管理器。
    /// </summary>
    /// <remarks>
    /// Game setting manager.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public sealed class SettingManager : IGameFrameworkModule, ISettingManager, IShutdownableModule
    {
        private ISettingHelper m_SettingHelper;

        private void EnsureSettingHelper()
        {
            if (m_SettingHelper == null)
            {
                throw new GameFrameworkException("Setting helper is invalid.");
            }
        }

        private void EnsureSettingName(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new GameFrameworkException("Setting name is invalid.");
            }
        }

        /// <summary>
        /// 初始化游戏配置管理器的新实例。
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of the game setting manager.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public SettingManager()
        {
            m_SettingHelper = null;
        }

        /// <summary>
        /// 获取游戏配置项数量。
        /// </summary>
        /// <remarks>
        /// Gets the number of game settings.
        /// </remarks>
        /// <value>游戏配置项数量 / Number of game settings</value>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public int Count
        {
            get
            {
                EnsureSettingHelper();

                return m_SettingHelper.Count;
            }
        }

        /// <summary>
        /// 关闭并清理游戏配置管理器。
        /// </summary>
        /// <remarks>
        /// Shuts down and cleans up the game setting manager.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void Shutdown()
        {
            // Shutdown 必须可安全重入：helper 未注入（启动期全量实例化后尚未接线）时直接返回，不抛异常。
            if (m_SettingHelper == null)
            {
                return;
            }

            Save();
        }

        /// <summary>
        /// 设置游戏配置辅助器。
        /// </summary>
        /// <remarks>
        /// Sets the game setting helper.
        /// </remarks>
        /// <param name="settingHelper">游戏配置辅助器 / Game setting helper</param>
        /// <exception cref="GameFrameworkException">当 <paramref name="settingHelper"/> 为 null 时抛出 / Thrown when <paramref name="settingHelper"/> is null</exception>
        [UnityEngine.Scripting.Preserve]
        public void SetSettingHelper(ISettingHelper settingHelper)
        {
            if (settingHelper == null)
            {
                throw new GameFrameworkException("Setting helper is invalid.");
            }

            m_SettingHelper = settingHelper;
        }

        /// <summary>
        /// 加载游戏配置。
        /// </summary>
        /// <remarks>
        /// Loads game settings.
        /// </remarks>
        /// <returns>是否加载游戏配置成功 / Whether the settings were loaded successfully</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public bool Load()
        {
            EnsureSettingHelper();

            return m_SettingHelper.Load();
        }

        /// <summary>
        /// 保存游戏配置。
        /// </summary>
        /// <remarks>
        /// Saves game settings.
        /// </remarks>
        /// <returns>是否保存游戏配置成功 / Whether the settings were saved successfully</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public bool Save()
        {
            EnsureSettingHelper();

            return m_SettingHelper.Save();
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <remarks>
        /// Gets all game setting names.
        /// </remarks>
        /// <returns>所有游戏配置项的名称 / Array of all game setting names</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public string[] GetAllSettingNames()
        {
            EnsureSettingHelper();

            return m_SettingHelper.GetAllSettingNames();
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <remarks>
        /// Gets all game setting names.
        /// </remarks>
        /// <param name="results">所有游戏配置项的名称列表 / List to store all game setting names</param>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public void GetAllSettingNames(List<string> results)
        {
            EnsureSettingHelper();

            m_SettingHelper.GetAllSettingNames(results);
        }

        /// <summary>
        /// 检查是否存在指定游戏配置项。
        /// </summary>
        /// <remarks>
        /// Checks whether the specified game setting exists.
        /// </remarks>
        /// <param name="settingName">要检查游戏配置项的名称 / Name of the game setting to check</param>
        /// <returns>指定的游戏配置项是否存在 / Whether the specified game setting exists</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public bool HasSetting(string settingName)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.HasSetting(settingName);
        }

        /// <summary>
        /// 移除指定游戏配置项。
        /// </summary>
        /// <remarks>
        /// Removes the specified game setting.
        /// </remarks>
        /// <param name="settingName">要移除游戏配置项的名称 / Name of the game setting to remove</param>
        /// <returns>是否移除指定游戏配置项成功 / Whether the specified game setting was removed successfully</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public bool RemoveSetting(string settingName)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.RemoveSetting(settingName);
        }

        /// <summary>
        /// 清空所有游戏配置项。
        /// </summary>
        /// <remarks>
        /// Removes all game settings.
        /// </remarks>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public void RemoveAllSettings()
        {
            EnsureSettingHelper();

            m_SettingHelper.RemoveAllSettings();
        }

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <remarks>
        /// Reads a boolean value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的布尔值 / The read boolean value</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public bool GetBool(string settingName)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetBool(settingName);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public bool GetBool(string settingName, bool defaultValue)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetBool(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入布尔值。
        /// </summary>
        /// <remarks>
        /// Writes a boolean value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的布尔值 / Boolean value to write</param>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public void SetBool(string settingName, bool value)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            m_SettingHelper.SetBool(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <remarks>
        /// Reads an integer value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的整数值 / The read integer value</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public int GetInt(string settingName)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetInt(settingName);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public int GetInt(string settingName, int defaultValue)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetInt(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入整数值。
        /// </summary>
        /// <remarks>
        /// Writes an integer value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的整数值 / Integer value to write</param>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public void SetInt(string settingName, int value)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            m_SettingHelper.SetInt(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <remarks>
        /// Reads a float value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的浮点数值 / The read float value</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public float GetFloat(string settingName)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetFloat(settingName);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public float GetFloat(string settingName, float defaultValue)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetFloat(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入浮点数值。
        /// </summary>
        /// <remarks>
        /// Writes a float value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的浮点数值 / Float value to write</param>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public void SetFloat(string settingName, float value)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            m_SettingHelper.SetFloat(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <remarks>
        /// Reads a string value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的字符串值 / The read string value</returns>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public string GetString(string settingName)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetString(settingName);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public string GetString(string settingName, string defaultValue)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetString(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入字符串值。
        /// </summary>
        /// <remarks>
        /// Writes a string value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的字符串值 / String value to write</param>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public void SetString(string settingName, string value)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            m_SettingHelper.SetString(settingName, value);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public T GetObject<T>(string settingName) where T : class, new()
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetObject<T>(settingName);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public object GetObject(Type objectType, string settingName)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            if (objectType == null)
            {
                throw new GameFrameworkException("Object type is invalid.");
            }

            return m_SettingHelper.GetObject(objectType, settingName);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public T GetObject<T>(string settingName, T defaultObj) where T : class, new()
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            return m_SettingHelper.GetObject(settingName, defaultObj);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public object GetObject(Type objectType, string settingName, object defaultObj)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            if (objectType == null)
            {
                throw new GameFrameworkException("Object type is invalid.");
            }

            return m_SettingHelper.GetObject(objectType, settingName, defaultObj);
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
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public void SetObject<T>(string settingName, T obj) where T : class, new()
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            m_SettingHelper.SetObject(settingName, obj);
        }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <remarks>
        /// Writes an object to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="obj">要写入的对象 / Object to write</param>
        /// <exception cref="GameFrameworkException">当游戏配置辅助器未设置时抛出 / Thrown when the setting helper is not set</exception>
        [UnityEngine.Scripting.Preserve]
        public void SetObject(string settingName, object obj)
        {
            EnsureSettingHelper();
            EnsureSettingName(settingName);

            m_SettingHelper.SetObject(settingName, obj);
        }
    }
}
