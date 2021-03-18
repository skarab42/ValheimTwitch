using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI.ScrollView
{
    public class VerticalScrollView : MonoBehaviour
    {
        public VerticalScrollbar verticalScrollbar;

        private GameObject goVerticalScrollView;
        private Viewport viewport;

        private void Awake()
        {
            goVerticalScrollView = new GameObject("VerticalScrollView");
            goVerticalScrollView.transform.SetParent(transform);

            var rect = goVerticalScrollView.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            goVerticalScrollView.AddComponent<CanvasRenderer>();

            var scrollRect = goVerticalScrollView.AddComponent<ScrollRect>();
            verticalScrollbar = goVerticalScrollView.AddComponent<VerticalScrollbar>();
            viewport = goVerticalScrollView.AddComponent<Viewport>();

            scrollRect.vertical = true;
            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 10;
            scrollRect.viewport = viewport.rect;
            scrollRect.content = viewport.content.rect;
            scrollRect.verticalScrollbar = verticalScrollbar.scrollbar;
        }

        public T AddContentComponent<T>() where T : Component
        {
            return viewport.content.goContent.AddComponent<T>();
        }
    }
}