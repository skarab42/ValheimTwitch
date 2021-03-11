using BepInEx;
using BepInEx.Configuration;
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

        public static ConfigEntry<string> twitchClientId;
        public static ConfigEntry<string> twitchAccessToken;

        public void Awake()
        {
            twitchClientId = Config.Bind("Twitch", "ClientId", "", "Twitch client ID");
            twitchAccessToken = Config.Bind("Twitch", "AccessToken", "", "Twitch access token");

            if (twitchClientId.Value.Length == 0 || twitchAccessToken.Value.Length == 0)
            {
                // TODO open custom url with tutorial or handle Twitch login direcly ?
                Application.OpenURL(TWITCH_TOKEN_GENERATOR_URL);
            } 
            else
            {
                Twitch.SetAuth(twitchClientId.Value, twitchAccessToken.Value);

                try
                {
                    TwitchUser user = Twitch.GetUser();

                    Log.Info($"Twitch User: {user.displayName}");
                } catch (Exception e)
                {
                    Log.Error($"Twitch User: {e.Message}");
                    // TODO open custom url with how to setup
                }
            }
        } 
    }
}