using BepInEx;
using HarmonyLib;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.GUI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class FejdStartupStartPatch
    {
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
            
            var gui = Object.Instantiate(prefab);
            var guiScript = gui.GetComponent<ValheimTwitchGUIScript>();

            guiScript.mainButton.onClick.AddListener(() =>
            {
                Log.Info("Prout prout prout !");
            });
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
    }
}

