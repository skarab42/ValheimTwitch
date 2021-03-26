using Newtonsoft.Json;
using System.Collections.Generic;

namespace ValheimTwitch.Twitch.API.Helix
{
    public class Rewards
    {
        [JsonConstructor]
        public Rewards(
            [JsonProperty("data")] List<Reward> data
        )
        {
            this.Data = data;
        }

        [JsonProperty("data")]
        public List<Reward> Data { get; }
    }
}
