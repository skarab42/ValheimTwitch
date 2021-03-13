using Newtonsoft.Json;
using System.Collections.Generic;

namespace ValheimTwitch.Twitch.API.Helix
{
    public class Users
    {
        [JsonConstructor]
        public Users(
            [JsonProperty("data")] List<User> data
        )
        {
            this.Data = data;
        }

        [JsonProperty("data")]
        public IReadOnlyList<User> Data { get; }
    }
}
