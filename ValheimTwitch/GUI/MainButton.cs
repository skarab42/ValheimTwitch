using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.GUI
{
    public class MainButton : MonoBehaviour
    {
        private MainButtonText mainButtonText;
        private GameObject mainButton;
        private Button button;

        private void Awake()
        {
            mainButton = new GameObject("MainButton");
            mainButton.transform.SetParent(transform);

            var rect = mainButton.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(250, 100);
            rect.anchorMin = new Vector2(0.0f, 0.5f);
            rect.anchorMax = new Vector2(0.0f, 0.5f);
            rect.offsetMin = new Vector2(0.0f, 0.0f);
            rect.offsetMax = new Vector2(250.0f, 100.0f);

            rect.Translate(10, 0, 0);

            mainButton.AddComponent<CanvasRenderer>();

            var image = mainButton.AddComponent<Image>();

            Texture2D logoTexture = EmbeddedAsset.LoadTexture2D("Assets.TwitchLogo.png");
            var sprite = Sprite.Create(logoTexture, new Rect(0, 0, logoTexture.width, logoTexture.height), new Vector2(0.5f, 0.5f));

            image.sprite = sprite;

            button = mainButton.AddComponent<Button>();
            button.targetGraphic = image;

            mainButtonText = mainButton.AddComponent<MainButtonText>();
        }

        public void SetText(string text)
        {
            mainButtonText.SetText(text);
        }

        public void SetActive(bool active)
        {
            mainButton.SetActive(active);
        }
    }
}
