//MIT, 2016-2017, WinterDev
//-----------------------------------  
using System;
using Typography.Contours;

using PixelFarm.Drawing;
using PixelFarm.Agg;
using PixelFarm.Drawing.Fonts;

namespace Typography.Rendering
{
    /// <summary>
    /// agg glyph texture generator
    /// </summary>
    public class AggGlyphTextureGen
    {

        public AggGlyphTextureGen()
        {
            BackGroundColor = Color.Black;
            GlyphColor = Color.White;
        }

        public Color BackGroundColor { get; set; }
        public Color GlyphColor { get; set; }

        public GlyphImage CreateGlyphImage(GlyphPathBuilder builder, bool useLcdFontEffect, float pxscale)
        {
            //1. create  

            var txToVxs = new GlyphTranslatorToVxs();
            builder.ReadShapes(txToVxs);
            //
            //create new one
            var glyphVxs = new VertexStore();
            txToVxs.WriteOutput(glyphVxs, pxscale);
            //find bound
            //-------------------------------------------- 
            //GlyphImage glyphImg = new GlyphImage()
            RectD bounds = new RectD();
            BoundingRect.GetBoundingRect(new VertexStoreSnap(glyphVxs), ref bounds);

            ////-------------------------------------------- 
            int w = (int)System.Math.Ceiling(bounds.Width);
            int h = (int)System.Math.Ceiling(bounds.Height);
            if (w < 5)
            {
                w = 5;
            }
            if (h < 5)
            {
                h = 5;
            }
            ////translate to positive quadrant 
            double dx = (bounds.Left < 0) ? -bounds.Left : 0;
            double dy = (bounds.Bottom < 0) ? -bounds.Bottom : 0;

            //we need some borders
            int horizontal_margin = 1; //'margin' 1px
            int vertical_margin = 1; //margin 1 px

            dx += horizontal_margin; //+ left margin
            dy += vertical_margin; //+ top margin

            VertexStore vxs2 = new VertexStore();
            glyphVxs.TranslateToNewVxs(dx, dy, vxs2);
            glyphVxs = vxs2;

            //
            w = (int)Math.Ceiling(dx + w + horizontal_margin); //+right margin
            h = (int)Math.Ceiling(dy + h + vertical_margin); //+bottom margin

            //-------------------------------------------- 
            //create glyph img  


            if (useLcdFontEffect)
            {   

                w *= 3;// *** x3 than normal

                ActualImage img = new ActualImage(w, h, PixelFormat.ARGB32);
                AggRenderSurface aggsx = new AggRenderSurface(img);
                AggPainter painter = new AggPainter(aggsx);
                painter.UseSubPixelRendering = true;


                //we use white glyph on black bg for this texture                
                painter.Clear(Color.Black);
                painter.FillColor = Color.White;
                painter.Fill(glyphVxs);

                //
                var glyphImage = new GlyphImage(w / 3, h);
                glyphImage.TextureOffsetX = dx;
                glyphImage.TextureOffsetY = dy;
                glyphImage.SetImageBuffer(ActualImageExtensions.CopyImgBuffer(img, w / 3), false);
                //copy data from agg canvas to glyph image 
                return glyphImage;

            }
            else
            {
                ActualImage img = new ActualImage(w, h, PixelFormat.ARGB32);
                AggRenderSurface aggsx = new AggRenderSurface(img);
                AggPainter painter = new AggPainter(aggsx);
                painter.UseSubPixelRendering = false;

                painter.Clear(BackGroundColor);
                painter.FillColor = GlyphColor;
                painter.Fill(glyphVxs);
                //

                var glyphImage = new GlyphImage(w, h);
                glyphImage.TextureOffsetX = dx;
                glyphImage.TextureOffsetY = dy;
                glyphImage.SetImageBuffer(ActualImage.CopyImgBuffer(img), false);
                //copy data from agg canvas to glyph image 
                return glyphImage;
            }


        }
    }
}