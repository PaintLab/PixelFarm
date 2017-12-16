//MIT, 2009-2015, Rene Schulte and WriteableBitmapEx Contributors, https://github.com/teichgraf/WriteableBitmapEx
//
//
//   Project:           WriteableBitmapEx - WriteableBitmap extensions
//   Description:       Collection of extension methods for the WriteableBitmap class.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2015-03-05 18:18:24 +0100 (Do, 05 Mrz 2015) $
//   Changed in:        $Revision: 113191 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/trunk/Source/WriteableBitmapEx/WriteableBitmapBaseExtensions.cs $
//   Id:                $Id: WriteableBitmapBaseExtensions.cs 113191 2015-03-05 17:18:24Z unknown $
//
//
//   Copyright © 2009-2015 Rene Schulte and WriteableBitmapEx Contributors
//
//   This code is open source. Please read the License.txt for details. No worries, we won't sue you! ;)
//


using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
namespace WinFormGdiPlus
{
    public partial class FormShape : Form
    {

        private int shapeCount;
        private static Random rand = new Random();
        private int frameCounter = 0;

        public FormShape()
        {
            InitializeComponent();
            //setup

        }
        private void FormShape_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (Graphics g = this.panel1.CreateGraphics())
            using (Bitmap bmp1 = new Bitmap(400, 500))
            using (var bmplock = bmp1.Lock())
            {
                WriteableBitmap wb = bmplock.GetWritableBitmap();
                DrawStaticShapes(wb);
                bmplock.WriteAndUnlock();
                //

                g.Clear(System.Drawing.Color.White);
                g.DrawImage(bmp1, 0, 0);
            }
        }
        /// <summary>
        /// Random color fully opaque
        /// </summary>
        /// <returns></returns>
        private static int GetRandomColor()
        {
            return (int)(0xFF000000 | (uint)rand.Next(0xFFFFFF));
        }

        /// <summary>
        /// Draws the different types of shapes.
        /// </summary>
        private void DrawStaticShapes(WriteableBitmap writeableBmp)
        {
            // Wrap updates in a GetContext call, to prevent invalidation and nested locking/unlocking during this block
            using (writeableBmp.GetBitmapContext())
            {
                // Init some size vars
                int w = writeableBmp.PixelWidth - 2;
                int h = writeableBmp.PixelHeight - 2;
                int w3rd = w / 3;
                int h3rd = h / 3;
                int w6th = w3rd >> 1;
                int h6th = h3rd >> 1;

                // Clear 
                writeableBmp.Clear();

                // Draw some points
                for (int i = 0; i < 200; i++)
                {
                    writeableBmp.SetPixel(rand.Next(w3rd), rand.Next(h3rd), GetRandomColor());
                }

                // Draw Standard shapes
                writeableBmp.DrawLine(rand.Next(w3rd, w3rd * 2), rand.Next(h3rd), rand.Next(w3rd, w3rd * 2), rand.Next(h3rd),
                                      GetRandomColor());
                writeableBmp.DrawTriangle(rand.Next(w3rd * 2, w - w6th), rand.Next(h6th), rand.Next(w3rd * 2, w),
                                          rand.Next(h6th, h3rd), rand.Next(w - w6th, w), rand.Next(h3rd),
                                          GetRandomColor());

                writeableBmp.DrawQuad(rand.Next(0, w6th), rand.Next(h3rd, h3rd + h6th), rand.Next(w6th, w3rd),
                                      rand.Next(h3rd, h3rd + h6th), rand.Next(w6th, w3rd),
                                      rand.Next(h3rd + h6th, 2 * h3rd), rand.Next(0, w6th), rand.Next(h3rd + h6th, 2 * h3rd),
                                      GetRandomColor());
                writeableBmp.DrawRectangle(rand.Next(w3rd, w3rd + w6th), rand.Next(h3rd, h3rd + h6th),
                                           rand.Next(w3rd + w6th, w3rd * 2), rand.Next(h3rd + h6th, 2 * h3rd),
                                           GetRandomColor());

                // Random polyline
                int[] p = new int[rand.Next(7, 10) * 2];
                for (int j = 0; j < p.Length; j += 2)
                {
                    p[j] = rand.Next(w3rd * 2, w);
                    p[j + 1] = rand.Next(h3rd, 2 * h3rd);
                }
                writeableBmp.DrawPolyline(p, GetRandomColor());

                // Random closed polyline
                p = new int[rand.Next(6, 9) * 2];
                for (int j = 0; j < p.Length - 2; j += 2)
                {
                    p[j] = rand.Next(w3rd);
                    p[j + 1] = rand.Next(2 * h3rd, h);
                }
                p[p.Length - 2] = p[0];
                p[p.Length - 1] = p[1];
                writeableBmp.DrawPolyline(p, GetRandomColor());

                // Ellipses
                writeableBmp.DrawEllipse(rand.Next(w3rd, w3rd + w6th), rand.Next(h3rd * 2, h - h6th),
                                         rand.Next(w3rd + w6th, w3rd * 2), rand.Next(h - h6th, h), GetRandomColor());
                writeableBmp.DrawEllipseCentered(w - w6th, h - h6th, w6th >> 1, h6th >> 1, GetRandomColor());

                System.Windows.Media.Imaging.Color black = System.Windows.Media.Imaging.Color.FromArgb(255, 0, 0, 0);
                // Draw Grid
                writeableBmp.DrawLine(0, h3rd, w, h3rd, black);
                writeableBmp.DrawLine(0, 2 * h3rd, w, 2 * h3rd, black);
                writeableBmp.DrawLine(w3rd, 0, w3rd, h, black);
                writeableBmp.DrawLine(2 * w3rd, 0, 2 * w3rd, h, black);

                // Invalidates on exit of using block
            }
        }


    }
}
