using HarmonyLib;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(Character), "Awake")]
    public static class CharacterAwakePatch
    {
        public static void Postfix(ref Character __instance, ref ZNetView ___m_nview)
        {
            var zdo = ___m_nview.GetZDO();

            if (zdo == null)
                return;

            var customName = zdo.GetString($"{Plugin.GUID}-name");
            var customTamed = zdo.GetBool($"{Plugin.GUID}-tamed");

            if (customName.Length > 0)
                __instance.m_name = customName;
            
            if (customTamed)
                Prefab.SetTameable(___m_nview, __instance.gameObject);
        }
    }
}

