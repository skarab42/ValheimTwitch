using System.Security.Cryptography;

namespace ValheimTwitch.Twitch.Utils
{
    class Generate
    {
        public static string RandomString(int length = 32, string allowableChars = null)
        {
            if (string.IsNullOrEmpty(allowableChars))
                allowableChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Generate random data
            var rnd = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(rnd);

            // Generate the output string
            var allowable = allowableChars.ToCharArray();
            var l = allowable.Length;
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = allowable[rnd[i] % l];

            return new string(chars);
        }
    }
}
