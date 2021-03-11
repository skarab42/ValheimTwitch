using System.Net;

namespace ValheimTwitch
{
    class Twitch
    {
        private static string clientId;
        private static string accessToken;

        private static readonly WebClient client = new WebClient();
        private static readonly string helixURL = "https://api.twitch.tv/helix";

        public static void SetAuth(string clientId, string accessToken)
        {
            Twitch.clientId = clientId;
            Twitch.accessToken = accessToken;
        }

        public static string GetUser()
        {
            var url = $"{helixURL}/users";

            client.Headers.Add($"Client-Id: {clientId}");
            client.Headers.Add($"Authorization: Bearer {accessToken}");

            return client.DownloadString(url);
        }
    }
}
