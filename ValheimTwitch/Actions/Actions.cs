using System.Collections.Generic;

namespace ValheimTwitch
{
    public static class Actions
    {
        public enum Types
        {
            None,
            SpawnHugin,
            SpawnMudin,
            PrintCenterMessage,
            PrintTopLeftMessage
        }
        
        private static Dictionary<int, string> names = new Dictionary<int, string>
        {
            { (int)Types.None, "None" },
            { (int)Types.SpawnHugin, "Spawn Hugin" },
            { (int)Types.SpawnMudin, "Spawn Mudin" },
            { (int)Types.PrintCenterMessage, "Print center message" },
            { (int)Types.PrintTopLeftMessage, "Print top left message" }
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
    }
}
