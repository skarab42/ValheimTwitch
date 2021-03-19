using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class SpawnMuninAction : Action
    {
        public override void Run(Redemption redemption)
        {
            RavenPatch.Message(redemption.User.DisplayName, redemption.UserInput, true);
        }
    }
}