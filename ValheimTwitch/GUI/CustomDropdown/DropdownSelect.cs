using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ValheimTwitch.GUI
{
    public class DropdownSelect : MonoBehaviour
    {
        private GameObject goSelect;
        public ADropdownLabel label;
        public ADropdownLabel prefix;
        private Button button;

        private void Awake()
        {
            goSelect = new GameObject("CustomDropdownSelect");
            goSelect.transform.SetParent(transform);

            var rect = goSelect.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(0, -40);
            rect.offsetMax = new Vector2(0, 0);

            goSelect.AddComponent<CanvasRenderer>();

            var image = goSelect.AddComponent<Image>();
            image.color = new Color32(0, 0, 0, 200);

            button = goSelect.AddComponent<Button>();
            prefix = goSelect.AddComponent<DropdownLabelLeft>();
            label = goSelect.AddComponent<DropdownLabelRight>();
        }

        public void OnClick(UnityAction action)
        {
            button.onClick.AddListener(action);
        }
    }
}