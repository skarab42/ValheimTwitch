using ValheimTwitch.Helpers;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class SpawnBoarHordAction : Action
    {
        public override void Run(Redemption redemption)
        {
            ConsoleUpdatePatch.AddAction(() => {
                for (int i = 0; i < 10; i++)
                {
                    Prefab.Spawn("Boar", 1, 20);
                }
            });
        }
    }
}