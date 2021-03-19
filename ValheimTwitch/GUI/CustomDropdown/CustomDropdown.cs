using System;
using UnityEngine;

namespace ValheimTwitch.GUI
{
    public class CustomDropdown : MonoBehaviour
    {
        private GameObject goDropdown;
        private DropdownSelect select;
        public DropdownOptions options;

        private void Awake()
        {
            goDropdown = new GameObject("CustomDropdown");
            goDropdown.transform.SetParent(transform);

            var rect = goDropdown.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);

            goDropdown.AddComponent<CanvasRenderer>();

            select = goDropdown.AddComponent<DropdownSelect>();
            options = goDropdown.AddComponent<DropdownOptions>();

            select.OnClick(() => options.Toggle());
        }

        public T AddOption<T>() where T : Component
        {
            return options.grid.transform.gameObject.AddComponent<T>();
        }

        public void SetLabel(string label)
        {
            select.label.text.text = label;
        }

        public void SetPrefix(string label)
        {
            select.prefix.text.text = label;
        }

        public void Toggle()
        {
            options.Toggle();
        }
    }
}