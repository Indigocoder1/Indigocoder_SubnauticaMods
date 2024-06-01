using Nautilus.Utility;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using UnityEngine;

namespace TextureReplacerEditor.Miscellaneous
{
    internal static class TextureLoadSaveHandler
    {
        public static void SaveTexture(Texture texture)
        {
            var sfd = new SaveFileDialog();
            sfd.Title = "Select an export location";
            sfd.Filter = "PNG Files|*.png";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string path = sfd.FileName;

                if (string.IsNullOrEmpty(path))
                {
                    ErrorMessage.AddError("<color=ff0000>Invalid texture save path!</color>");
                    return;
                }
                
                byte[] texBytes = texture.BasedEncoteToPNG();
                File.WriteAllBytes(path, texBytes);
            }
        }

        public static Texture2D LoadTexture()
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Load texture from disk";
            ofd.Filter = "PNG Files|*.png";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                if(string.IsNullOrEmpty(ofd.FileName))
                {
                    ErrorMessage.AddError("<color=ff0000>Invalid texture path!</color>");
                    return null;
                }

                return ImageUtils.LoadTextureFromFile(ofd.FileName);
            }

            return null;
        }

        //https://github.com/LeeTwentyThree/SubnauticaMods/blob/341e867b0debb8e32c3a48423487477872bd82fa/SubnauticaRuntimeEditor/SubnauticaRuntimeEditor.Core/Utils/TextureUtils.cs#L43
        private static byte[] BasedEncoteToPNG(this Texture tex)
        {
            var t2d = tex.ToTexture2D();
            try
            {
                var m = typeof(Texture2D).GetMethod("EncodeToPNG", BindingFlags.Instance | BindingFlags.Public);
                if (m != null)
                {
                    return (byte[])m.Invoke(t2d, new object[0]);
                }
                else
                {
                    var t = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule", false);
                    var m2 = t?.GetMethod("EncodeToPNG", BindingFlags.Static | BindingFlags.Public);
                    if (m2 != null)
                    {
                        return (byte[])m2.Invoke(null, new object[] { t2d });
                    }

                    throw new Exception("Could not find method EncodeToPNG, can't save to file.");
                }
            }
            finally
            {
                UnityEngine.Object.Destroy(t2d);
            }
        }

        //https://github.com/LeeTwentyThree/SubnauticaMods/blob/341e867b0debb8e32c3a48423487477872bd82fa/SubnauticaRuntimeEditor/SubnauticaRuntimeEditor.Core/Utils/TextureUtils.cs#L21
        private static Texture2D ToTexture2D(this Texture tex, TextureFormat format = TextureFormat.ARGB32, bool mipMaps = false)
        {
            var rt = RenderTexture.GetTemporary(tex.width, tex.height);
            var prev = RenderTexture.active;
            RenderTexture.active = rt;

            GL.Clear(true, true, Color.clear);

            Graphics.Blit(tex, rt);

            var t = new Texture2D(tex.width, tex.height, format, mipMaps);
            t.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            t.Apply(false);

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            return t;
        }
    }
}
