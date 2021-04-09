using HarmonyLib;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(Player), "Load")]
    public static class PlayerLoadPatch
    {
        public static AudioSource audioSource;
        public static AudioClip whistleClip;

        public static void Postfix(Player __instance, ref HashSet<string> ___m_shownTutorials)
        {
            // Remove old custom tutorial messages
            int count = ___m_shownTutorials.Count;

            ___m_shownTutorials.RemoveWhere(hash => hash.StartsWith(Plugin.GUID));

            UnityEngine.Debug.Log($"Removed {count - ___m_shownTutorials.Count} messages");

            audioSource = __instance.gameObject.AddComponent<AudioSource>();
            whistleClip = EmbeddedAsset.LoadAudioClip("Assets.calling-whistle.wav");

            Log.Info($"Audio time {audioSource.time}");
        }

        public static void Whistle(float volume = 0.5f)
        {
            audioSource.PlayOneShot(whistleClip, volume);
        }
    }

    [HarmonyPatch(typeof(Player), "OnSpawned")]
    public static class PlayerOnSpawnedPatch
    {
        public static void Postfix(ref HashSet<string> ___m_shownTutorials)
        {
            if (Plugin.Instance.isHuginIntroShown || (Plugin.Instance.twitchClient != null && Plugin.Instance.twitchClient.IsFollowing()))
                return;

            RavenPatch.Message($"Welcome to {Plugin.NAME}", "Follow me on twitch.tv/skarab42 to get rid of this message ;)", false);

            Plugin.Instance.isHuginIntroShown = true;
        }
    }

    [HarmonyPatch(typeof(Player), "Update")]
    public static class PlayerUpdatePatch
    {
        public static void Prefix(Player __instance)
        {
            if (CustomInput.GetKeyDown("ToggleAllRewards"))
            {
                new Thread(() =>
                {
                    Plugin.Instance.ToggleRewards(!Plugin.Instance.isRewardsEnabled);
                }).Start();
            }

            if (!Plugin.Instance.isInGame || !CustomInput.GetKeyDown("Whistle"))
                return;

            PlayerLoadPatch.Whistle(AudioMan.GetSFXVolume());

            foreach (var character in CharacterAwakePatch.tamedCharacters)
            {
                var znview = character.GetComponent<ZNetView>();

                if (znview == null)
                    continue;

                var zdo = znview.GetZDO();

                if (zdo == null)
                    continue;

                var customName = zdo.GetString($"{Plugin.GUID}-name");

                if (customName.Length == 0)
                    return;

                var ai = character.GetComponent<MonsterAI>();

                ai.SetFollowTarget(__instance.gameObject);
            }
        }
    }
}
