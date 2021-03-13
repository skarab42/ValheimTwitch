using Newtonsoft.Json;
using System;

namespace ValheimTwitch.Twitch.API.Helix
{
    public class User
    {
        [JsonConstructor]
        public User(
            [JsonProperty("id")] string id,
            [JsonProperty("login")] string login,
            [JsonProperty("display_name")] string displayName,
            [JsonProperty("type")] string type,
            [JsonProperty("broadcaster_type")] string broadcasterType,
            [JsonProperty("description")] string description,
            [JsonProperty("profile_image_url")] string profileImageUrl,
            [JsonProperty("offline_image_url")] string offlineImageUrl,
            [JsonProperty("view_count")] int viewCount,
            [JsonProperty("email")] string email,
            [JsonProperty("created_at")] DateTime createdAt
        )
        {
            this.Id = id;
            this.Login = login;
            this.DisplayName = displayName;
            this.Type = type;
            this.BroadcasterType = broadcasterType;
            this.Description = description;
            this.ProfileImageUrl = profileImageUrl;
            this.OfflineImageUrl = offlineImageUrl;
            this.ViewCount = viewCount;
            this.Email = email;
            this.CreatedAt = createdAt;
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("login")]
        public string Login { get; }

        [JsonProperty("display_name")]
        public string DisplayName { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("broadcaster_type")]
        public string BroadcasterType { get; }

        [JsonProperty("description")]
        public string Description { get; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; }

        [JsonProperty("offline_image_url")]
        public string OfflineImageUrl { get; }

        [JsonProperty("view_count")]
        public int ViewCount { get; }

        [JsonProperty("email")]
        public string Email { get; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }
    }
}
