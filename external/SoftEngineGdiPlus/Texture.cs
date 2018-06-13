using System;
using SharpDX;
using System.Drawing;

namespace SoftEngine
{

    public class Texture
    {
        private byte[] internalBuffer;
        private int width;
        private int height;

        // Working with a fix sized texture (512x512, 1024x1024, etc.).
        public Texture(string filename, int width, int height)
        {
            this.width = width;
            this.height = height;
            Load(filename);
        }

        void Load(string filename)
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename))
            {
                var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int stride = data.Stride;
                internalBuffer = new byte[stride * data.Height];
                System.Runtime.InteropServices.Marshal.Copy(data.Scan0,
                    internalBuffer, 0, internalBuffer.Length);
                bmp.UnlockBits(data);
            }
        }

        // Takes the U & V coordinates exported by Blender
        // and return the corresponding pixel color in the texture
        public Color4 Map(double tu, double tv)
        {
            // Image is not loaded yet
            if (internalBuffer == null)
            {
                return new Color4(1, 1, 1, 1);
            }
            // using a % operator to cycle/repeat the texture if needed
            int u = Math.Abs((int)(tu * width) % width);
            int v = Math.Abs((int)(tv * height) % height);

            int pos = (u + v * width) * 4;
            byte b = internalBuffer[pos];
            byte g = internalBuffer[pos + 1];
            byte r = internalBuffer[pos + 2];
            byte a = internalBuffer[pos + 3];

            return new Color4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }
    }
}
