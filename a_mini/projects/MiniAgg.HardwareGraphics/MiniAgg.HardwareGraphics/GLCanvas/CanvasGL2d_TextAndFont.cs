//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using OpenTK.Graphics.OpenGL;
using Tesselate;

using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;



namespace OpenTkEssTest
{
    partial class CanvasGL2d
    {
       
        
        void SetupFonts()
        {

        }
        public PixelFarm.Font2.FontFace CurrentFontFace
        {
            get { return this.textPriner.CurrentFontFace; }
            set { this.textPriner.CurrentFontFace = value; }
        }
        public void DrawString(string str, float x, float y)
        {
            //test bitmap 
            this.textPriner.Print(str.ToCharArray(), x, y);

            //test----------------------------------
            //myGraphicsPath.AddString(str,
            //    FontFamily.GenericSerif,
            //    0,120,
            //    new Point(0, 0), new StringFormat(StringFormatFlags.NoClip));
            //myGraphicsPath.Flatten();
            ////the get path points

            //PointF[] points = myGraphicsPath.PathPoints;
            //int j = myGraphicsPath.PointCount;
            //float[] coords = new float[j * 2];
            //int nn = 0;
            //for (int i = 0; i < j; ++i)
            //{
            //    PointF p = points[i];
            //    coords[nn] = p.X;
            //    coords[nn + 1] = p.Y;
            //    nn += 2;
            //}
            //var pathData = myGraphicsPath.PathData;
            //DrawPolygon(coords, j * 2);
            //test----------------------------------
        }

    }


}