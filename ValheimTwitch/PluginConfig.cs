using UnityEngine;

namespace ValheimTwitch
{
    static class PluginConfig
    {
        private static string MakeKey(string group, string key)
        {
            return $"{Plugin.GUID}.{group}.{key}";
        }

        public static bool HasKey(string group, string key)
        {
            return PlayerPrefs.HasKey(MakeKey(group, key));
        }

        public static string GetString(string group, string key)
        {
            return PlayerPrefs.GetString(MakeKey(group, key));
        }

        public static void SetString(string group, string key, string value)
        {
            PlayerPrefs.SetString(MakeKey(group, key), value);
        }

        public static int GetInt(string group, string key)
        {
            return PlayerPrefs.GetInt(MakeKey(group, key));
        }

        public static void SetInt(string group, string key, int value)
        {
            PlayerPrefs.SetInt(MakeKey(group, key), value);
        }
    }
}
