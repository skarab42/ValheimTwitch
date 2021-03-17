using UnityEngine;

namespace ValheimTwitch.GUI
{
    public class StartGui : MonoBehaviour
    {
        public GameObject startGui;

        private void Awake()
        {
            startGui = new GameObject($"{Plugin.LABEL}StartGui");
            startGui.transform.SetParent(this.transform);

            var rect = startGui.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0.0f, 0.0f); 
            rect.anchorMax = new Vector2(1.0f, 1.0f);
            rect.offsetMin = new Vector2(0.0f, 0.0f);
            rect.offsetMax = new Vector2(0.0f, 0.0f);

            startGui.AddComponent<MainButton>();
        }

        public void SetActive(bool active)
        {
            startGui.SetActive(active);
        }
    }
}
