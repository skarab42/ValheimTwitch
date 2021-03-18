using UnityEngine;

namespace ValheimTwitch.GUI
{
    public class SlidingArea : MonoBehaviour
    {
        private GameObject goSlidingArea;
        public SlidingAreaHandle handle;

        private void Awake()
        {
            goSlidingArea = new GameObject("SlidingArea");
            goSlidingArea.transform.SetParent(transform);

            var rect = goSlidingArea.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            handle = goSlidingArea.AddComponent<SlidingAreaHandle>();
        }
    }
}