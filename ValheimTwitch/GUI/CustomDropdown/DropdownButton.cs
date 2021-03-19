using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ValheimTwitch.GUI
{
    public class DropdownButton : MonoBehaviour
    {
        public GameObject goButton;
        public DropdownLabel label;
        public Button button;
        public Image image;

        private void Awake()
        {
            goButton = new GameObject("Button");
            goButton.transform.SetParent(transform);

            var rect = goButton.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            image = goButton.AddComponent<Image>();
            button = goButton.AddComponent<Button>();
            label = goButton.AddComponent<DropdownLabel>();

            image.color = new Color32(255, 255, 255, 127);
        }

        public void SetText(string text)
        {
            label.text.text = text;
        }

        public void SetColor(Color32 color)
        {
            image.color = color;
        }

        public void OnClick(UnityAction action)
        {
            button.onClick.AddListener(action);
        }
    }
}