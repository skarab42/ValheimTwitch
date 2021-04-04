using Newtonsoft.Json.Linq;
using System;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class PlayerAction
    {
        internal static void Run(Redemption redemption, JToken data)
        {
            var name = data["Name"].Value<string>();

            switch (name)
            {
                case "puke":
                    ConsoleUpdatePatch.AddAction(Puke);
                    break;
                case "heal":
                    ConsoleUpdatePatch.AddAction(Heal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static void Puke()
        {
            if (Player.m_localPlayer != null)
                Player.m_localPlayer.ClearFood();
        }

        static void Heal ()
        {
            if (Player.m_localPlayer != null)
                Player.m_localPlayer.Heal(Player.m_localPlayer.GetMaxHealth(), true);
        }
    }
}