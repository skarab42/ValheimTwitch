using System;
using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.GUI
{
    abstract public class ADropdownLabel : MonoBehaviour
    {
        public GameObject goLabel;
        public RectTransform rect;
        public Text text;

        public void Awake()
        {
            goLabel = new GameObject("Label");
            goLabel.transform.SetParent(transform);

            rect = goLabel.AddComponent<RectTransform>();

            SetRectTransform();

            goLabel.AddComponent<CanvasRenderer>();

            text = goLabel.AddComponent<Text>();
            text.font = EmbeddedAsset.GetFont("Norse");
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.color = Color.white;
            text.text = "None";
        }

        protected virtual void SetRectTransform()
        {
            throw new NotImplementedException();
        }
    }

    public class DropdownLabel : ADropdownLabel
    {
        protected override void SetRectTransform()
        {
            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
        }
    }

    public class DropdownLabelLeft : ADropdownLabel
    {
        protected override void SetRectTransform()
        {
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0.5f, 1);
        }
    }

    public class DropdownLabelRight : ADropdownLabel
    {
        protected override void SetRectTransform()
        {
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(0.5f, 0);
            rect.offsetMax = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(0.5f, 1);
        }
    }
}