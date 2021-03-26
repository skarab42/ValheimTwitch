using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using UnityEngine;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch.Twitch.API
{
    public class CustomRewardException : Exception
    {
        public CustomRewardResponse response;

        public CustomRewardException(CustomRewardResponse response) : base(response.Message) {
            this.response = response;
        }
    }

    public class CustomRewardResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class Client
    {
        public Helix.User user;
        public TokenProvider tokenProvider;
        public readonly Credentials credentials;

        private readonly string helixURL = "https://api.twitch.tv/helix";

        public Client(Credentials credentials)
        {
            this.credentials = credentials;
        }

        public Client(string clientId, string accessToken)
        {
            credentials = new Credentials(clientId, accessToken);
        }

        public Client(string clientId, string accessToken, string refreshToken, TokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
            credentials = new Credentials(clientId, accessToken, refreshToken);
        }

        public string Get(string url, bool refresh = true)
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

                    if (refresh == false || response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        throw e;
                    }

                    if (tokenProvider.RefreshToken(this) == null)
                    {
                        throw e;
                    }

                    return Get(url, false);
                }
            }
        }

        public string Post(string url, string query, bool refresh = true)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add($"Client-Id: {credentials.clientId}");
                    client.Headers.Add($"Authorization: Bearer {credentials.accessToken}");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

                    return client.UploadString(url, query);
                }
                catch (WebException e)
                {
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (refresh == false || response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        throw e;
                    }

                    if (tokenProvider.RefreshToken(this) == null)
                    {
                        throw e;
                    }

                    return Post(url, query, false);
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

        public void Auth()
        {
            tokenProvider.GetToken();
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

        public string CreateCustomReward(NewRewardArgs reward)
        {
            var color = UnityEngine.Random.ColorHSV(0f, 1f, 0.6f, 0.7f, 0.4f, 0.5f);
            var hexColor = "#" + ColorUtility.ToHtmlStringRGB(color);
            var url = $"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}";
            var query = $"title={reward.Title}&cost={reward.Cost}&prompt={reward.Prompt}&is_user_input_required={reward.IsUserInputRequired}&background_color={hexColor}";

            try
            {
                return Post(url, query);
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;
                
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    var json = reader.ReadToEnd(); 
                    var message = JsonConvert.DeserializeObject<CustomRewardResponse>(json);

                    throw new CustomRewardException(message);
                }

                if (response.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw e;
                }

                return null;
            }
        }
    }
}
