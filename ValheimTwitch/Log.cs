using BepInEx.Logging;

namespace ValheimTwitch
{
    class Log
    {
        private static ManualLogSource logger = Logger.CreateLogSource(Plugin.LABEL);

        public static void Debug(string message)
        {
            logger.LogDebug(message);
        }

        public static void Info(string message)
        {
            logger.LogInfo(message);
        }

        public static void Warning(string message)
        {
            logger.LogWarning(message);
        }

        public static void Error(string message)
        {
            logger.LogError(message);
        }
    }
}
