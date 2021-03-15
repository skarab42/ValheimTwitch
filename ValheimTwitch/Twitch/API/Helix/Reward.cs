using Newtonsoft.Json;
using System.Collections.Generic;

namespace ValheimTwitch.Twitch.API.Helix
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Image
    {
        [JsonProperty("url_1x")]
        public string Url1x { get; set; }

        [JsonProperty("url_2x")]
        public string Url2x { get; set; }

        [JsonProperty("url_4x")]
        public string Url4x { get; set; }
    }

    public class MaxPerStreamSetting
    {
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("max_per_stream")]
        public int MaxPerStream { get; set; }
    }

    public class MaxPerUserPerStreamSetting
    {
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("max_per_user_per_stream")]
        public int MaxPerUserPerStream { get; set; }
    }

    public class GlobalCooldownSetting
    {
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("global_cooldown_seconds")]
        public int GlobalCooldownSeconds { get; set; }
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

    public class Reward
    {
        [JsonProperty("broadcaster_name")]
        public string BroadcasterName { get; set; }

        [JsonProperty("broadcaster_login")]
        public string BroadcasterLogin { get; set; }

        [JsonProperty("broadcaster_id")]
        public string BroadcasterId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }

        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("cost")]
        public int Cost { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("is_user_input_required")]
        public bool IsUserInputRequired { get; set; }

        [JsonProperty("max_per_stream_setting")]
        public MaxPerStreamSetting MaxPerStreamSetting { get; set; }

        [JsonProperty("max_per_user_per_stream_setting")]
        public MaxPerUserPerStreamSetting MaxPerUserPerStreamSetting { get; set; }

        [JsonProperty("global_cooldown_setting")]
        public GlobalCooldownSetting GlobalCooldownSetting { get; set; }

        [JsonProperty("is_paused")]
        public bool IsPaused { get; set; }

        [JsonProperty("is_in_stock")]
        public bool IsInStock { get; set; }

        [JsonProperty("default_image")]
        public DefaultImage DefaultImage { get; set; }

        [JsonProperty("should_redemptions_skip_request_queue")]
        public bool ShouldRedemptionsSkipRequestQueue { get; set; }

        [JsonProperty("redemptions_redeemed_current_stream")]
        public object RedemptionsRedeemedCurrentStream { get; set; }

        [JsonProperty("cooldown_expires_at")]
        public object CooldownExpiresAt { get; set; }
    }
}