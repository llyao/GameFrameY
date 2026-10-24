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

using GameFrameX;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GameFrameX.Runtime;

namespace GameFrameX.Setting.Runtime
{
    /// <summary>
    /// 默认游戏配置。
    /// </summary>
    /// <remarks>
    /// Default game setting implementation.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public sealed class DefaultSetting
    {
        private readonly SortedDictionary<string, string> m_Settings = new SortedDictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// 获取游戏配置项数量。
        /// </summary>
        /// <remarks>
        /// Gets the number of game settings.
        /// </remarks>
        /// <value>游戏配置项数量 / Number of game settings</value>
        [UnityEngine.Scripting.Preserve]
        public int Count
        {
            get
            {
                return m_Settings.Count;
            }
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <remarks>
        /// Gets all game setting names.
        /// </remarks>
        /// <returns>所有游戏配置项的名称 / Array of all game setting names</returns>
        [UnityEngine.Scripting.Preserve]
        public string[] GetAllSettingNames()
        {
            int index = 0;
            string[] allSettingNames = new string[m_Settings.Count];
            foreach (var setting in m_Settings)
            {
                allSettingNames[index++] = setting.Key;
            }

            return allSettingNames;
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <remarks>
        /// Gets all game setting names.
        /// </remarks>
        /// <param name="results">所有游戏配置项的名称列表 / List to store all game setting names</param>
        /// <exception cref="GameFrameworkException">当 <paramref name="results"/> 为 null 时抛出 / Thrown when <paramref name="results"/> is null</exception>
        [UnityEngine.Scripting.Preserve]
        public void GetAllSettingNames(List<string> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (var setting in m_Settings)
            {
                results.Add(setting.Key);
            }
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
        public bool HasSetting(string settingName)
        {
            return m_Settings.ContainsKey(settingName);
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
        public bool RemoveSetting(string settingName)
        {
            return m_Settings.Remove(settingName);
        }

        /// <summary>
        /// 清空所有游戏配置项。
        /// </summary>
        /// <remarks>
        /// Removes all game settings.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void RemoveAllSettings()
        {
            m_Settings.Clear();
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
        public bool GetBool(string settingName)
        {
            if (!m_Settings.TryGetValue(settingName, out var value))
            {
                Log.Warning("Setting '{0}' is not exist.", settingName);
                return false;
            }

            if (!int.TryParse(value, out var intValue))
            {
                Log.Warning("Setting '{0}' value '{1}' is not a valid integer.", settingName, value);
                return false;
            }

            return intValue != 0;
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
        public bool GetBool(string settingName, bool defaultValue)
        {
            if (!m_Settings.TryGetValue(settingName, out var value))
            {
                return defaultValue;
            }

            if (!int.TryParse(value, out var intValue))
            {
                Log.Warning("Setting '{0}' value '{1}' is not a valid integer.", settingName, value);
                return defaultValue;
            }

            return intValue != 0;
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
        public void SetBool(string settingName, bool value)
        {
            m_Settings[settingName] = value ? "1" : "0";
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
        public int GetInt(string settingName)
        {
            if (!m_Settings.TryGetValue(settingName, out var value))
            {
                Log.Warning("Setting '{0}' is not exist.", settingName);
                return 0;
            }

            if (!int.TryParse(value, out var intValue))
            {
                Log.Warning("Setting '{0}' value '{1}' is not a valid integer.", settingName, value);
                return 0;
            }

            return intValue;
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
        public int GetInt(string settingName, int defaultValue)
        {
            if (!m_Settings.TryGetValue(settingName, out var value))
            {
                return defaultValue;
            }

            if (!int.TryParse(value, out var intValue))
            {
                Log.Warning("Setting '{0}' value '{1}' is not a valid integer.", settingName, value);
                return defaultValue;
            }

            return intValue;
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
        public void SetInt(string settingName, int value)
        {
            m_Settings[settingName] = value.ToString();
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
        public float GetFloat(string settingName)
        {
            if (!m_Settings.TryGetValue(settingName, out var value))
            {
                Log.Warning("Setting '{0}' is not exist.", settingName);
                return 0f;
            }

            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
            {
                Log.Warning("Setting '{0}' value '{1}' is not a valid float.", settingName, value);
                return 0f;
            }

            return floatValue;
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
        public float GetFloat(string settingName, float defaultValue)
        {
            if (!m_Settings.TryGetValue(settingName, out var value))
            {
                return defaultValue;
            }

            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
            {
                Log.Warning("Setting '{0}' value '{1}' is not a valid float.", settingName, value);
                return defaultValue;
            }

            return floatValue;
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
        public void SetFloat(string settingName, float value)
        {
            m_Settings[settingName] = value.ToString(CultureInfo.InvariantCulture);
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
        public string GetString(string settingName)
        {
            if (!m_Settings.TryGetValue(settingName, out var value))
            {
                Log.Warning("Setting '{0}' is not exist.", settingName);
                return null;
            }

            return value;
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
        public string GetString(string settingName, string defaultValue)
        {
            if (!m_Settings.TryGetValue(settingName, out var value))
            {
                return defaultValue;
            }

            return value;
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
        public void SetString(string settingName, string value)
        {
            m_Settings[settingName] = value;
        }

        /// <summary>
        /// 序列化数据。
        /// </summary>
        /// <remarks>
        /// Serializes the settings data to the specified stream.
        /// </remarks>
        /// <param name="stream">目标流 / Target stream to write serialized data</param>
        [UnityEngine.Scripting.Preserve]
        public void Serialize(Stream stream)
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8);
            binaryWriter.Write7BitEncodedInt32(m_Settings.Count);
            foreach (KeyValuePair<string, string> setting in m_Settings)
            {
                binaryWriter.Write(setting.Key);
                binaryWriter.Write(setting.Value);
            }
        }

        /// <summary>
        /// 反序列化数据。
        /// </summary>
        /// <remarks>
        /// Deserializes the settings data from the specified stream.
        /// </remarks>
        /// <param name="stream">指定流 / Source stream to read serialized data from</param>
        [UnityEngine.Scripting.Preserve]
        public void Deserialize(Stream stream)
        {
            m_Settings.Clear();
            BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8);
            int settingCount = binaryReader.Read7BitEncodedInt32();
            for (int i = 0; i < settingCount; i++)
            {
                m_Settings.Add(binaryReader.ReadString(), binaryReader.ReadString());
            }
        }
    }
}
