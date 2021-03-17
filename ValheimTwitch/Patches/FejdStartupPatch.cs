using HarmonyLib;
using ValheimTwitch.GUI;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class StartPatch
    {
        public static void Postfix(FejdStartup __instance)
        {
            var mainGui = __instance.transform.parent.gameObject;
            var gui = mainGui.AddComponent<ValeimTwitchStartup>();
        }
    }
}

