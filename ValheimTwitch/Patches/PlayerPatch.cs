using HarmonyLib;
using System.Collections.Generic;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(Player), "Load")]
    public static class PlayerLoadPatch
    {
        public static void Postfix(ref HashSet<string> ___m_shownTutorials)
        {
            // Remove old custom tutorial messages
            int count = ___m_shownTutorials.Count;

            ___m_shownTutorials.RemoveWhere(hash => hash.StartsWith(Plugin.GUID));

            UnityEngine.Debug.Log($"Removed {count - ___m_shownTutorials.Count} messages");
        }
    }

    [HarmonyPatch(typeof(Player), "OnSpawned")]
    public static class PlayerOnSpawnedPatch
    {
        public static void Postfix(ref HashSet<string> ___m_shownTutorials)
        {
            if (Plugin.Instance.twitchClient != null && Plugin.Instance.twitchClient.IsFollowing())
                return;

            RavenPatch.Message($"Welcome to {Plugin.NAME}", "Follow me on twitch.tv/skarab42 to get rid of this message ;)", false);
        }
    }

}
