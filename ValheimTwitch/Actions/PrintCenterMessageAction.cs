using System;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class PrintCenterMessageAction : Action
    {
        public override void Run(Redemption redemption)
        {
            try
            {
                var user = redemption.User.DisplayName;
                var text = redemption.UserInput;

                Log.Info($"Message -> user:{user} text:{text}");

                if (Player.m_localPlayer != null)
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"<{user}> {text}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}