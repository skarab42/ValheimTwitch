using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ValheimTwitch
{
    static class PluginConfig
    {
        private static string Key(string key)
        {
            return $"{Plugin.GUID}.{key}";
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(Key(key));
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(Key(key));
        }

        public static void SetObject(string key, object obj)
        {
            PlayerPrefs.SetString(Key(key), JsonConvert.SerializeObject(obj));
        }

        public static JObject GetObject(string key)
        {
            if (PlayerPrefs.HasKey(Key(key)))
            {
                return JObject.Parse(PlayerPrefs.GetString(Key(key)));
            }

            return null;
        }

        public static string GetString(string key)
        {
            return PlayerPrefs.GetString(Key(key));
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(Key(key), value);
        }
    }
}
