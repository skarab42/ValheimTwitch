﻿using Newtonsoft.Json;
using System.Net;

namespace ValheimTwitch.Twitch.API
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
    }
}