using HarmonyLib;
using System;
using UnityEngine;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class FejdStartupStartPatch
    {
        public static GameObject gui;
        public static AssetBundle guiBundle;
        public static ValheimTwitchGUIScript guiScript;

        public static void Postfix(FejdStartup __instance)
        {
            var mainGui = __instance.m_mainMenu;
            guiBundle = guiBundle ?? EmbeddedAsset.LoadAssetBundle("Assets.valheimtwitchgui");
            var prefab = guiBundle.LoadAsset<GameObject>("Valheim Twitch GUI");

            gui = UnityEngine.Object.Instantiate(prefab);
            gui.transform.SetParent(mainGui.transform);

            guiScript = gui.GetComponent<ValheimTwitchGUIScript>();

            guiScript.mainButton.OnClick(() => OnMainButtonClick());
            guiScript.refreshRewardButton.OnClick(() => {
                Plugin.Instance.UpdateRwardsList();
                UpdateRewardGrid();
            });

            guiScript.rewardSettings.OnSettingsChanged += OnRewardSettingschanged;

            UpdateMainButonText();
            UpdateRewardGrid();
        }

        private static void OnRewardSettingschanged(object sender, SettingsChangedArgs e)
        {
            var key = guiScript.rewardSettings.reward.id;

            if (e.Data is RavenMessageData)
            {
                RewardsConfig.Set(key, e.Data as RavenMessageData);
            }
            else if (e.Data is SpawnCreatureData)
            {
                RewardsConfig.Set(key, e.Data as SpawnCreatureData);
            }
            else if (e.Data is HUDMessageData)
            {
                RewardsConfig.Set(key, e.Data as HUDMessageData);
            }
            else
            {
                RewardsConfig.Set(key, e.Data);
            }

            UpdateRewardGrid();
        }

        private static void OnMainButtonClick()
        {
            if (Plugin.Instance.GetUser() == null)
                Plugin.Instance.TwitchAuth();
            else
                guiScript.mainPanel.ToggleActive();
        }

        public static void UpdateMainButonText()
        {
            var user = Plugin.Instance.GetUser();

            guiScript?.mainButton.SetText(user == null ? "Connexion" : user.DisplayName);
        }

        public static void UpdateRewardGrid()
        {
            guiScript?.rewardGrid.Clear();

            if (Plugin.Instance.twitchRewards == null)
            {
                return;
            }

            foreach (Twitch.API.Helix.Reward reward in Plugin.Instance.twitchRewards.Data)
            {
                try
                {
                    if (reward.IsEnabled == false)
                        continue;

                    Log.Info($"Reward: {reward.Title}");

                    var title = reward.Title;
                    var data = RewardsConfig.Get(reward.Id);
                    var color = Colors.FromHex(reward.BackgroundColor);
                    var texture = TextureLoader.LoadFromURL((reward.Image ?? reward.DefaultImage).Url4x);
                    var rewardGridItem = new RewardGridItem(reward.Id, title, color, texture, data);

                    guiScript.rewardGrid.Add(rewardGridItem);
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    Log.Warning($"Reward image unavailable: {reward.Title}");
                }
            }
        }
    }
}

