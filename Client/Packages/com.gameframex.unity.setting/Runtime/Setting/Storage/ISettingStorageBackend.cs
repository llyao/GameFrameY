namespace GameFrameX.Setting.Runtime
{
    /// <summary>
    /// 配置存储后端的公开契约：以键值对形式读写原始设置数据。
    /// </summary>
    /// <remarks>
    /// Public key-value storage backend contract used by setting helpers. The default implementation is Unity PlayerPrefs; mini-game channel packages provide platform SDK implementations and register them via <see cref="SettingStorageBackendFactory.RegisterMiniGameBackend"/>.
    /// </remarks>
    public interface ISettingStorageBackend
    {
        /// <summary>
        /// 将待写入数据持久化到底层存储。
        /// </summary>
        bool Save();

        /// <summary>
        /// 检查指定键是否存在。
        /// </summary>
        bool HasKey(string key);

        /// <summary>
        /// 删除指定键。键不存在时返回 false。
        /// </summary>
        bool DeleteKey(string key);

        /// <summary>
        /// 清空全部键值数据。
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// 读取整数键值，键不存在时返回默认值。
        /// </summary>
        int GetInt(string key, int defaultValue);

        /// <summary>
        /// 写入整数键值。
        /// </summary>
        void SetInt(string key, int value);

        /// <summary>
        /// 读取浮点数键值，键不存在时返回默认值。
        /// </summary>
        float GetFloat(string key, float defaultValue);

        /// <summary>
        /// 写入浮点数键值。
        /// </summary>
        void SetFloat(string key, float value);

        /// <summary>
        /// 读取字符串键值，键不存在时返回默认值。
        /// </summary>
        string GetString(string key, string defaultValue);

        /// <summary>
        /// 写入字符串键值。
        /// </summary>
        void SetString(string key, string value);
    }
}
