using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ValheimTwitch.Events;
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

            var actions = Actions.GetActionNames();
            var options = new List<string>(actions.Values);

            guiScript.rewardActionsDropdown.AddOptions(options);
            guiScript.rewardActionsDropdown.onValueChanged.AddListener(OnActionsDropdownChanged);

            UpdateMainButonText();
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

        private static void UpdateRewardGrid()
        {
            guiScript.rewardGrid.Clear();

            foreach (Twitch.API.Helix.Reward reward in Plugin.Instance.twitchRewards.Data)
            {
                try
                {
                    if (reward.IsEnabled == false)
                        continue;

                    Log.Info($"Reward: {reward.Title}");

                    var actionIndex = 0;
                    var actions = Actions.GetActionNames();

                    if (PluginConfig.HasKey("reward-actions", reward.Id))
                    {
                        var actionType = PluginConfig.GetInt("reward-actions", reward.Id);
                        actionIndex = actions.Keys.ToList().IndexOf(actionType);
                    }

                    var title = reward.Title;
                    var color = Colors.FromHex(reward.BackgroundColor);
                    var texture = TextureLoader.LoadFromURL((reward.Image ?? reward.DefaultImage).Url4x);
                    var rewardGridItem = new RewardGridItem(reward.Id, title, color, texture, actionIndex);

                    guiScript.rewardGrid.Add(rewardGridItem);
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    Log.Warning($"Reward image unavailable: {reward.Title}");
                }
            }
        }

        private static void OnActionsDropdownChanged(int index)
        {
            var actions = Actions.GetActionNames();
            var action = actions.ElementAt(index);
            var reward = guiScript.rewardSettings.reward;

            Log.Info($"Change -> {reward.title}");
            Log.Info($"Action -> {action}");

            reward.actionIndex = index;

            PluginConfig.SetInt("reward-actions", reward.id, action.Key);
        }
    }
}

