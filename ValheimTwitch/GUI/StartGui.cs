using UnityEngine;

namespace ValheimTwitch.GUI
{
    public class StartGui : MonoBehaviour
    {
        public MainButton mainButton;

        private GameObject goStartGui;

        private void Awake()
        {
            goStartGui = new GameObject($"{Plugin.LABEL}StartGui");
            goStartGui.transform.SetParent(transform);

            var rect = goStartGui.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0.0f, 0.0f); 
            rect.anchorMax = new Vector2(1.0f, 1.0f);
            rect.offsetMin = new Vector2(0.0f, 0.0f);
            rect.offsetMax = new Vector2(0.0f, 0.0f);

            mainButton = goStartGui.AddComponent<MainButton>();
        }

        public void SetMainButtonText(string text)
        {
            mainButton.SetText(text);
        }

        public void SetActive(bool active)
        {
            goStartGui.SetActive(active);
        }
    }
}
