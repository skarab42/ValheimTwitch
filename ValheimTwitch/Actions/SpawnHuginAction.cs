using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class SpawnHuginAction : Action
    {
        public override void Run(Redemption redemption)
        {
            RavenPatch.Message(redemption.User.DisplayName, redemption.UserInput, false);
        }
    }
}