using Newtonsoft.Json.Linq;
using UnityEngine;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class StartRandomEventAction
    {
        internal static void Run(Redemption redemption, JToken data)
        {
            ConsoleUpdatePatch.AddAction(() => StartRandomEvent(redemption, data));
        }

        private static void StartRandomEvent(Redemption redemption, JToken data)
        {
            var eventName = data["EventName"].Value<string>();
            var distance = data["Distance"].Value<int>();
            var duration = data["Duration"].Value<int>();

            Vector3 b = Random.insideUnitSphere * distance;
            var position = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up + b;

            RandomEventSystemStartPatch.StartEvent(redemption, eventName, position, duration);
        }
    }
}