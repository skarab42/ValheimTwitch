using HarmonyLib;
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
        private static Scrollbar scrollbar;
        private static GameObject goSettingsUI;

        private static readonly int gridCols = 3;
        private static readonly int gridWidth = 380;
        private static readonly int gridHeight = 380;
        private static readonly int lineHeight = 120;

        public static void Postfix(FejdStartup __instance)
        {
            var parent = __instance.m_versionLabel.transform.parent.gameObject;

            CreateMainButton(parent);

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

        private static GameObject CreateSlidingHandle(GameObject parent)
        {
            var go = new GameObject($"{Plugin.LABEL}SlidingHandle");

            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;

            var rect = go.AddComponent<RectTransform>();

            go.AddComponent<CanvasRenderer>();

            rect.offsetMin = new Vector2(-20, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(20, 0);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.anchorMin = new Vector2(1f, 0f);

            rect.SetParent(go.transform);

            var image = go.AddComponent<Image>();

            image.color = new Color32(234, 128, 2, 150);

            return go;
        }

        private static GameObject CreateSlidingArea(GameObject parent)
        {
            var go = new GameObject($"{Plugin.LABEL}SlidingArea");

            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;

            var rect = go.AddComponent<RectTransform>();

            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.anchorMin = new Vector2(1f, 0f);

            rect.SetParent(go.transform);

            //var image = go.AddComponent<Image>();
            //image.color = new Color32(0, 0, 255, 100);

            CreateSlidingHandle(go);

            return go;
        }

        private static GameObject CreateVerticalScrollbar(GameObject parent)
        {
            var go = new GameObject($"{Plugin.LABEL}VerticalScrollbar");

            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;

            var rect = go.AddComponent<RectTransform>();

            go.AddComponent<CanvasRenderer>();

            var image = go.AddComponent<Image>();

            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(20, 0);
            rect.sizeDelta = new Vector2(20, 0);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.anchorMin = new Vector2(1f, 0f);

            image.color = new Color32(0, 0, 0, 150);

            scrollbar = go.AddComponent<Scrollbar>();
            var slidingArea = CreateSlidingArea(go);

            scrollbar.handleRect = slidingArea.GetComponentInChildren<RectTransform>();
            scrollbar.targetGraphic = slidingArea.GetComponentInChildren<Image>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            scrollbar.interactable = true;
            scrollbar.value = 1;

            return go;
        }

        private static GameObject CreateViewport(GameObject parent)
        {
            var go = new GameObject($"{Plugin.LABEL}Viewport");

            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;

            var rect = go.AddComponent<RectTransform>();

            go.AddComponent<CanvasRenderer>();

            rect.sizeDelta = new Vector2(0.0f, 0.0f);
            rect.anchorMin = new Vector2(0.0f, 0.0f);
            rect.anchorMax = new Vector2(1.0f, 1.0f);

            go.AddComponent<RectMask2D>();

            CreateRewardGrid(go);

            return go;
        }

        private static GameObject CreateSettingsPanel(GameObject parent)
        {
            var go = new GameObject($"{Plugin.LABEL}SettingsPanel");

            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;

            var rect = go.AddComponent<RectTransform>();
            go.AddComponent<CanvasRenderer>();
            var image = go.AddComponent<Image>();
            var scrollRect = go.AddComponent<ScrollRect>();

            rect.sizeDelta = new Vector2(gridWidth, gridHeight);
            rect.anchorMax = new Vector2(0.25f, 0.5f);
            rect.anchorMin = new Vector2(0.25f, 0.5f);

            image.color = new Color32(0, 0, 0, 100);

            var verticalScrollbar = CreateVerticalScrollbar(go);
            var viewport = CreateViewport(go);

            scrollRect.vertical = true;
            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 10;
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.content = viewport.transform.GetChild(0).GetComponent<RectTransform>();
            scrollRect.verticalScrollbar = verticalScrollbar.GetComponent<Scrollbar>();

            go.SetActive(false);

            return go;
        }

        private static GameObject CreateRewardGrid(GameObject parent)
        {
            var go = new GameObject($"{Plugin.LABEL}RewardGrid");

            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;

            var rect = go.AddComponent<RectTransform>();

            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            var grid = go.AddComponent<GridLayoutGroup>();

            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.padding = new RectOffset(20, 20, 20, 20);
            grid.spacing = new Vector2(20, 20);

            AddGridItem(grid, "Image1", "#ff0000");
            AddGridItem(grid, "Image2", "#00ff00");
            AddGridItem(grid, "Image3", "#0000ff");
            AddGridItem(grid, "Image4", "#ffff00");
            AddGridItem(grid, "Image5", "#ff00ff");
            AddGridItem(grid, "Image6", "#00ffff");

            AddGridItem(grid, "Image7", "#ff0000");
            AddGridItem(grid, "Image8", "#00ff00");
            AddGridItem(grid, "Image9", "#0000ff");
            AddGridItem(grid, "Image10", "#ffff00");
            AddGridItem(grid, "Image11", "#ff00ff");
            AddGridItem(grid, "Image12", "#00ffff");

            AddGridItem(grid, "Image7", "#ff0000");
            AddGridItem(grid, "Image8", "#00ff00");
            AddGridItem(grid, "Image9", "#0000ff");
            AddGridItem(grid, "Image10", "#ffff00");
            AddGridItem(grid, "Image11", "#ff00ff");
            AddGridItem(grid, "Image12", "#00ffff");

            AddGridItem(grid, "Image7", "#ff0000");
            AddGridItem(grid, "Image8", "#00ff00");
            AddGridItem(grid, "Image9", "#0000ff");
            AddGridItem(grid, "Image10", "#ffff00");
            AddGridItem(grid, "Image11", "#ff00ff");
            AddGridItem(grid, "Image12", "#00ffff");

            return go;
        }

        private static void AddGridItem(GridLayoutGroup grid, string name, string color)
        {
            var item = CreateGridItem(name, color);

            item.transform.SetParent(grid.transform);

            var rect = grid.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, (grid.transform.childCount / gridCols * lineHeight) - gridHeight + 20);
        }

        private static Color HexToColor(string hex)
        {
            var result = ColorUtility.TryParseHtmlString($"{hex}66", out Color color);

            return result ? color : new Color(1, 1, 1, 0.5f);
        }

        private static GameObject CreateGridItem(string name, string color)
        {
            var go = new GameObject($"{Plugin.LABEL}RewardGridItem{name}");

            var image = go.AddComponent<Image>();
            image.color = HexToColor(color);

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

