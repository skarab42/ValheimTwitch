using System;
using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Events;
using ValheimTwitch.GUI.ScrollView;

namespace ValheimTwitch.GUI
{
    public class MainPanel : MonoBehaviour
    {
        private GameObject goMainPanel;
        private Twitch.API.Helix.Reward selectedReward;

        private void Awake()
        {
            goMainPanel = new GameObject("MainPanel");
            goMainPanel.transform.SetParent(transform);

            var rect = goMainPanel.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(380, 380);
            rect.anchorMin = new Vector2(0.0f, 0.5f);
            rect.anchorMax = new Vector2(0.0f, 0.5f);
            rect.offsetMin = new Vector2(0.0f, -190.0f);
            rect.offsetMax = new Vector2(380.0f, 190.0f);

            rect.Translate(280, 0, 0);

            goMainPanel.AddComponent<CanvasRenderer>();

            var image = goMainPanel.AddComponent<Image>();

            image.color = new Color32(0, 0, 0, 120);

            var scrollView = goMainPanel.AddComponent<VerticalScrollView>();
            var grid = scrollView.AddContentComponent<GridLayoutGroup>();
            var dropdown = goMainPanel.AddComponent<CustomDropdown>();

            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.padding = new RectOffset(20, 20, 20, 20);
            grid.spacing = new Vector2(20, 20);


            foreach (Twitch.API.Helix.Reward reward in Plugin.Instance.twitchRewards.Data)
            {
                Log.Info($"Reward: {reward.Title}");

                var item = grid.transform.gameObject.AddComponent<RewardItem>();

                item.button.onClick.AddListener(() => {
                    Log.Info($"Clicked on {reward.Title}");

                    selectedReward = reward;

                    dropdown.SetPrefix(reward.Title);
                    dropdown.Toggle();

                    if (PluginConfig.HasKey("reward-actions", reward.Id))
                    {
                        var value = PluginConfig.GetInt("reward-actions", reward.Id);
                        dropdown.SetLabel(Actions.GetActionName(value));
                    } else
                    {
                        dropdown.SetLabel(Actions.GetActionName(Actions.Types.None));
                    }
                });

                item.SetReward(reward);
            }

            foreach (var action in Actions.GetActionNames())
            {
                var item = dropdown.AddOption<OptionsItem>();

                item.SetLabel(action.Value);
                item.SetValue(action.Key.ToString());
                item.SetColor(new Color32(127, 127, 127, 127));

                item.OnClick(() =>
                {
                    dropdown.Toggle();
                    dropdown.SetLabel(item.GetLabel());

                    if (selectedReward != null)
                    {
                        Log.Info($"reward-actions -> {selectedReward.Id} -> {item.GetValue()}");
                        PluginConfig.SetInt("reward-actions", selectedReward.Id, Convert.ToInt32(item.GetValue()));
                    }
                });
            }

            // hide...
            dropdown.options.Toggle();
            goMainPanel.SetActive(false);
        }

        public void ToggleActive() {
            goMainPanel.SetActive(!goMainPanel.activeSelf);
        }
    }
}