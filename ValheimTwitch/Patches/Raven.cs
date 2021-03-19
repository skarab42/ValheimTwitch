using System;

namespace ValheimTwitch.Patches
{
    public static class RavenPatch
    {
        static int messageCount = 0;

        public static void Message(string user, string text, bool munin)
        {
            try
            {
                Log.Info($"Message -> user:{user} text:{text}");

                Raven.RavenText ravenText = new Raven.RavenText();

                var key = $"{Plugin.GUID}_" + messageCount++;
                var label = user;
                var topic = user;

                Log.Info($"AddTempText -> {key}");

                Raven.AddTempText(key, topic, text, label, munin);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
