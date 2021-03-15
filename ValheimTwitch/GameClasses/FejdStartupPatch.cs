using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Helpers;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch
{
    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class FejdStartupPatch
    {
        private static Text text;
        private static Button button;

        public static void Postfix(FejdStartup __instance)
        {
            var go = new GameObject($"{Plugin.LABEL}Info");
            var parent = __instance.m_versionLabel.transform.parent.gameObject;

            go.transform.SetParent(parent.transform);

            go.AddComponent<CanvasRenderer>();
            go.transform.localPosition = new Vector3(150, 0, 0);

            var rect = go.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(250, 100);
            rect.anchorMax = new Vector2(0.0f, 0.5f); // top right
            rect.anchorMin = new Vector2(0.0f, 0.5f); // bottom left

            var image = go.AddComponent<Image>();

            button = go.AddComponent<Button>();

            Texture2D logoTexture = EmbeddedAsset.LoadTexture2D("Assets.TwitchLogo.png");
            var sprite = Sprite.Create(logoTexture, new Rect(0, 0, logoTexture.width, logoTexture.height), new Vector2(0.5f, 0.5f));

            image.sprite = sprite;

            var goText = new GameObject($"{Plugin.LABEL}InfoText");

            goText.transform.SetParent(go.transform);
            goText.transform.localPosition = Vector3.zero;

            goText.AddComponent<CanvasRenderer>();
            var textRect = goText.AddComponent<RectTransform>();

            textRect.sizeDelta = rect.sizeDelta - new Vector2(80, 30);
            textRect.transform.localPosition = new Vector3(30, -20, 0);

            text = goText.AddComponent<Text>();

            text.font = Font.CreateDynamicFontFromOSFont("Arial", 10);
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.color = Color.white;

            UpdateText();
        }

        public static void UpdateText()
        {
            var client = Plugin.Instance.twitchClient;

            if (client == null || client.user == null)
            {
                text.text = "Connexion";
                button.onClick.AddListener(OnButtonClick);
            }
            else
            {
                text.text = client.user.DisplayName;
                button.onClick.RemoveListener(OnButtonClick);
            }
        }

        private static void OnButtonClick()
        {
            var provider = new Provider(
                Plugin.TWITCH_APP_CLIENT_ID,
                Plugin.TWITCH_REDIRECT_HOST,
                Plugin.TWITCH_REDIRECT_PORT,
                Plugin.TWITCH_SCOPES
            );

            provider.OnAuthToken += OnAuthToken;

            provider.GetCode();
        }

        private static void OnAuthToken(object sender, AuthTokenArgs e)
        {
            if (e.Error == null)
            {
                Plugin.Instance.OnAuthToken(e.Token);
            }
            else
            {
                Log.Error($"OnAuthToken: {e.Error}");
            }
        }
    }
}

