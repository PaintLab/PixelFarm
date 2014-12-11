//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

 
using System.Text;
using OpenTK.Graphics.OpenGL;
using Tesselate;

using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;



namespace LayoutFarm.DrawingGL
{
    partial class CanvasGL2d
    { 
        void SetupFonts()
        { 
        }
        public PixelFarm.Agg.Fonts.Font CurrentFont 
        {
            get { return this.textPriner.CurrentFont; }
            set { this.textPriner.CurrentFont = value; }
        }
        public void DrawString(string str, float x, float y)
        {
            //test bitmap 
            this.textPriner.Print(str.ToCharArray(), x, y);

        }

    }


}