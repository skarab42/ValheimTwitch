using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using ValheimTwitch.Helpers;
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

        public ConfigEntry<string> twitchAccessToken;

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

            twitchAccessToken = Config.Bind("Twitch", "AccessToken", "", "Twitch access token");

            if (twitchAccessToken.Value.Length != 0)
            {
                TwitchLogin();
            }

            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();
        }

        private void TwitchLogin()
        {
            try
            {
                twitchClient = new Twitch.API.Client(TWITCH_APP_CLIENT_ID, twitchAccessToken.Value);
                twitchPubSubClient = new Twitch.PubSub.Client(twitchClient);

                Twitch.API.Helix.User user = twitchClient.GetUser();
                Twitch.API.Helix.Rewards rewards = twitchClient.GetRewards();

                Log.Info($"Twitch User: {user.Login}");
                
                foreach (Twitch.API.Helix.Reward reward in rewards.Data)
                {
                    Log.Info($"Reward: {reward.Title}");
                }

                twitchPubSubClient.OnRewardRedeemed += OnRewardRedeemed;
                twitchPubSubClient.OnMaxReconnect += OnMaxReconnect;

                twitchPubSubClient.Connect();
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}");
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

        internal void OnAuthToken(Token token)
        {
            twitchAccessToken.Value = token.AccessToken;

            TwitchLogin();

            FejdStartupPatch.UpdateText();
        }
    }
}