using BepInEx;
using HarmonyLib;
using System;
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
        public const string VERSION = "0.1.0";

        public const string TWITCH_APP_CLIENT_ID = "5b9v1vm0jv7kx9afpmz0ylb3lp7k9w";
        public const string TWITCH_REDIRECT_HOST = "localhost";
        public const int TWITCH_REDIRECT_PORT = 42224;

        public static readonly string[] TWITCH_SCOPES = {
            "user:read:email",
            "channel:read:redemptions"
        };

        public Rewards twitchRewards;
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
            EmbeddedAsset.LoadAssembly("Assets.WatsonWebserver.dll");
            EmbeddedAsset.LoadAssembly("Assets.websocket-sharp.dll");
        }

        public void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            if (PluginConfig.HasKey("twitch", "accessToken"))
            {
                TwitchConnect();
            }

            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();
        }

        private void OnToken(object sender, TokenArgs e)
        {
            PluginConfig.SetString("twitch", "accessToken", e.Token.AccessToken);
            PluginConfig.SetString("twitch", "refreshToken", e.Token.RefreshToken);

            TwitchConnect();

            StartPatch.UpdateMainButonText();
        }

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
                var accessToken = PluginConfig.GetString("twitch", "accessToken");
                var refreshToken = PluginConfig.GetString("twitch", "refreshToken");

                var tokenProvider = new TokenProvider(
                    TWITCH_APP_CLIENT_ID,
                    TWITCH_REDIRECT_HOST,
                    TWITCH_REDIRECT_PORT,
                    TWITCH_SCOPES
                );

                tokenProvider.OnToken += OnToken;
                tokenProvider.OnError += OnTokenError;

                twitchClient = new Twitch.API.Client(TWITCH_APP_CLIENT_ID, accessToken, refreshToken, tokenProvider);
                twitchPubSubClient = new Twitch.PubSub.Client(twitchClient);

                User user = twitchClient.GetUser();
                twitchRewards = twitchClient.GetRewards();

                Log.Info($"Twitch User: {user.Login}");

                //foreach (Twitch.API.Helix.Reward reward in rewards.Data)
                //{
                //    Log.Info($"Reward: {reward.Title}");

                //    FejdStartupPatch.AddGridItem(reward.Title, reward.BackgroundColor);
                //}

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

        private void OnMaxReconnect(object sender, Twitch.PubSub.MaxReconnectErrorArgs e)
        {
            Log.Info($"OnMaxReconnect: {e.Message}");
        }

        private void OnRewardRedeemed(object sender, Twitch.PubSub.RewardRedeemedArgs e)
        {
            Log.Info($"OnRewardRedeemed: {e.Redemption.Reward.Title}");
        }
    }
}