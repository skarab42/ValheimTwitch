using System;
using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.GUI.ScrollView;

namespace ValheimTwitch.GUI
{
    public class DropdownOptions : MonoBehaviour
    {
        private GameObject goOptions;
        public GridLayoutGroup grid;

        private void Awake()
        {
            goOptions = new GameObject("CustomDropdownOptions");
            goOptions.transform.SetParent(transform);

            var rect = goOptions.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 340);

            goOptions.AddComponent<CanvasRenderer>();

            var image = goOptions.AddComponent<Image>();
            image.color = new Color32(0, 0, 0, 200);

            var scrollView = goOptions.AddComponent<VerticalScrollView>();
            grid = scrollView.AddContentComponent<GridLayoutGroup>();

            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.padding = new RectOffset(0, 0, 0, 0);
            grid.cellSize = new Vector2(380, 40);
            grid.spacing = new Vector2(0, 2);
        }

        public void Toggle()
        {
            goOptions.SetActive(!goOptions.activeSelf);
        }
    }
}