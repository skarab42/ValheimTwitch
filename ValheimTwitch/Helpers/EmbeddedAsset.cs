using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using WWUtils.Audio;

namespace ValheimTwitch.Helpers
{
    // https://github.com/valheimPlus/ValheimPlus/blob/development/ValheimPlus/Utility/EmbeddedAsset.cs
    public static class EmbeddedAsset
    {
        public static Stream LoadEmbeddedAsset(string assetPath)
        {
            Assembly objAsm = Assembly.GetExecutingAssembly();

            if (objAsm.GetManifestResourceInfo(objAsm.GetName().Name + "." + assetPath) != null)
            {
                return objAsm.GetManifestResourceStream(objAsm.GetName().Name + "." + assetPath);
            }

            return null;
        }

        public static bool LoadAssembly(string assetPath)
        {
            Stream fileStream = LoadEmbeddedAsset(assetPath);

            if (fileStream != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    Assembly.Load(memoryStream.ToArray());
                    fileStream.Dispose();

                    return true;
                }
            }

            return false;
        }

        public static AssetBundle LoadAssetBundle(string assetPath)
        {
            Stream fileStream = LoadEmbeddedAsset(assetPath);

            if (fileStream != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    var ret = AssetBundle.LoadFromMemory(memoryStream.ToArray());
                    fileStream.Dispose();

                    return ret;
                }
            }

            return null;
        }

        public static Texture2D LoadTexture2D(string assetPath)
        {
            Texture2D texture = null;
            Stream fileStream = LoadEmbeddedAsset(assetPath);

            if (fileStream != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    texture = new Texture2D(2, 2);
                    
                    fileStream.CopyTo(memoryStream);
                    texture.LoadImage(memoryStream.ToArray());
                    fileStream.Dispose();
                }
            }

            return texture;
        }

        public static AudioClip LoadAudioClip(string assetPath)
        {
            AudioClip audioClip = null;
            Stream fileStream = LoadEmbeddedAsset(assetPath);

            if (fileStream != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);

                    WAV wav = new WAV(memoryStream.ToArray());
                    audioClip = AudioClip.Create(assetPath, wav.SampleCount, 1, wav.Frequency, false);

                    audioClip.SetData(wav.LeftChannel, 0);
                }
            }

            return audioClip;
        }

        public static Font GetFont(string fontName)
        {
            var fonts = Resources.FindObjectsOfTypeAll<Font>();

            foreach (Font font in fonts)
            {
                if (font.name.Equals(fontName))
                    return font;
            }

            return fonts[0];
        }
    }

}
