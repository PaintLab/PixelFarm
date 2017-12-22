//MIT, 2016-2017, WinterDev
//-----------------------------------  
using System;
using Typography.Contours;

using PixelFarm.Drawing;
using PixelFarm.Agg;
using PixelFarm.Drawing.Fonts;
using Typography.TextServices;
using PixelFarm;
using PixelFarm.Agg.Transform;

namespace Typography.Rendering
{
    /// <summary>
    /// agg glyph texture generator
    /// </summary>
    public class AggGlyphTextureGen
    {

        public AggGlyphTextureGen()
        {

        }
        public GlyphImage CreateGlyphImage(GlyphPathBuilder builder, float pxscale)
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

            if (dx != 0 || dy != 0)
            {
                Affine transformMat = Affine.NewTranslation(dx, dy);
                VertexStore vxs2 = new VertexStore();
                glyphVxs.TranslateToNewVxs(dx, dy, vxs2);
                glyphVxs = vxs2;
                w = (int)Math.Ceiling(w + dx);
                h = (int)Math.Ceiling(h + dy);
            }
            //-------------------------------------------- 
            //create glyph img 

            bool useLcdFontEffect = false;

            if (useLcdFontEffect)
            {
                w *= 4;
            }

            ActualImage img = new ActualImage(w, h, PixelFormat.ARGB32);
            AggRenderSurface aggsx = new AggRenderSurface(img);
            AggPainter painter = new AggPainter(aggsx);
            //we use white glyph on black bg for this texture                
            painter.Clear(Color.Black); //fill with black
            painter.FillColor = Color.White;
            painter.StrokeColor = Color.White;
            //--------------------------------------------  
            painter.UseSubPixelRendering = useLcdFontEffect;
            //------------------------------------- -------  


            //-------------------------------------------- 
            painter.Fill(glyphVxs);
            //-------------------------------------------- 
            var glyphImage = new GlyphImage(w, h);
            glyphImage.TextureOffsetX = dx;
            glyphImage.TextureOffsetY = dy;
            glyphImage.SetImageBuffer(ActualImage.CopyImgBuffer(img), false);
            //copy data from agg canvas to glyph image

            return glyphImage;
        }
    }
}