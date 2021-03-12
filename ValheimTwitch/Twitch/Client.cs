using SimpleJSON;
using System.Net;

namespace ValheimTwitch.Twitch
{
    /// <summary>
    /// Twitch API client.
    /// </summary>
    public class Client
    {
        public Helix.User user;

        private readonly Credentials credentials;
        private readonly WebClient client = new WebClient();
        private readonly string helixURL = "https://api.twitch.tv/helix";

        public Client(Credentials credentials)
        {
            this.credentials = credentials;
        }

        public Client(string clientId, string accessToken)
        {
            credentials = new Credentials(clientId, accessToken);
        }

        public string Get(string url)
        {
            client.Headers.Add($"Client-Id: {credentials.clientId}");
            client.Headers.Add($"Authorization: Bearer {credentials.accessToken}");

            return client.DownloadString(url);
        }

        public Helix.User GetUser(bool force = false)
        {
            if (user != null && force == true)
            {
                return user;
            }

            string json = Get($"{helixURL}/users");

            JSONNode node = JSON.Parse(json);

            user = Helix.User.Factory(node["data"][0]);

            return user;
        }
    }
}
