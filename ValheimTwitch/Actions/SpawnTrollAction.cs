using ValheimTwitch.Helpers;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class SpawnTrollAction : Action
    {
        public override void Run(Redemption redemption)
        {
            ConsoleUpdatePatch.AddAction(() => Prefab.Spawn("Troll", 1));
        }
    }
}