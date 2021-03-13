using Newtonsoft.Json;

namespace ValheimTwitch.Twitch.PubSub.Messages
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
    }

    public class Image
    {
        [JsonProperty("url_1x")]
        public string Url1x { get; set; }

        [JsonProperty("url_2x")]
        public string Url2x { get; set; }

        [JsonProperty("url_4x")]
        public string Url4x { get; set; }
    }

    public class DefaultImage
    {
        [JsonProperty("url_1x")]
        public string Url1x { get; set; }

        [JsonProperty("url_2x")]
        public string Url2x { get; set; }

        [JsonProperty("url_4x")]
        public string Url4x { get; set; }
    }

    public class MaxPerStream
    {
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("max_per_stream")]
        public int Count { get; set; }
    }

    public class Reward
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("cost")]
        public int Cost { get; set; }

        [JsonProperty("is_user_input_required")]
        public bool IsUserInputRequired { get; set; }

        [JsonProperty("is_sub_only")]
        public bool IsSubOnly { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("default_image")]
        public DefaultImage DefaultImage { get; set; }

        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }

        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("is_paused")]
        public bool IsPaused { get; set; }

        [JsonProperty("is_in_stock")]
        public bool IsInStock { get; set; }

        [JsonProperty("max_per_stream")]
        public MaxPerStream MaxPerStream { get; set; }

        [JsonProperty("should_redemptions_skip_request_queue")]
        public bool ShouldRedemptionsSkipRequestQueue { get; set; }
    }

    public class Redemption
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("redeemed_at")]
        public string RedeemedAt { get; set; }

        [JsonProperty("reward")]
        public Reward Reward { get; set; }

        [JsonProperty("user_input")]
        public string UserInput { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Data
    {
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("redemption")]
        public Redemption Redemption { get; set; }
    }

    public class RewardRedeem
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
