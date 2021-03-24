using Newtonsoft.Json.Linq;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class HUDMessageAction
    {
        internal static void Run(Redemption redemption, JToken data)
        {
            var position = data["Position"].Value<int>();
            var user = redemption.User.DisplayName;
            var text = redemption.UserInput;

            var messageType = position == 0 ? MessageHud.MessageType.TopLeft : MessageHud.MessageType.Center;

            Log.Info($"Message -> user:{user} text:{text}");

            if (Player.m_localPlayer != null)
            {
                Player.m_localPlayer.Message(messageType, $"<{user}> {text}");
            }
        }
    }
}