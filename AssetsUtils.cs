using System.IO;

using UnityEngine;
using UnityEngine.Networking;

namespace JaLib
{
    public static class AssetUtils
    {
        public static Texture2D LoadTextureFromFile(string texturePath)
        {
            string path = Path.Combine(BepInEx.Paths.PluginPath, texturePath);

            if (!File.Exists(path))
            {
                return null;
            }

            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
            ImageConversion.LoadImage(tex, data);

            tex.filterMode = FilterMode.Point;

            return tex;
        }

        public static Sprite LoadSpriteFromFile(string spritePath)
        {
            return LoadSpriteFromFile(spritePath, Vector2.zero);
        }

        public static Sprite LoadSpriteFromFile(string spritePath, Vector2 pivot)
        {
            Texture2D tex = LoadTextureFromFile(spritePath);

            if (tex != null)
            {
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot);
            }

            return null;
        }

        public static AudioClip LoadAudioClipFromFile(string audioClipPath, AudioType type)
        {
            string path = Path.Combine(BepInEx.Paths.PluginPath, audioClipPath);

            if (!File.Exists(path))
                return null;

            var uwr = UnityWebRequest.GetAudioClip("file:///" + path, type);

            uwr.SendWebRequest();

            while (!uwr.isDone)
                continue;

            if (
                uwr.result == UnityWebRequest.Result.ConnectionError
                || uwr.result == UnityWebRequest.Result.ProtocolError
            )
                return null;

            return DownloadHandlerAudioClip.GetContent(uwr);
        }

        public static AssetBundle LoadAssetBundleFromFile(string bundlePath)
        {
            string path = Path.Combine(BepInEx.Paths.PluginPath, bundlePath);

            if (!File.Exists(path))
            {
                return null;
            }

            return AssetBundle.LoadFromFile(path);
        }
    }
}
