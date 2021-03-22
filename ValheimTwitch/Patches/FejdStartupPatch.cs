using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class FejdStartupStartPatch
    {
        public static GameObject gui;
        public static ValheimTwitchGUIScript guiScript;
        
        public static void Postfix(FejdStartup __instance)
        {
            var mainGui = __instance.transform.parent.gameObject;
            var bundle = EmbeddedAsset.LoadAssetBundle("Assets.valheimtwitchgui");
            var prefab = bundle.LoadAsset<GameObject>("Valheim Twitch GUI");

            gui = UnityEngine.Object.Instantiate(prefab);
            gui.transform.SetParent(mainGui.transform);

            guiScript = gui.GetComponent<ValheimTwitchGUIScript>();

            guiScript.mainButton.OnClick(() => OnMainButtonClick());
            guiScript.refreshRewardButton.OnClick(() => {
                Plugin.Instance.UpdateRwardsList();
                UpdateRewardGrid();
            });

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

                    var title = reward.Title;
                    var color = Colors.FromHex(reward.BackgroundColor);

                    if (title.Length > 25)
                    {
                        title = title.Substring(0, 25).TrimEnd() + ". . .";
                    }

                    var texture = TextureLoader.LoadFromURL((reward.Image ?? reward.DefaultImage).Url4x);

                    Log.Info($"Sprit -> {texture.width} x {texture.height}");

                    var item = guiScript.rewardGrid.Add(title, color, texture);
                    var button = item.GetComponent<Button>();

                    button.onClick.AddListener(() =>
                    {
                        //guiScript.ShowPanel("Reward Settings Panel");
                    });
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
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

