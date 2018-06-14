
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SoftEngine
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    public struct BitmapBuffer
    {
        public static readonly BitmapBuffer Empty = new BitmapBuffer();

        //in this version , only 32 bits 
        public BitmapBuffer(int w, int h)
        {
            this.PixelWidth = w;
            this.PixelHeight = h;
            this.Pixels = new int[w * h];
        }
        public BitmapBuffer(int w, int h, int[] orgBuffer)
        {
            this.PixelWidth = w;
            this.PixelHeight = h;
            this.Pixels = orgBuffer;
        }
        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }
        /// <summary>
        /// pre-multiplied alpha color pixels
        /// </summary>
        public int[] Pixels { get; private set; }

        public bool IsEmpty { get { return Pixels == null; } }
    }
    public struct Color4
    {
        public float r;
        public float g;
        public float b;
        public float a;
        public Color4(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public float Blue
        {
            get { return b; }
            set
            {
                this.b = value;
            }
        }
        public float Green
        {
            get { return g; }
            set
            {
                this.g = value;
            }
        }
        public float Red
        {
            get { return r; }
            set
            {
                this.r = value;
            }
        }
        public float Alpha
        {
            get { return a; }
            set
            {
                this.a = value;
            }
        }
        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="value">The factor by which to scale the color.</param>
        /// <param name="scale">The color to scale.</param>
        /// <returns>The scaled color.</returns>
        public static Color4 operator *(Color4 value, float scale)
        {
            return new Color4(value.Red * scale, value.Green * scale, value.Blue * scale, value.Alpha * scale);
        }

        /// <summary>
        /// Modulates two colors.
        /// </summary>
        /// <param name="left">The first color to modulate.</param>
        /// <param name="right">The second color to modulate.</param>
        /// <returns>The modulated color.</returns>
        public static Color4 operator *(Color4 left, Color4 right)
        {
            return new Color4(left.Red * right.Red, left.Green * right.Green, left.Blue * right.Blue, left.Alpha * right.Alpha);
        }
    }
}
