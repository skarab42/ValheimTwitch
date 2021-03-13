using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using UnityEngine;

namespace ValheimTwitch
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "dev.skarab42.valheim_twitch";
        public const string NAME = "Valheim Twitch";
        public const string LABEL = "ValheimTwitch";
        public const string VERSION = "0.1.0";

        public const string TWITCH_TOKEN_GENERATOR_URL = "https://twitchtokengenerator.com";

        public ConfigEntry<string> twitchClientId;
        public ConfigEntry<string> twitchAccessToken;

        public Twitch.Client twitchClient;
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
                Application.OpenURL(TWITCH_TOKEN_GENERATOR_URL);
            }
            else
            {
                try
                {
                    twitchClient = new Twitch.Client(twitchClientId.Value, twitchAccessToken.Value);
                    twitchPubSubClient = new Twitch.PubSub.Client(twitchClient);

                    Twitch.Helix.User user = twitchClient.GetUser();
                    Log.Info($"Twitch User: {user.Login}");

                    twitchPubSubClient.OnRewardRedeemed += OnRewardRedeemed;
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

        private void OnRewardRedeemed(object sender, Twitch.PubSub.RewardRedeemedArgs e)
        {
            Log.Info($"OnRewardRedeemed: {e.Redemption.Reward.Title}");
        }
    }
}