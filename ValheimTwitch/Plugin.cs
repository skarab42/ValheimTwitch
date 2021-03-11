using BepInEx;
using BepInEx.Configuration;

namespace ValheimTwitch
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "dev.skarab42.valheim_twitch";
        public const string NAME = "Valheim Twitch";
        public const string LABEL = "ValheimTwitch";
        public const string VERSION = "0.1.0";

        public static ConfigEntry<string> twitchClientId;
        public static ConfigEntry<string> twitchAccessToken;

        void Awake()
        {
            Log.Info($"{NAME} Awake!");

            twitchClientId = Config.Bind("Twitch", "ClientId", "", "Twitch client ID");
            twitchAccessToken = Config.Bind("Twitch", "AccessToken", "", "Twitch access token");
        }
    }
}