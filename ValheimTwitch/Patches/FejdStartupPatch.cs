using HarmonyLib;
using ValheimTwitch.GUI;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class StartPatch
    {
        private static ValeimTwitchStartup startup;

        public static void Postfix(FejdStartup __instance)
        {
            var mainGui = __instance.transform.parent.gameObject;
            startup = mainGui.AddComponent<ValeimTwitchStartup>();

            UpdateMainButonText();

            startup.startGui.mainButton.button.onClick.AddListener(OnMainButtonClick);
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

