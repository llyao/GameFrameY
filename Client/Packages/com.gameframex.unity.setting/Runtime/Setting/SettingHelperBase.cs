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
using UnityEngine;

namespace GameFrameX.Setting.Runtime
{
    /// <summary>
    /// 游戏配置辅助器基类。
    /// </summary>
    /// <remarks>
    /// Base class for game setting helpers.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public abstract class SettingHelperBase : MonoBehaviour, ISettingHelper
    {
        /// <summary>
        /// 获取游戏配置项数量。
        /// </summary>
        /// <remarks>
        /// Gets the number of game settings.
        /// </remarks>
        /// <value>游戏配置项数量 / Number of game settings</value>
        [UnityEngine.Scripting.Preserve]
        public abstract int Count { get; }

        /// <summary>
        /// 加载游戏配置。
        /// </summary>
        /// <remarks>
        /// Loads game settings.
        /// </remarks>
        /// <returns>是否加载游戏配置成功 / Whether the settings were loaded successfully</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract bool Load();

        /// <summary>
        /// 保存游戏配置。
        /// </summary>
        /// <remarks>
        /// Saves game settings.
        /// </remarks>
        /// <returns>是否保存游戏配置成功 / Whether the settings were saved successfully</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract bool Save();

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <remarks>
        /// Gets all game setting names.
        /// </remarks>
        /// <returns>所有游戏配置项的名称 / Array of all game setting names</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract string[] GetAllSettingNames();

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <remarks>
        /// Gets all game setting names.
        /// </remarks>
        /// <param name="results">所有游戏配置项的名称列表 / List to store all game setting names</param>
        [UnityEngine.Scripting.Preserve]
        public abstract void GetAllSettingNames(List<string> results);

        /// <summary>
        /// 检查是否存在指定游戏配置项。
        /// </summary>
        /// <remarks>
        /// Checks whether the specified game setting exists.
        /// </remarks>
        /// <param name="settingName">要检查游戏配置项的名称 / Name of the game setting to check</param>
        /// <returns>指定的游戏配置项是否存在 / Whether the specified game setting exists</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract bool HasSetting(string settingName);

        /// <summary>
        /// 移除指定游戏配置项。
        /// </summary>
        /// <remarks>
        /// Removes the specified game setting.
        /// </remarks>
        /// <param name="settingName">要移除游戏配置项的名称 / Name of the game setting to remove</param>
        /// <returns>是否移除指定游戏配置项成功 / Whether the specified game setting was removed successfully</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract bool RemoveSetting(string settingName);

        /// <summary>
        /// 清空所有游戏配置项。
        /// </summary>
        /// <remarks>
        /// Removes all game settings.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public abstract void RemoveAllSettings();

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <remarks>
        /// Reads a boolean value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的布尔值 / The read boolean value</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract bool GetBool(string settingName);

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
        public abstract bool GetBool(string settingName, bool defaultValue);

        /// <summary>
        /// 向指定游戏配置项写入布尔值。
        /// </summary>
        /// <remarks>
        /// Writes a boolean value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的布尔值 / Boolean value to write</param>
        [UnityEngine.Scripting.Preserve]
        public abstract void SetBool(string settingName, bool value);

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <remarks>
        /// Reads an integer value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的整数值 / The read integer value</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract int GetInt(string settingName);

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
        public abstract int GetInt(string settingName, int defaultValue);

        /// <summary>
        /// 向指定游戏配置项写入整数值。
        /// </summary>
        /// <remarks>
        /// Writes an integer value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的整数值 / Integer value to write</param>
        [UnityEngine.Scripting.Preserve]
        public abstract void SetInt(string settingName, int value);

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <remarks>
        /// Reads a float value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的浮点数值 / The read float value</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract float GetFloat(string settingName);

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
        public abstract float GetFloat(string settingName, float defaultValue);

        /// <summary>
        /// 向指定游戏配置项写入浮点数值。
        /// </summary>
        /// <remarks>
        /// Writes a float value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的浮点数值 / Float value to write</param>
        [UnityEngine.Scripting.Preserve]
        public abstract void SetFloat(string settingName, float value);

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <remarks>
        /// Reads a string value from the specified game setting.
        /// </remarks>
        /// <param name="settingName">要获取游戏配置项的名称 / Name of the game setting to read</param>
        /// <returns>读取的字符串值 / The read string value</returns>
        [UnityEngine.Scripting.Preserve]
        public abstract string GetString(string settingName);

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
        public abstract string GetString(string settingName, string defaultValue);

        /// <summary>
        /// 向指定游戏配置项写入字符串值。
        /// </summary>
        /// <remarks>
        /// Writes a string value to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="value">要写入的字符串值 / String value to write</param>
        [UnityEngine.Scripting.Preserve]
        public abstract void SetString(string settingName, string value);

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
        public abstract T GetObject<T>(string settingName) where T : class, new();

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
        public abstract object GetObject(Type objectType, string settingName);

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
        public abstract T GetObject<T>(string settingName, T defaultObj) where T : class, new();

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
        public abstract object GetObject(Type objectType, string settingName, object defaultObj);

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
        public abstract void SetObject<T>(string settingName, T obj) where T : class, new();

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <remarks>
        /// Writes an object to the specified game setting.
        /// </remarks>
        /// <param name="settingName">要写入游戏配置项的名称 / Name of the game setting to write</param>
        /// <param name="obj">要写入的对象 / Object to write</param>
        [UnityEngine.Scripting.Preserve]
        public abstract void SetObject(string settingName, object obj);
    }
}
