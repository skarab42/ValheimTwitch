using BepInEx;

namespace ValheimTwitch
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGUID = "dev.skarab42.valheim_twitch";
        public const string pluginName = "Valheim Twitch";
        public const string pluginLabel = "ValheimTwitch";
        public const string pluginVersion = "0.1.0";

        void Awake()
        {
            Log.Info($"{pluginName} Awake!");
        }
    }
}