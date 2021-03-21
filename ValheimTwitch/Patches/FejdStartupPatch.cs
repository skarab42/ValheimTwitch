﻿using HarmonyLib;
using System;
using UnityEngine;
using ValheimTwitch.GUI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class FejdStartupStartPatch
    {
        public static GameObject gui;
        public static ValheimTwitchGUIScript guiScript;

        private static ValeimTwitchStartup startup;
        
        public static void Postfix(FejdStartup __instance)
        {
            var mainGui = __instance.transform.parent.gameObject;
            startup = mainGui.AddComponent<ValeimTwitchStartup>();

            UpdateMainButonText();

            startup.startGui.mainButton.button.onClick.AddListener(OnMainButtonClick);

            var bundle = EmbeddedAsset.LoadAssetBundle("Assets.valheimtwitchgui");

            if (bundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            var prefab = bundle.LoadAsset<GameObject>("Valheim Twitch GUI");

            if (!prefab)
            {
                Log.Info($"Prefab not found!!!");
                return;
            }

            gui = UnityEngine.Object.Instantiate(prefab);
            guiScript = gui.GetComponent<ValheimTwitchGUIScript>();

            guiScript.mainButton.onClick.AddListener(() =>
            {
                Log.Info("Prout prout prout !");
                guiScript.ToggleGUI();
            });

            UpdateRewardGrid();
        }

        private static void OnMainButtonClick()
        {
            if (Plugin.Instance.GetUser() == null)
                Plugin.Instance.TwitchAuth();
            else
                startup.startGui.mainPanel.ToggleActive();
        }

        public static void UpdateMainButonText()
        {
            var user = Plugin.Instance.GetUser();

            if (user == null)
            {
                startup.startGui.SetMainButtonText("Connexion");
            }
            else
            {
                startup.startGui.SetMainButtonText(user.DisplayName);
            }
        }

        private static void UpdateRewardGrid()
        {
            foreach (Twitch.API.Helix.Reward reward in Plugin.Instance.twitchRewards.Data)
            {
                try
                {
                    if (reward.IsEnabled == false)
                        continue;

                    Log.Info($"Reward: {reward.Title}");

                    var title = reward.Title;
                    var color = Colors.FromHex(reward.BackgroundColor);

                    if (title.Length > 25)
                    {
                        title = title.Substring(0, 25).TrimEnd() + ". . .";
                    }

                    var texture = TextureLoader.LoadFromURL((reward.Image ?? reward.DefaultImage).Url2x);

                    Log.Info($"Sprit -> {texture.width} x {texture.height}");

                    var item = guiScript.AddReward(title, color, texture);
                }
                catch (Exception)
                {
                    Log.Warning($"Reward image unavailable: {reward.Title}");
                }


                //item.button.onClick.AddListener(() => {
                //    Log.Info($"Clicked on {reward.Title}");

                //    selectedReward = reward;

                //    dropdown.SetPrefix(reward.Title);
                //    dropdown.Toggle();

                //    if (PluginConfig.HasKey("reward-actions", reward.Id))
                //    {
                //        var value = PluginConfig.GetInt("reward-actions", reward.Id);
                //        dropdown.SetLabel(Actions.GetActionName(value));
                //    }
                //    else
                //    {
                //        dropdown.SetLabel(Actions.GetActionName(Actions.Types.None));
                //    }
                //});

                //item.SetReward(reward);
            }
        }

        

        
    }
}

