using HarmonyLib;
using System.Collections.Generic;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(Character), "Awake")]
    public static class CharacterAwakePatch
    {
        public static List<Character> tamedCharacters = new List<Character>();

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
            {
                //Log.Info($"Add tamed character -> {customName}");

                tamedCharacters.Add(__instance);
                Prefab.SetTameable(___m_nview, __instance.gameObject);
                __instance.m_name = Prefab.GetTamedName(customName, true);
            }
        }
    }

    [HarmonyPatch(typeof(Character), "OnDestroy")]
    public static class CharacterOnDestroyPatch
    {
        public static void Postfix(ref Character __instance, ref ZNetView ___m_name)
        {
            //Log.Info($"Remove tamed character -> {___m_name}");

            CharacterAwakePatch.tamedCharacters.Remove(__instance);
        }
    }
}

