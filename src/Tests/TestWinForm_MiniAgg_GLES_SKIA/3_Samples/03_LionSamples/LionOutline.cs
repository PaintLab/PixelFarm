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
using PixelFarm;
using PixelFarm.Agg;
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
            lionShape = new SpriteShape(SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\lion.svg"));
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


        void DrawAsScanline(ClipProxyImage imageClippingProxy,
            AggRenderSurface aggsx,
            ScanlineRasterizer rasterizer,
            ScalineRasToDestinationBitmap sclineRasToBmp)
        {
            SvgRenderVx renderVx = lionShape.GetRenderVx();
            int num_paths = renderVx.SvgVxCount;

            for (int i = 0; i < num_paths; ++i)
            {
                rasterizer.Reset();
                SvgPart svgPart = renderVx.GetInnerVx(i);

                switch (svgPart.Kind)
                {
                    case SvgRenderVxKind.Path:
                        {
                            rasterizer.AddPath(new PixelFarm.Drawing.VertexStoreSnap(svgPart.GetVxs(), 0));
                            sclineRasToBmp.RenderWithColor(imageClippingProxy, rasterizer, aggsx.ScanlinePacked8, new Drawing.Color(255, 0, 0));
                        }
                        break;
                }

            }
        }
        struct TempRenderState
        {
            public float strokeWidth;
            public PixelFarm.Drawing.Color strokeColor;
            public PixelFarm.Drawing.Color fillColor;
            public PixelFarm.Agg.Transform.Affine affineTx;
        }


        void DrawWithLineProfile(OutlineAARasterizer rasterizer)
        {
            SvgRenderVx renderVx = lionShape.GetRenderVx();
            int num_paths = renderVx.SvgVxCount;

            var renderState = new TempRenderState();
            renderState.strokeColor = PixelFarm.Drawing.Color.Black;
            renderState.strokeWidth = 1;
            renderState.fillColor = PixelFarm.Drawing.Color.Black;
            renderState.affineTx = null;


            for (int i = 0; i < num_paths; ++i)
            {
                SvgPart vx = renderVx.GetInnerVx(i);
                switch (vx.Kind)
                {
                    case SvgRenderVxKind.BeginGroup:
                        {
                            ////1. save current state before enter new state 
                            //p.StackPushUserObject(renderState);

                            ////2. enter new px context
                            //if (vx.HasFillColor)
                            //{
                            //    p.FillColor = renderState.fillColor = vx.FillColor;
                            //}
                            //if (vx.HasStrokeColor)
                            //{
                            //    p.StrokeColor = renderState.strokeColor = vx.StrokeColor;
                            //}
                            //if (vx.HasStrokeWidth)
                            //{
                            //    p.StrokeWidth = renderState.strokeWidth = vx.StrokeWidth;
                            //}
                            //if (vx.AffineTx != null)
                            //{
                            //    //apply this to current tx
                            //    if (currentTx != null)
                            //    {
                            //        currentTx = currentTx * vx.AffineTx;
                            //    }
                            //    else
                            //    {
                            //        currentTx = vx.AffineTx;
                            //    }
                            //    renderState.affineTx = currentTx;
                            //}
                        }
                        break;
                    case SvgRenderVxKind.EndGroup:
                        {
                            ////restore to prev state
                            //renderState = (TempRenderState)p.StackPopUserObject();
                            //p.FillColor = renderState.fillColor;
                            //p.StrokeColor = renderState.strokeColor;
                            //p.StrokeWidth = renderState.strokeWidth;
                            //currentTx = renderState.affineTx;
                        }
                        break;
                    case SvgRenderVxKind.Path:
                        {
                            //temp
                            rasterizer.RenderVertexSnap(
                              new PixelFarm.Drawing.VertexStoreSnap(vx.GetVxs(), 0),
                              new Drawing.Color(255, 0, 0));
                        }
                        break;
                        //{

                        //    VertexStore vxs = vx.GetVxs();
                        //    if (vx.HasFillColor)
                        //    {
                        //        //has specific fill color
                        //        if (vx.FillColor.A > 0)
                        //        {
                        //            if (currentTx == null)
                        //            {
                        //                p.Fill(vxs, vx.FillColor);
                        //            }
                        //            else
                        //            {
                        //                //have some tx
                        //                tempVxs.Clear();
                        //                currentTx.TransformToVxs(vxs, tempVxs);
                        //                p.Fill(tempVxs, vx.FillColor);
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (p.FillColor.A > 0)
                        //        {
                        //            if (currentTx == null)
                        //            {
                        //                p.Fill(vxs);
                        //            }
                        //            else
                        //            {
                        //                //have some tx
                        //                tempVxs.Clear();
                        //                currentTx.TransformToVxs(vxs, tempVxs);
                        //                p.Fill(tempVxs);
                        //            }

                        //        }
                        //    }

                        //    if (p.StrokeWidth > 0)
                        //    {
                        //        //check if we have a stroke version of this render vx
                        //        //if not then request a new one 

                        //        VertexStore strokeVxs = GetStrokeVxsOrCreateNew(vx, p, (float)p.StrokeWidth);
                        //        if (vx.HasStrokeColor)
                        //        {
                        //            //has speciic stroke color 
                        //            p.StrokeWidth = vx.StrokeWidth;
                        //            if (currentTx == null)
                        //            {
                        //                p.Fill(strokeVxs, vx.StrokeColor);
                        //            }
                        //            else
                        //            {
                        //                //have some tx
                        //                tempVxs.Clear();
                        //                currentTx.TransformToVxs(strokeVxs, tempVxs);
                        //                p.Fill(tempVxs, vx.StrokeColor);
                        //            }

                        //        }
                        //        else if (p.StrokeColor.A > 0)
                        //        {
                        //            if (currentTx == null)
                        //            {
                        //                p.Fill(strokeVxs, p.StrokeColor);
                        //            }
                        //            else
                        //            {
                        //                tempVxs.Clear();
                        //                currentTx.TransformToVxs(strokeVxs, tempVxs);
                        //                p.Fill(tempVxs, p.StrokeColor);
                        //            }
                        //        }
                        //        else
                        //        {

                        //        }
                        //    }
                        //    else
                        //    {

                        //        if (vx.HasStrokeColor)
                        //        {
                        //            VertexStore strokeVxs = GetStrokeVxsOrCreateNew(vx, p, (float)p.StrokeWidth);
                        //            p.Fill(strokeVxs);
                        //        }
                        //        else if (p.StrokeColor.A > 0)
                        //        {
                        //            VertexStore strokeVxs = GetStrokeVxsOrCreateNew(vx, p, (float)p.StrokeWidth);
                        //            p.Fill(strokeVxs, p.StrokeColor);
                        //        }
                        //    }
                        //}
                        break;
                }
            }
        }
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
                ScanlineRasterizer rasterizer = aggsx.ScanlineRasterizer;
                rasterizer.SetClipBox(0, 0, width, height);
                //Stroke stroke = new Stroke(strokeWidth);
                //stroke.LineJoin = LineJoin.Round; 
                lionShape.ApplyTransform(affTx);


                DrawAsScanline(imageClippingProxy, aggsx, rasterizer, aggsx.ScanlineRasToDestBitmap);



                lionShape.ResetTransform();


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

                SvgRenderVx renderVx = lionShape.GetRenderVx();

                lionShape.ApplyTransform(affTx);

                DrawWithLineProfile(rasterizer);

                lionShape.ResetTransform();
            }
            base.Draw(p);
        }
    }
}
