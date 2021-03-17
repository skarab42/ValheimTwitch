using HarmonyLib;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
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
        private static GridLayoutGroup grid;
        private static GameObject goSettingsUI;

        public static Twitch.API.Helix.Rewards rewards;

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

            grid = go.AddComponent<GridLayoutGroup>();

            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.padding = new RectOffset(20, 20, 20, 20);
            grid.spacing = new Vector2(20, 20);

            foreach (Twitch.API.Helix.Reward reward in rewards.Data)
            {
                AddReward(reward);
            }

            return go;
        }

        public static void AddReward(Twitch.API.Helix.Reward reward)
        {
            var item = CreateGridItem(reward);

            item.transform.SetParent(grid.transform);

            var rect = grid.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, (grid.transform.childCount / gridCols * lineHeight) - gridHeight + 20);
        }

        private static Color HexToColor(string hex)
        {
            var result = ColorUtility.TryParseHtmlString($"{hex}cc", out Color color);

            return result ? color : new Color(1, 1, 1, 0.5f);
        }

        public static async Task<Texture2D> GetRemoteTextureAsync(string url)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                // begin request:
                var asyncOp = www.SendWebRequest();

                Log.Info($"Loading :{www.url}");

                // await until it's done: 
                while (asyncOp.isDone == false)
                    await Task.Delay(1000 / 30); //30 hertz

                Log.Info($"Loaded :{www.url}");

                // read results:
                if (www.isNetworkError || www.isHttpError)
                {
                    Log.Info($"{www.error}, URL:{www.url}");

                    // nothing to return on error:
                    return null;
                }
                else
                {
                    Log.Info($"Done :{www.url}");

                    // return valid results:
                    return DownloadHandlerTexture.GetContent(www);
                }
            }
        }

        public static Texture2D GetRemoteTexture(string url)
        {
            Log.Info($"PROUT -------- 1");

            Task<Texture2D> task = Task.Run<Texture2D>(async () => await GetRemoteTextureAsync(url));

            Log.Info($"PROUT -------- 2");

            return task.Result;
        }

        private static GameObject CreateGridItem(Twitch.API.Helix.Reward reward)
        {
            var go = new GameObject($"{Plugin.LABEL}RewardGridItem{reward.Title}");

            var bgImage = go.AddComponent<Image>();
            bgImage.color = HexToColor(reward.BackgroundColor);

            var goImage = new GameObject($"{Plugin.LABEL}RewardGridItem{reward.Title}Immage");

            goImage.transform.SetParent(go.transform);

            Log.Info($"Image -> {reward.DefaultImage.Url1x}");

            var image = goImage.AddComponent<Image>();
            var texture = GetRemoteTextureAsync(reward.DefaultImage.Url1x).GetAwaiter().GetResult();
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            image.sprite = sprite;

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

