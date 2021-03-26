using HarmonyLib;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(Game), "OnApplicationQuit")]
    public static class GameOnApplicationQuitPatch
    {
        public static void Prefix()
        {
            Plugin.Instance.ToggleRewards(false);
        }
    }
}

