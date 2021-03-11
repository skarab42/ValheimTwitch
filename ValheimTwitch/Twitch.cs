using SimpleJSON;
using System.Net;

namespace ValheimTwitch
{
    public class TwitchUser
    {
        public string broadcasterType; // User’s broadcaster type: "partner", "affiliate", or "".
        public string description;     // User’s channel description.
        public string displayName;     // User’s display name.
        public string id;              // User’s ID.
        public string login;           // User’s login name.
        public string offlineImageURL; // URL of the user’s offline image.
        public string profileImageURL; // URL of the user’s profile image.
        public string type;            // User’s type: "staff", "admin", "global_mod", or "".
        public int    viewCount;       // Total number of views of the user’s channel.
        public string email;           // User’s verified email address. Returned if the request includes the user:read:email scope.
        public string createdAt;       // Date when the user was created.

        public static TwitchUser Factory(JSONNode node)
        {
            var user = new TwitchUser
            {
                broadcasterType = node["broadcaster_type"].Value,
                description     = node["description"].Value,
                displayName     = node["display_name"].Value,
                id              = node["id"].Value,
                login           = node["login"].Value,
                offlineImageURL = node["offline_image_url"].Value,
                profileImageURL = node["profile_image_url"].Value,
                type            = node["type"].Value,
                viewCount       = node["view_count"].AsInt,
                email           = node["email"].Value,
                createdAt       = node["created_at"].Value
            };

            return user;
        }
    }

    class Twitch
    {
        public static TwitchUser user = null;

        private static string clientId;
        private static string accessToken;

        private static readonly WebClient client = new WebClient();
        private static readonly string helixURL = "https://api.twitch.tv/helix";

        public static void SetAuth(string clientId, string accessToken)
        {
            Twitch.clientId = clientId;
            Twitch.accessToken = accessToken;
        }

        public static string Get(string url)
        {
            client.Headers.Add($"Client-Id: {clientId}");
            client.Headers.Add($"Authorization: Bearer {accessToken}");

            return client.DownloadString(url);
        }

        public static TwitchUser GetUser(bool force = false)
        {
            if (user != null && !force)
            {
                return user;
            }

            string json = Get($"{helixURL}/users");

            JSONNode node = JSON.Parse(json);

            return TwitchUser.Factory(node["data"][0]);
        }
    }
}
