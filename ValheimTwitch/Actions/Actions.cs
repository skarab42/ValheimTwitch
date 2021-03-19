using System.Collections.Generic;

namespace ValheimTwitch.Events
{

    abstract public class Action
    {
        public void Run()
        {
            Log.Info($"Run -> {GetType().Name}");
        }
    }

    public static class Actions
    {
        public enum Types
        {
            None,
            SpawnHugin,
            SpawnMunin,
            PrintCenterMessage,
            PrintTopLeftMessage
        }
        
        private static Dictionary<int, string> names = new Dictionary<int, string>
        {
            { (int)Types.None, "None" },
            { (int)Types.SpawnHugin, "Spawn Hugin" },
            { (int)Types.SpawnMunin, "Spawn Munin" },
            { (int)Types.PrintCenterMessage, "Print center message" },
            { (int)Types.PrintTopLeftMessage, "Print top left message" }
        };

        private static Dictionary<int, Action> actions = new Dictionary<int, Action>
        {
            { (int)Types.None, new NoneAction() },
            { (int)Types.SpawnHugin, new SpawnHuginAction() },
            { (int)Types.SpawnMunin, new SpawnMuninAction() },
            { (int)Types.PrintCenterMessage, new PrintCenterMessageAction() },
            { (int)Types.PrintTopLeftMessage, new PrintTopLeftMessageAction() }
        };

        public static Dictionary<int, string> GetActionNames()
        {
            return names;
        }

        public static string GetActionName(Types type)
        {
            return names[(int)type];
        }

        public static string GetActionName(int type)
        {
            return names[type];
        }

        public static void RunAction(Types type)
        {
            actions[(int)type].Run();
        }

        public static void RunAction(int type)
        {
            actions[type].Run();
        }
    }
}
