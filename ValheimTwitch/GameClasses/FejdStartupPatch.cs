using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ValheimTwitch
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class FejdStartupPatch
    {
        public static void Postfix(FejdStartup __instance)
        {
            var pluginInstance = Plugin.Instance;
            var go = new GameObject($"{Plugin.LABEL}Info");
            var parent = __instance.m_versionLabel.transform.parent.gameObject;

            go.transform.SetParent(parent.transform);

            go.AddComponent<CanvasRenderer>();
            go.transform.localPosition = new Vector3(120, 0, 0);

            var rect = go.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(200, 80);
            rect.anchorMax = new Vector2(0.0f, 0.5f); // top right
            rect.anchorMin = new Vector2(0.0f, 0.5f); // bottom left

            var image = go.AddComponent<Image>();
            var button = go.AddComponent<Button>();

            image.color = new Color32(140, 69, 247, 200);

            var goText = new GameObject($"{Plugin.LABEL}InfoText");

            goText.transform.SetParent(go.transform);
            goText.transform.localPosition = Vector3.zero;

            goText.AddComponent<CanvasRenderer>();
            var textRect = goText.AddComponent<RectTransform>();

            textRect.sizeDelta = rect.sizeDelta - new Vector2(20, 20);

            var text = goText.AddComponent<Text>();

            text.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.color = Color.white;

            var client = pluginInstance.twitchClient;

            if (client == null || client.user == null)
            {
                button.onClick.AddListener(OnButtonClick);
                text.text = $"{Plugin.NAME}\nLogin";
            }
            else
            {
                text.text = $"{Plugin.NAME}\n{client.user.DisplayName}";
            }
        }

        private static void OnButtonClick()
        {
            Log.Info("clicked button");
        }
    }
}

