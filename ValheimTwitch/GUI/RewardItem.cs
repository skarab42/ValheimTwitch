using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using ValheimTwitch.Helpers;

namespace ValheimTwitch.GUI
{
    class RewardItem : MonoBehaviour
    {
        private GameObject goRewardItem;
        private Image bgImage;
        private Image image;
        private Text text;

        private void Awake()
        {
            goRewardItem = new GameObject("RewardItem");
            goRewardItem.transform.SetParent(transform);

            bgImage = goRewardItem.AddComponent<Image>();

            // reward image
            var goImage = new GameObject("RewardItemImage");
            goImage.transform.SetParent(goRewardItem.transform);

            var rect = goImage.AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(50, 50);
            rect.anchorMin = new Vector2(0.5f, 0.65f);
            rect.anchorMax = new Vector2(0.5f, 0.65f);

            image = goImage.AddComponent<Image>();

            // reward text
            var goText = new GameObject("RewardItemText");
            goText.transform.SetParent(goRewardItem.transform);

            var textRect = goText.AddComponent<RectTransform>();

            textRect.sizeDelta = new Vector2(0, 0);
            textRect.offsetMin = new Vector2(0, 0);
            textRect.offsetMax = new Vector2(0, 0);
            textRect.anchorMin = new Vector2(0.15f, 0.0f);
            textRect.anchorMax = new Vector2(0.85f, 0.5f);

            text = goText.AddComponent<Text>();
            text.font = EmbeddedAsset.GetFont("Norse");
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.color = Color.white;
        }

        private static Color HexToColor(string hex)
        {
            var result = ColorUtility.TryParseHtmlString($"{hex}cc", out Color color);

            return result ? color : new Color(1, 1, 1, 0.5f);
        }

        public Texture2D LoadTextureFromURL(string url)
        {
            using (WebClient client = new WebClient())
            {
                var bytes = client.DownloadData(url);
                var texture = new Texture2D(2, 2);

                texture.LoadImage(bytes);

                return texture;
            }
        }

        public void SetReward(Twitch.API.Helix.Reward reward)
        {
            text.text = reward.Title;
            bgImage.color = HexToColor(reward.BackgroundColor);

            if (text.text.Length > 15)
            {
                text.text = text.text.Substring(0, 15).TrimEnd() + ". . .";
            }

            try
            {
                var texture = LoadTextureFromURL((reward.Image ?? reward.DefaultImage).Url4x);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;
            } 
            catch(Exception)
            {
                Log.Warning($"Reward image unavailable: {reward.Title}");
            }
        }
    }
}
