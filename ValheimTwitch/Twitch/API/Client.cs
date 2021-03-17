using Newtonsoft.Json;
using System;
using System.Net;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch.Twitch.API
{
    public class Client
    {
        public Helix.User user;

        private readonly Credentials credentials;
        private readonly string helixURL = "https://api.twitch.tv/helix";

        public Client(Credentials credentials)
        {
            this.credentials = credentials;
        }

        public Client(string clientId, string accessToken)
        {
            credentials = new Credentials(clientId, accessToken);
        }

        public Client(string clientId, string accessToken, string refreshToken)
        {
            credentials = new Credentials(clientId, accessToken, refreshToken);
        }

        public string Get(string url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add($"Client-Id: {credentials.clientId}");
                    client.Headers.Add($"Authorization: Bearer {credentials.accessToken}");

                    return client.DownloadString(url);
                }
                catch (WebException e)
                {
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        throw e;
                    }

                    // try to refresh token
                    // -> Get
                    // -> throw Ex
                    // return Get(url, false);
                    throw new Exception("TODO refresh token!");
                }
            }
        }

        public Helix.User GetUser(bool force = false)
        {
            if (user != null && force == false)
            {
                return user;
            }

            string json = Get($"{helixURL}/users");

            var users = JsonConvert.DeserializeObject<Helix.Users>(json);

            user = users.Data[0];

            return user;
        }

        public string GetUserAcessToken()
        {
            return credentials.accessToken;
        }

        public Helix.Rewards GetRewards()
        {
            string json = Get($"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}");

            return JsonConvert.DeserializeObject<Helix.Rewards>(json);
        }
    }
}
