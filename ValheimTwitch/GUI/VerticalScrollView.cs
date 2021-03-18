using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI
{
    public class VerticalScrollView : MonoBehaviour
    {
        private GameObject goVerticalScrollView;

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
            var verticalScrollbar = goVerticalScrollView.AddComponent<VerticalScrollbar>();

            scrollRect.vertical = true;
            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 10;
            scrollRect.verticalScrollbar = verticalScrollbar.scrollbar;
            //scrollRect.viewport = viewport.GetComponent<RectTransform>();
            //scrollRect.content = viewport.transform.GetChild(0).GetComponent<RectTransform>();
        }
    }
}