using UnityEngine;

namespace ValheimTwitch.Helpers
{
    public static class Colors
    {
        public static Color FromHex(string hex)
        {
            var result = ColorUtility.TryParseHtmlString($"{hex}", out Color color);

            return result ? color : new Color(1, 1, 1, 0.5f);
        }
    }
}
