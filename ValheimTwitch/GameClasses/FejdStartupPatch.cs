using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class FejdStartupPatch
    {
        public static void Postfix(FejdStartup __instance)
        {
            var pluginInstance = Plugin.Instance;
            var parent = __instance.m_versionLabel.transform.parent.gameObject;
            var bepinGo = new GameObject($"{Plugin.LABEL}Info");

            bepinGo.transform.parent = parent.transform;

            bepinGo.AddComponent<CanvasRenderer>();
            bepinGo.transform.localPosition = Vector3.zero;

            bepinGo.transform.localPosition = new Vector3(0.0f, -100.0f, 0.0f);

            var rt = bepinGo.AddComponent<RectTransform>();

            rt.anchorMax = new Vector2(0.3f, 0.95f);  // top left
            rt.anchorMin = new Vector2(0.03f, 0.95f); // bottom rigth

            var text = bepinGo.AddComponent<Text>();
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 20);

            var user = pluginInstance.twitchClient.user;

            if (user == null)
            {
                text.text = $"Valheim Twitch\nPress F7 to log-in.";
            }
            else
            {
                text.text = $"Valheim Twitch: {user.DisplayName}";
            }

            text.color = Color.white;
            text.fontSize = 20;
        }
    }
}

