using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ValheimTwitch.GUI
{
    public class OptionsItem : MonoBehaviour
    {
        private GameObject goOptions;
        private DropdownButton button;
        private string label;
        private string value;

        private void Awake()
        {
            goOptions = new GameObject("OptionsItem");
            goOptions.transform.SetParent(transform);

            var rect = goOptions.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(0, -40);
            rect.offsetMax = new Vector2(20, 0);

            goOptions.AddComponent<CanvasRenderer>();

            button = goOptions.AddComponent<DropdownButton>();
        }

        public void SetLabel(string label)
        {
            this.label = label;
            button.SetText(label);
        }

        public string GetLabel()
        {
            return label;
        }

        public void SetValue(string value)
        {
            this.value = value;
        }

        public string GetValue()
        {
            return value;
        }

        public void SetColor(Color32 color)
        {
            button.SetColor(color);
        }

        public void OnClick(UnityAction action)
        {
            button.OnClick(action);
        }
    }
}