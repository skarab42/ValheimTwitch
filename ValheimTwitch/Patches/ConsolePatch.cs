using HarmonyLib;
using System;
using System.Collections.Concurrent;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(Console), "Update")]
    public static class ConsoleUpdatePatch
    {
        private static ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        public static void AddAction(Action action)
        {
            actions.Enqueue(action);
        }

        public static void Postfix()
        {
            if (actions.Count == 0)
            {
                return;
            }

            Action action;

            while (actions.TryDequeue(out action))
                action();
        }
    }
}

