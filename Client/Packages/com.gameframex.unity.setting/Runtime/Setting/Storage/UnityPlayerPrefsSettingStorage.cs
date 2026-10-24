namespace GameFrameX.Setting.Runtime
{
    /// <summary>
    /// 基于 Unity PlayerPrefs 的默认配置存储后端。
    /// </summary>
    /// <remarks>
    /// Default setting storage backend based on Unity PlayerPrefs.
    /// </remarks>
    internal sealed class UnityPlayerPrefsSettingStorage : ISettingStorageBackend
    {
        public bool Save()
        {
            UnityEngine.PlayerPrefs.Save();
            return true;
        }

        public bool HasKey(string key)
        {
            return UnityEngine.PlayerPrefs.HasKey(key);
        }

        public bool DeleteKey(string key)
        {
            if (!UnityEngine.PlayerPrefs.HasKey(key))
            {
                return false;
            }

            UnityEngine.PlayerPrefs.DeleteKey(key);
            return true;
        }

        public void DeleteAll()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
        }

        public int GetInt(string key, int defaultValue)
        {
            return UnityEngine.PlayerPrefs.GetInt(key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            UnityEngine.PlayerPrefs.SetInt(key, value);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return UnityEngine.PlayerPrefs.GetFloat(key, defaultValue);
        }

        public void SetFloat(string key, float value)
        {
            UnityEngine.PlayerPrefs.SetFloat(key, value);
        }

        public string GetString(string key, string defaultValue)
        {
            return UnityEngine.PlayerPrefs.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            UnityEngine.PlayerPrefs.SetString(key, value);
        }
    }
}
