using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.GUI
{
    public class DropdownLabel : MonoBehaviour
    {
        private GameObject goLabel;
        public Text text;

        private void Awake()
        {
            goLabel = new GameObject("Label");
            goLabel.transform.SetParent(transform);

            var rect = goLabel.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);

            goLabel.AddComponent<CanvasRenderer>();

            text = goLabel.AddComponent<Text>();
            text.font = EmbeddedAsset.GetFont("Norse");
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.color = Color.black;
            text.text = "None";
        }
    }
}