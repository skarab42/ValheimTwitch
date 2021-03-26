using Newtonsoft.Json;

namespace ValheimTwitch.Twitch.API.Helix
{
    public class CustomRewardResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
