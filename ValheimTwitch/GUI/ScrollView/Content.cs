using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI.ScrollView
{
    public class Content : MonoBehaviour
    {
        public GameObject goContent;
        public RectTransform rect;

        private void Awake()
        {
            goContent = new GameObject("Content");
            goContent.transform.SetParent(transform);

            rect = goContent.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);

            //var image = goContent.AddComponent<Image>();
            //image.color = new Color32(0, 255, 0, 100);

            var fitter = goContent.AddComponent<ContentSizeFitter>();

            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
}