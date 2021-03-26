using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ValheimTwitch.Twitch.API.Helix
{
    public class Follower
    {
        [JsonProperty("from_id")]
        public string FromId { get; set; }

        [JsonProperty("from_login")]
        public string FromLogin { get; set; }

        [JsonProperty("from_name")]
        public string FromName { get; set; }

        [JsonProperty("to_id")]
        public string ToId { get; set; }

        [JsonProperty("to_login")]
        public string ToLogin { get; set; }

        [JsonProperty("to_name")]
        public string ToName { get; set; }

        [JsonProperty("followed_at")]
        public DateTime FollowedAt { get; set; }
    }

    public class Pagination
    {
    }

    public class FollowsResponse
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("data")]
        public List<Follower> Data { get; set; }

        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }
}
