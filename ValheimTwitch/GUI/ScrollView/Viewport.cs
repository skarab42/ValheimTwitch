using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI.ScrollView
{
    public class Viewport : MonoBehaviour
    {
        private GameObject goViewport;
        public RectTransform rect;
        public Content content;

        private void Awake()
        {
            goViewport = new GameObject("Viewport");
            goViewport.transform.SetParent(transform);

            rect = goViewport.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            goViewport.AddComponent<CanvasRenderer>();
            goViewport.AddComponent<RectMask2D>();

            content = goViewport.AddComponent<Content>();

            //var image = goViewport.AddComponent<Image>();
            //image.color = new Color32(255, 0, 0, 100);
        }
    }
}