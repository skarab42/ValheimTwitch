using System.Net;
using UnityEngine;

namespace ValheimTwitch.Helpers
{
    public static class TextureLoader
    {
        public static Texture2D LoadFromURL(string url)
        {
            using (WebClient client = new WebClient())
            {
                var bytes = client.DownloadData(url);
                var texture = new Texture2D(2, 2);

                texture.LoadImage(bytes);

                return texture;
            }
        }
    }
}
