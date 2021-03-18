using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.GUI.ScrollView;

namespace ValheimTwitch.GUI
{
    public class MainPanel : MonoBehaviour
    {
        private GameObject goMainPanel;

        private void Awake()
        {
            goMainPanel = new GameObject("MainPanel");
            goMainPanel.transform.SetParent(transform);

            var rect = goMainPanel.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(400, 400);
            rect.anchorMin = new Vector2(0.0f, 0.5f);
            rect.anchorMax = new Vector2(0.0f, 0.5f);
            rect.offsetMin = new Vector2(0.0f, -200.0f);
            rect.offsetMax = new Vector2(400.0f, 200.0f);

            rect.Translate(280, 0, 0);

            goMainPanel.AddComponent<CanvasRenderer>();

            var image = goMainPanel.AddComponent<Image>();

            image.color = new Color32(0, 0, 0, 120);

            var scrollView = goMainPanel.AddComponent<VerticalScrollView>();
            var grid = scrollView.AddContentComponent<GridLayoutGroup>();

            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.padding = new RectOffset(20, 20, 20, 20);
            grid.spacing = new Vector2(20, 20);

            foreach (Twitch.API.Helix.Reward reward in Plugin.Instance.twitchRewards.Data)
            {
                Log.Info($"Reward: {reward.Title}");

                var item = grid.transform.gameObject.AddComponent<RewardItem>();

                item.SetReward(reward);
            }

            //
            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();

            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();

            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();

            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();

            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();

            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();
            //grid.transform.gameObject.AddComponent<RewardItem>();

            goMainPanel.SetActive(false);
        }

        public void ToggleActive() {
            goMainPanel.SetActive(!goMainPanel.activeSelf);
        }
    }
}