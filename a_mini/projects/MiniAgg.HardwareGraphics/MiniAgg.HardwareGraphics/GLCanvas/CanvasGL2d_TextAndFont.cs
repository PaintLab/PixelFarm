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
             
        }

    }


}