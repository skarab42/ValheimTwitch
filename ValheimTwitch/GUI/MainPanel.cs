using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI
{
    public class MainPanel : MonoBehaviour
    {
        private GameObject goMainPanel;

        private void Awake()
        {
            goMainPanel = new GameObject("MainPanel");
            goMainPanel.transform.SetParent(transform);

            var rect = goMainPanel.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(400, 400);
            rect.anchorMin = new Vector2(0.0f, 0.5f);
            rect.anchorMax = new Vector2(0.0f, 0.5f);
            rect.offsetMin = new Vector2(0.0f, -200.0f);
            rect.offsetMax = new Vector2(400.0f, 200.0f);

            rect.Translate(280, 0, 0);

            goMainPanel.AddComponent<CanvasRenderer>();

            var image = goMainPanel.AddComponent<Image>();

            image.color = new Color32(0, 0, 0, 120);

            goMainPanel.AddComponent<VerticalScrollView>();

            goMainPanel.SetActive(false);
        }

        public void ToggleActive() {
            goMainPanel.SetActive(!goMainPanel.activeSelf);
        }
    }
}