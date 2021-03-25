using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ValheimTwitch
{
    static class RewardsConfig
    {
        private static Dictionary<string, JToken> rewards = new Dictionary<string, JToken>();

        public static void Load()
        {
            var json = PluginConfig.GetObject("rewards");

            if (json == null)
                return;

            foreach (var token in json)
            {
                rewards[token.Key] = token.Value;
            }
        }

        public static void Save()
        {
            PluginConfig.SetObject("rewards", rewards);
        }

        public static JToken Get(string key)
        {
            JToken data;
            rewards.TryGetValue(key, out data);
            return data;
        }

        public static void Set(string key, SettingsMessageData data, bool save = true)
        {
            rewards[key] = JToken.FromObject(data);

            if (save)
                Save();
        }
    }
}
