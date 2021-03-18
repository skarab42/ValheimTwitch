using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.GUI
{
    public class MainButton : MonoBehaviour
    {
        public MainButtonText mainButtonText;
        public Button button;

        private GameObject goMainButton;

        private void Awake()
        {
            goMainButton = new GameObject("MainButton");
            goMainButton.transform.SetParent(transform);

            var rect = goMainButton.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(250, 100);
            rect.anchorMin = new Vector2(0.0f, 0.5f);
            rect.anchorMax = new Vector2(0.0f, 0.5f);
            rect.offsetMin = new Vector2(0.0f, -50.0f);
            rect.offsetMax = new Vector2(250.0f, 50.0f);

            rect.Translate(10, 0, 0);

            goMainButton.AddComponent<CanvasRenderer>();

            var image = goMainButton.AddComponent<Image>();

            Texture2D logoTexture = EmbeddedAsset.LoadTexture2D("Assets.TwitchLogo.png");
            var sprite = Sprite.Create(logoTexture, new Rect(0, 0, logoTexture.width, logoTexture.height), new Vector2(0.5f, 0.5f));

            image.sprite = sprite;

            button = goMainButton.AddComponent<Button>();
            button.targetGraphic = image;

            mainButtonText = goMainButton.AddComponent<MainButtonText>();
        }

        public void SetText(string text)
        {
            mainButtonText.SetText(text);
        }

        public void SetActive(bool active)
        {
            goMainButton.SetActive(active);
        }
    }
}
