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
        private static Button mainButton;
        private static Text mainButtonText;

        private static GameObject goMainButton;
        private static GameObject goSettingsUI;

        public static void Postfix(FejdStartup __instance)
        {
            var parent = __instance.m_versionLabel.transform.parent.gameObject;

            goMainButton = CreateMainButton(parent);
            goSettingsUI = CreateSettingsPanel(parent);
        }

        public static GameObject CreateMainButton(GameObject parent)
        {
            var go = new GameObject($"{Plugin.LABEL}MainButton");

            go.transform.SetParent(parent.transform);

            go.AddComponent<CanvasRenderer>();
            go.transform.localPosition = new Vector3(150, 0, 0);

            var rect = go.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(250, 100);
            rect.anchorMax = new Vector2(0.0f, 0.5f); // top right
            rect.anchorMin = new Vector2(0.0f, 0.5f); // bottom left

            var image = go.AddComponent<Image>();

            mainButton = go.AddComponent<Button>();

            Texture2D logoTexture = EmbeddedAsset.LoadTexture2D("Assets.TwitchLogo.png");
            var sprite = Sprite.Create(logoTexture, new Rect(0, 0, logoTexture.width, logoTexture.height), new Vector2(0.5f, 0.5f));

            image.sprite = sprite;

            var goText = new GameObject($"{Plugin.LABEL}MainButtonText");

            goText.transform.SetParent(go.transform);
            goText.transform.localPosition = Vector3.zero;

            goText.AddComponent<CanvasRenderer>();
            var textRect = goText.AddComponent<RectTransform>();

            textRect.sizeDelta = rect.sizeDelta - new Vector2(80, 30);
            textRect.transform.localPosition = new Vector3(30, -20, 0);

            mainButtonText = goText.AddComponent<Text>();

            mainButton.onClick.AddListener(OnButtonClick);
            
            mainButtonText.font = Font.CreateDynamicFontFromOSFont("Arial", 10);
            mainButtonText.alignment = TextAnchor.MiddleCenter;
            mainButtonText.resizeTextForBestFit = true;
            mainButtonText.color = Color.white;

            UpdateText();

            return go;
        }

        private static GameObject CreateSettingsPanel(GameObject parent)
        {
            var go = new GameObject($"{Plugin.LABEL}SettingsPanel");

            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;

            go.AddComponent<CanvasRenderer>();

            var rect = go.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(400, 400);
            rect.anchorMax = new Vector2(0.25f, 0.5f);
            rect.anchorMin = new Vector2(0.25f, 0.5f);

            var image = go.AddComponent<Image>();

            image.color = new Color32(0, 0, 0, 100);

            go.SetActive(false);

            return go;
        }

        public static void UpdateText()
        {
            var client = Plugin.Instance.twitchClient;

            if (client == null || client.user == null)
            {
                mainButtonText.text = "Connexion";
            }
            else
            {
                mainButtonText.text = client.user.DisplayName;
            }
        }

        private static void OnButtonClick()
        {
            var client = Plugin.Instance.twitchClient;

            if (client != null && client.user != null)
            {
                goSettingsUI.SetActive(!goSettingsUI.activeSelf);
                return;
            }

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

