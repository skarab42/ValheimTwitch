using UnityEngine;
using ValheimTwitch.Patches;

namespace ValheimTwitch.Helpers
{
    static class CustomInput
    {
        public static KeyCode GetCode(string name)
        {
            return FejdStartupStartPatch.shortcuts.Find(shortcut => shortcut.Name == name).Code;
        }

        public static bool GetKey(string name)
        {
            return Input.GetKey(GetCode(name));
        }

        public static bool GetKeyDown(string name)
        {
            return Input.GetKeyDown(GetCode(name));
        }
    }
}
