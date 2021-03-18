using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI.ScrollView
{
    public class Handle : MonoBehaviour
    {
        public RectTransform rect;
        public Image image;

        private void Awake()
        {
            var goHandle = new GameObject("Handle");
            goHandle.transform.SetParent(transform);

            rect = goHandle.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            goHandle.AddComponent<CanvasRenderer>();

            image = goHandle.AddComponent<Image>();
            image.color = new Color32(234, 128, 2, 150);
        }
    }
}