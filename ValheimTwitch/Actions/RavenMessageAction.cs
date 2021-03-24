using Newtonsoft.Json.Linq;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class RavenMessageAction
    {
        internal static void Run(Redemption redemption, JToken data)
        {
            var munin = data["Type"].Value<bool>();

            RavenPatch.Message(redemption.User.DisplayName, redemption.UserInput, munin);
        }
    }
}