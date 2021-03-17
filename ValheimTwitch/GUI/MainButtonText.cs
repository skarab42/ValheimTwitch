using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.GUI
{
    public class MainButtonText : MonoBehaviour
    {
        private GameObject goMainButtonText;
        private Text buttonText;

        private void Awake()
        {
            goMainButtonText = new GameObject("MainButtonText");
            goMainButtonText.transform.SetParent(transform);

            var rect = goMainButtonText.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0.0f, 0.0f);
            rect.anchorMin = new Vector2(0.3f, 0.0f);
            rect.anchorMax = new Vector2(0.95f, 0.55f);
            rect.offsetMin = new Vector2(0.0f, 0.0f);
            rect.offsetMax = new Vector2(0.0f, 0.0f);

            goMainButtonText.AddComponent<CanvasRenderer>();

            buttonText = goMainButtonText.AddComponent<Text>();
            buttonText.font = EmbeddedAsset.GetFont("Norse");
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.resizeTextForBestFit = true;
            buttonText.color = Color.white;
            buttonText.text = "Loading...";
        }

        public void SetText(string text)
        {
            buttonText.text = text;
        }
    }
}
