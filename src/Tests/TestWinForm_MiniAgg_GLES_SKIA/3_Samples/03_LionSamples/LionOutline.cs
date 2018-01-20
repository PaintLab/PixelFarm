//BSD, 2014-2018, WinterDev

/*
Copyright (c) 2013, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using System;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.Imaging;
using PixelFarm.Agg.Lines;
using Mini;
namespace PixelFarm.Agg.Sample_LionOutline
{
    [Info(OrderCode = "03")]
    [Info("The example demonstrates Maxim's algorithm of drawing Anti-Aliased lines. " +
            "The algorithm works about 2.5 times faster than the scanline rasterizer but has" +
            " some restrictions, particularly, line joins can be only of the �miter� type, " +
            "and when so called miter limit is exceded, they are not as accurate as generated " +
            "by the stroke converter (conv_stroke). To see the difference, maximize the window" +
            " and try to rotate and scale the �lion� with and without using the scanline " +
            "rasterizer (a checkbox at the bottom). The difference in performance is obvious.")]
    public class LionFillOutlineExample : DemoBase
    {
        LionOutlineSprite lionFill;
        public override void Init()
        {
            lionFill = new LionOutlineSprite();
        }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            lionFill.Draw(p);

        }
        public override void MouseDrag(int x, int y)
        {
            lionFill.Move(x, y);
        }

        [DemoConfig]
        public bool RenderAsScanline
        {
            get
            {
                return this.lionFill.RenderAsScanline;
            }
            set
            {
                this.lionFill.RenderAsScanline = value;
            }
        }

        [DemoConfig]
        public bool RenderAccurateJoins
        {
            get
            {
                return this.lionFill.RenderAccurateJoins;
            }
            set
            {
                this.lionFill.RenderAccurateJoins = value;
            }
        }

        [DemoConfig]
        public bool UseBitmapExt
        {
            get { return lionFill.UseBitmapExt; }
            set
            {
                lionFill.UseBitmapExt = value;
            }
        }

    }
    //--------------------------------------------------
    public class LionOutlineSprite : BasicSprite
    {
        private SpriteShape lionShape;
        //special option 
        public LionOutlineSprite()
        {
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            this.Width = 500;
            this.Height = 500;
        }
        void NeedsRedraw(object sender, EventArgs e)
        {
        }

        public bool RenderAsScanline
        {
            get;
            set;
        }
        public bool RenderAccurateJoins
        {
            get;
            set;
        }

        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }

        Stroke stroke1 = new Stroke(1);

        public override void Draw(PixelFarm.Drawing.Painter p)
        {


            int strokeWidth = 1;
            int width = p.Width;
            int height = p.Height;

            Affine affTx = Affine.NewMatix(
                   AffinePlan.Translate(-lionShape.Center.x, -lionShape.Center.y),
                   AffinePlan.Scale(spriteScale, spriteScale),
                   AffinePlan.Rotate(angle + Math.PI),
                   AffinePlan.Skew(skewX / 1000.0, skewY / 1000.0),
                   AffinePlan.Translate(width / 2, height / 2));

            var p1 = p as AggPainter;
            if (p1 == null)
            {
                //TODO: review here

                lionShape.Paint(p, affTx);

                //int j = lionShape.NumPaths;
                //int[] pathList = lionShape.PathIndexList; 

                //Drawing.Color[] colors = lionShape.Colors;
                ////graphics2D.UseSubPixelRendering = true; 
                //var vxs = GetFreeVxs();
                //affTx.TransformToVxs(lionShape.Vxs, vxs);


                //p.StrokeWidth = 1;
                //for (int i = 0; i < j; ++i)
                //{
                //    p.StrokeColor = colors[i];
                //    p.Draw(new PixelFarm.Drawing.VertexStoreSnap(vxs, pathList[i]));

                //}
                ////not agg   
                //Release(ref vxs);
                //return; //**
            }


            if (UseBitmapExt)
            {
                p.RenderQuality = Drawing.RenderQualtity.Fast;
                p.Clear(Drawing.Color.White);
                p.StrokeWidth = 1;

                //-------------------------
                lionShape.DrawOutline(p);

               

            }
            else
            {
                p.RenderQuality = Drawing.RenderQualtity.HighQuality;
            }




            //-----------------------
            AggRenderSurface aggsx = p1.RenderSurface;
            //var widgetsSubImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRect());
            //int width = widgetsSubImage.Width;
            //int height = widgetsSubImage.Height; 

            SubImageRW widgetsSubImage = ImageHelper.CreateSubImgRW(aggsx.DestImage, aggsx.GetClippingRect());
            SubImageRW clippedSubImage = new SubImageRW(widgetsSubImage, new PixelBlenderBGRA());
            ClipProxyImage imageClippingProxy = new ClipProxyImage(clippedSubImage);
            imageClippingProxy.Clear(PixelFarm.Drawing.Color.White);


            if (RenderAsScanline)
            {
                var rasterizer = aggsx.ScanlineRasterizer;
                rasterizer.SetClipBox(0, 0, width, height);
                //Stroke stroke = new Stroke(strokeWidth);
                //stroke.LineJoin = LineJoin.Round;
                var vxs = GetFreeVxs();
                affTx.TransformToVxs(lionShape.Vxs, vxs);
                ScanlineRasToDestBitmapRenderer sclineRasToBmp = aggsx.ScanlineRasToDestBitmap;
                sclineRasToBmp.RenderSolidAllPaths(
                    imageClippingProxy,
                    rasterizer,
                    aggsx.ScanlinePacked8,
                    vxs,
                    lionShape.Colors,
                    lionShape.PathIndexList,
                    lionShape.NumPaths);
                Release(ref vxs);
            }
            else
            {

                //LineProfileAnitAlias lineProfile = new LineProfileAnitAlias(strokeWidth * affTx.GetScale(), new GammaNone());
                LineProfileAnitAlias lineProfile = new LineProfileAnitAlias(strokeWidth * affTx.GetScale(), null);
                OutlineRenderer outlineRenderer = new OutlineRenderer(imageClippingProxy, new PixelBlenderBGRA(), lineProfile);
                OutlineAARasterizer rasterizer = new OutlineAARasterizer(outlineRenderer);
                rasterizer.LineJoin = (RenderAccurateJoins ?
                    OutlineAARasterizer.OutlineJoin.AccurateJoin
                    : OutlineAARasterizer.OutlineJoin.Round);
                rasterizer.RoundCap = true;
                //VertexSourceApplyTransform trans = new VertexSourceApplyTransform(lionShape.Path, transform);
                var vxs = GetFreeVxs();
                affTx.TransformToVxs(lionShape.Vxs, vxs);// trans.DoTransformToNewVxStorage();
                int j = lionShape.NumPaths;
                for (int i = 0; i < j; ++i)
                {
                    rasterizer.RenderVertexSnap(
                        new PixelFarm.Drawing.VertexStoreSnap(vxs,
                            lionShape.PathIndexList[i]),
                            lionShape.Colors[i]);
                }
                Release(ref vxs);
            }
            base.Draw(p);
        }
    }
}
