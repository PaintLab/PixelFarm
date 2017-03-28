//MIT, 2017, Zou Wei(github/zwcloud)
using System.IO;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.ES30;
using Android.Util;

namespace Xamarin.OpenGL
{
    internal class Utility
    {
        internal static Stream ReadFile(string filePath)
        {
            using (Stream s = Test_Android_Glyph.MainActivity.AssetManager.Open(filePath))
            using (var ms = new MemoryStream())// This is a simple hack because on Xamarin.Android, a `Stream` created by `AssetManager.Open` is not seekable.
            {
                s.CopyTo(ms);
                return new MemoryStream(ms.ToArray());
            }
        }
         
    }
}