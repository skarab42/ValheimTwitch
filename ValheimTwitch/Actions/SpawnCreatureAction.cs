using Newtonsoft.Json.Linq;
using ValheimTwitch.Helpers;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class SpawnCreatureAction
    {
        internal static void Run(Redemption redemption, JToken data)
        {
            var creature = data["Creature"].Value<int>();
            var level = data["Level"].Value<int>();
            var count = data["Count"].Value<int>();
            var offset = data["Distance"].Value<int>();
            var tamed = data?["Tamed"].Value<bool>();

            var name = redemption.User.DisplayName;
            var type = SpawnCreatureSettings.creatures[creature];

            for (int i = 0; i < count; i++)
            {
                ConsoleUpdatePatch.AddAction(() => Prefab.Spawn(type, level, offset, tamed??false, name));
            }
        }
    }
}