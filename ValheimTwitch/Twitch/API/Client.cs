using Newtonsoft.Json;
using System.IO;
using System.Net;
using UnityEngine;
using ValheimTwitch.Twitch.API.Helix;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch.Twitch.API
{
    public class Client
    {
        public User user;
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

                    return client.UploadString(url, "POST", query);
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

        public string Patch(string url, string query, bool refresh = true)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add($"Client-Id: {credentials.clientId}");
                    client.Headers.Add($"Authorization: Bearer {credentials.accessToken}");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                    return client.UploadString(url, "PATCH", query);
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

                    return Patch(url, query, false);
                }
            }
        }

        public User GetUser(bool force = false)
        {
            if (user != null && force == false)
            {
                return user;
            }

            string json = Get($"{helixURL}/users");

            var users = JsonConvert.DeserializeObject<Users>(json);

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

        public Rewards GetRewards(bool custom = false)
        {
            string json = Get($"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}&only_manageable_rewards={custom}");

            return JsonConvert.DeserializeObject<Rewards>(json);
        }

        public Rewards GetCustomRewards()
        {
            return GetRewards(true);
        }

        public bool IsFollowing()
        {
            string json = Get($"{helixURL}/users/follows?to_id={Plugin.TWITCH_SKARAB42_ID}&from_id={user.Id}");
            var follower = JsonConvert.DeserializeObject<FollowsResponse>(json);

            return follower?.Total == 1;
        }

        public string ToggleReward(string id, bool enabled)
        {
            try
            {
                var url = $"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}&id={id}";

                return Patch(url, JsonConvert.SerializeObject(new { is_enabled = enabled }));
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

        public string CreateCustomReward(NewRewardArgs reward)
        {
            var color = Random.ColorHSV(0f, 1f, 0.6f, 0.7f, 0.4f, 0.5f);
            var backgroundColor = "#" + ColorUtility.ToHtmlStringRGB(color);
            var url = $"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}";
            var query = $"title={reward.Title}" +
                        $"&cost={reward.Cost}" +
                        $"&prompt={reward.Prompt}" +
                        $"&background_color={backgroundColor}" +
                        $"&is_user_input_required={reward.IsUserInputRequired}" +
                        $"&should_redemptions_skip_request_queue={reward.ShouldRedemptionsSkipRequestQueue}";

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
