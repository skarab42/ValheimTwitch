using BepInEx;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.SceneManagement;
using ValheimTwitch.Events;
using ValheimTwitch.Helpers;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.API.Helix;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "dev.skarab42.valheim.twitch";
        public const string NAME = "Valheim Twitch";
        public const string LABEL = "ValheimTwitch";
        public const string VERSION = "1.2.0";

        public const string TWITCH_APP_CLIENT_ID = "5b9v1vm0jv7kx9afpmz0ylb3lp7k9w";
        public const string TWITCH_REDIRECT_HOST = "localhost";
        public const string TWITCH_SKARAB42_ID = "485824438"; //"18615783"; //
        public const int TWITCH_REDIRECT_PORT = 42224;

        public static readonly string[] TWITCH_SCOPES = {
            "user:read:email",
            "channel:read:redemptions",
            "channel:manage:redemptions"
        };

        //public bool updateUI = false;
        public Rewards twitchRewards;
        public Rewards twitchCustomRewards;
        public Twitch.API.Client twitchClient;
        public Twitch.PubSub.Client twitchPubSubClient;

        private static Plugin instance;

        public static Plugin Instance
        {
            get => instance;
            private set { instance = value; }
        }

        private Plugin() {
            EmbeddedAsset.LoadAssembly("Assets.IpMatcher.dll");
            EmbeddedAsset.LoadAssembly("Assets.netstandard.dll");
            EmbeddedAsset.LoadAssembly("Assets.Newtonsoft.Json.dll");
            EmbeddedAsset.LoadAssembly("Assets.RegexMatcher.dll");
            EmbeddedAsset.LoadAssembly("Assets.System.Runtime.Serialization.dll");
            EmbeddedAsset.LoadAssembly("Assets.UrlMatcher.dll");
            EmbeddedAsset.LoadAssembly("Assets.ValheimTwitchGUI.dll");
            EmbeddedAsset.LoadAssembly("Assets.WatsonWebserver.dll");
            EmbeddedAsset.LoadAssembly("Assets.websocket-sharp.dll");

            if (!PluginConfig.HasKey("channel:manage:redemptions"))
            {
                PluginConfig.DeleteKey("twitchAuthToken");
                PluginConfig.SetString("channel:manage:redemptions", "ok");
            }

            //PluginConfig.DeleteKey("channel:manage:redemptions"); 
            //PluginConfig.DeleteKey("twitchAuthToken");
            //PluginConfig.DeleteKey("rewards");
        }

        public void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            RewardsConfig.Load();

            TwitchConnect();

            SceneManager.activeSceneChanged += OnSceneChanged;

            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();
        }

        private void OnToken(object sender, TokenArgs e)
        {
            PluginConfig.SetObject("twitchAuthToken", new {
                accessToken = e.Token.AccessToken,
                refreshToken = e.Token.RefreshToken
            });

            TwitchConnect();
            UpdateRwardsList();
        }

        //public void Update()
        //{
        //    // TODO use action queue
        //    if (updateUI)
        //    {
        //        FejdStartupStartPatch.UpdateRewardGrid();
        //        FejdStartupStartPatch.UpdateMainButonText();
        //    }

        //    updateUI = false;
        //}

        public User GetUser()
        {
            return twitchClient?.user;
        }

        private void OnTokenError(object sender, TokenErrorArgs e)
        {
            Log.Error($"OnTokenError: {e.Message}");
            // TODO notify user...
        }

        public void TwitchAuth()
        {
            Log.Debug("TwitchAuth....");

            twitchClient.Auth();
        }

        public void TwitchConnect()
        {
            try
            {
                var tokenProvider = new TokenProvider(
                    TWITCH_APP_CLIENT_ID,
                    TWITCH_REDIRECT_HOST,
                    TWITCH_REDIRECT_PORT,
                    TWITCH_SCOPES
                );

                tokenProvider.OnToken += OnToken;
                tokenProvider.OnError += OnTokenError;

                var token = PluginConfig.GetObject("twitchAuthToken");

                if (token == null)
                {
                    twitchClient = new Twitch.API.Client(TWITCH_APP_CLIENT_ID, "", "", tokenProvider);
                    return;
                }

                JToken accessToken;
                JToken refreshToken;

                token.TryGetValue("accessToken", out accessToken);
                token.TryGetValue("refreshToken", out refreshToken);

                twitchClient = new Twitch.API.Client(TWITCH_APP_CLIENT_ID, accessToken.Value<string>(), refreshToken.Value<string>(), tokenProvider);

                twitchPubSubClient = new Twitch.PubSub.Client(twitchClient);

                User user = twitchClient.GetUser();
                var isFollowing = twitchClient.IsFollowing();

                Log.Info($"Twitch User: {user.Login} (follow: {isFollowing})");

                UpdateRwardsList();

                twitchPubSubClient.OnRewardRedeemed += OnRewardRedeemed;
                twitchPubSubClient.OnMaxReconnect += OnMaxReconnect;

                twitchPubSubClient.Connect();
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e}");
                // TODO notify user...
            }
        }

        public void UpdateRwardsList()
        {
            twitchCustomRewards = twitchClient?.GetCustomRewards();
            twitchRewards = twitchClient?.GetRewards();
            FejdStartupUpdatePatch.updateUI = true;
        }

        public void ToggleRewards(bool enable)
        {
            if (twitchCustomRewards == null)
                return;

            bool needRefresh = false;

            foreach (var reward in new List<Reward>(twitchCustomRewards.Data))
            {
                Log.Info($">>> ToggleReward: {reward.Id} -> {enable}");
                try
                {
                    twitchClient.ToggleReward(reward.Id, enable);
                }

                catch (WebException e)
                {
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (response.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw e;
                    }

                    needRefresh = true;
                }
            }

            if (needRefresh)
            {
                UpdateRwardsList();
                FejdStartupUpdatePatch.updateUI = true;
            }
        }

        private void OnSceneChanged(Scene current, Scene next)
        {
            ToggleRewards(next.name == "main");
        }

        private void OnMaxReconnect(object sender, Twitch.PubSub.MaxReconnectErrorArgs e)
        {
            Log.Info($"OnMaxReconnect: {e.Message}");
        }

        private void OnRewardRedeemed(object sender, Twitch.PubSub.RewardRedeemedArgs e)
        {
            var reward = e.Redemption.Reward;

            Log.Info($"OnRewardRedeemed: {reward.Title}");

            var action = RewardsConfig.Get(reward.Id);

            if (action == null)
                return;

            Actions.RunAction(e.Redemption, action);
        }
    }
}