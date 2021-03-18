using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI
{
    public class VerticalScrollbar : MonoBehaviour
    {
        private GameObject goVerticalScrollbar;
        public Scrollbar scrollbar;

        private void Awake()
        {
            goVerticalScrollbar = new GameObject("VerticalScrollbar");
            goVerticalScrollbar.transform.SetParent(transform);

            var rect = goVerticalScrollbar.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(20, 0);
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            goVerticalScrollbar.AddComponent<CanvasRenderer>();
            scrollbar = goVerticalScrollbar.AddComponent<Scrollbar>();

            var image = goVerticalScrollbar.AddComponent<Image>();
            image.color = new Color32(0, 255, 0, 150);

            var slidingArea = goVerticalScrollbar.AddComponent<SlidingArea>();

            scrollbar.targetGraphic = slidingArea.handle.image;
            scrollbar.handleRect = slidingArea.handle.rect;
        }
    }
}