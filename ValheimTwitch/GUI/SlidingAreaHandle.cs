using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI
{
    public class SlidingAreaHandle : MonoBehaviour
    {
        private GameObject goSlidingAreaHandle;
        public RectTransform rect;
        public Image image;

        private void Awake()
        {
            goSlidingAreaHandle = new GameObject("SlidingAreaHandle");
            goSlidingAreaHandle.transform.SetParent(transform);

            rect = goSlidingAreaHandle.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            goSlidingAreaHandle.AddComponent<CanvasRenderer>();

            image = goSlidingAreaHandle.AddComponent<Image>();
            image.color = new Color32(0, 0, 255, 150);
        }
    }
}