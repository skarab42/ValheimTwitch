using Newtonsoft.Json.Linq;
using System;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    // 0 None
    // 1 RavenMessage
    // 2 SpawnCreature
    // 3 ChangeEnvironment 
    // 4 Start event
    // 5 Environement

    public static class Actions
    {
        internal static void RunAction(Redemption redemption, JToken data)
        {
            try
            {
                var type = data["Action"].Value<int>();

                Log.Info($"RunAction: {redemption.Reward.Id} -> type: {type}");

                switch (type)
                {
                    case 0:
                        break;
                    case 1:
                        RavenMessageAction.Run(redemption, data);
                        break;
                    case 2:
                        SpawnCreatureAction.Run(redemption, data);
                        break;
                    case 3:
                        HUDMessageAction.Run(redemption, data);
                        break;
                    case 4:
                        StartRandomEventAction.Run(redemption, data);
                        break;
                    case 5:
                        ChangeEnvironmentAction.Run(redemption, data);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Log.Error("RunAction Error >>> " + ex.ToString());
            }
        }
    }
}
