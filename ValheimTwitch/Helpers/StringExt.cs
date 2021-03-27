namespace ValheimTwitch.Helpers
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength, string ext = "…")
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + ext;
        }
    }
}
