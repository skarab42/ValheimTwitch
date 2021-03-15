using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;

namespace ValheimTwitch
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "dev.skarab42.valheim_twitch";
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

        public ConfigEntry<string> twitchClientId;
        public ConfigEntry<string> twitchAccessToken;

        public Twitch.API.Client twitchClient;
        public Twitch.PubSub.Client twitchPubSubClient;

        private static Plugin instance;

        public static Plugin Instance
        {
            get => instance;
            private set { instance = value; }
        }

        private Plugin() { }

        public void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            twitchClientId = Config.Bind("Twitch", "ClientId", "", "Twitch client ID");
            twitchAccessToken = Config.Bind("Twitch", "AccessToken", "", "Twitch access token");

            if (twitchClientId.Value.Length == 0 || twitchAccessToken.Value.Length == 0)
            {
                // TODO open custom url with tutorial or handle Twitch login direcly ?
                // Application.OpenURL(TWITCH_TOKEN_GENERATOR_URL);
                Log.Info("Show Twitch login link...");
            }
            else
            {
                try
                {
                    twitchClient = new Twitch.API.Client(twitchClientId.Value, twitchAccessToken.Value);
                    twitchPubSubClient = new Twitch.PubSub.Client(twitchClient);

                    Twitch.API.Helix.User user = twitchClient.GetUser();
                    Log.Info($"Twitch User: {user.Login}");

                    twitchPubSubClient.OnRewardRedeemed += OnRewardRedeemed;
                    twitchPubSubClient.OnMaxReconnect += OnMaxReconnect;
                    twitchPubSubClient.Connect();
                }
                catch (Exception e)
                {
                    Log.Error($"Twitch User: {e.Message}");
                    // TODO open custom url with how to setup
                }
            }

            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();
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