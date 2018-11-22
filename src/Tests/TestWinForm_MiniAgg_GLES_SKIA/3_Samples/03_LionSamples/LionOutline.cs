//BSD, 2014-present, WinterDev

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
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.CpuBlit.Imaging;
using PixelFarm.CpuBlit.PixelProcessing;
using PixelFarm.CpuBlit.Rasterization;
using PixelFarm.CpuBlit.Rasterization.Lines;
using PaintLab.Svg;
using Mini;
namespace PixelFarm.CpuBlit.Sample_LionOutline
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
        LionOutlineSprite _lionOutlineSprite;
        public override void Init()
        {
            _lionOutlineSprite = new LionOutlineSprite();
        }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            _lionOutlineSprite.Render(p);

        }
        public override void MouseDrag(int x, int y)
        {
            _lionOutlineSprite.Move(x, y);
        }

        [DemoConfig]
        public bool RenderAsScanline
        {
            get
            {
                return this._lionOutlineSprite.RenderAsScanline;
            }
            set
            {
                this._lionOutlineSprite.RenderAsScanline = value;
            }
        }

        [DemoConfig]
        public bool RenderAccurateJoins
        {
            get
            {
                return this._lionOutlineSprite.RenderAccurateJoins;
            }
            set
            {
                this._lionOutlineSprite.RenderAccurateJoins = value;
            }
        }

        [DemoConfig]
        public bool UseBitmapExt
        {
            get { return _lionOutlineSprite.UseBitmapExt; }
            set
            {
                _lionOutlineSprite.UseBitmapExt = value;
            }
        }
        [DemoConfig]
        public bool UseBuiltInAggOutlineAATech
        {
            get { return _lionOutlineSprite.UseBuiltInAggOutlineAATech; }
            set
            {
                _lionOutlineSprite.UseBuiltInAggOutlineAATech = value;
            }
        }
    }
    //--------------------------------------------------
    public class LionOutlineSprite : BasicSprite
    {
        private SpriteShape _spriteShape;
        //special option 
        public LionOutlineSprite()
        {
            _spriteShape = new SpriteShape(VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\lion.svg").VgRootElem);
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


        public bool UseBitmapExt
        {
            get;
            set;
        }
        public bool UseBuiltInAggOutlineAATech { get; set; }

        public override void Render(PixelFarm.Drawing.Painter p)
        {


            int strokeWidth = 1;
            int width = p.Width;
            int height = p.Height;

            Affine affTx = Affine.NewMatix(
                   AffinePlan.Translate(-_spriteShape.Center.x, -_spriteShape.Center.y),
                   AffinePlan.Scale(_spriteScale, _spriteScale),
                   AffinePlan.Rotate(_angle + Math.PI),
                   AffinePlan.Skew(_skewX / 1000.0, _skewY / 1000.0),
                   AffinePlan.Translate(width / 2, height / 2));

            var p1 = p as AggPainter;
            if (p1 == null)
            {
                //TODO: review here 
                _spriteShape.Paint(p, affTx);
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
                p.RenderQuality = Drawing.RenderQuality.Fast;
                p.Clear(Drawing.Color.White);
                p.StrokeWidth = 1;

                //-------------------------
                _spriteShape.DrawOutline(p);
            }
            else
            {
                p.RenderQuality = Drawing.RenderQuality.HighQuality;
            }


            //-----------------------
            AggRenderSurface aggsx = p1.RenderSurface;
            //-----------------------
            //TODO: make this reusable ...
            //
            SubBitmapBlender widgetsSubImage = BitmapBlenderExtension.CreateSubBitmapBlender(aggsx.DestBitmapBlender, aggsx.GetClippingRect());
            SubBitmapBlender clippedSubImage = new SubBitmapBlender(widgetsSubImage, new PixelBlenderBGRA());
            ClipProxyImage imageClippingProxy = new ClipProxyImage(clippedSubImage);
            imageClippingProxy.Clear(PixelFarm.Drawing.Color.White);

            AggPainter aggPainter = (AggPainter)p;

            if (RenderAsScanline)
            {
                //a low-level example, expose scanline rasterizer

                ScanlineRasterizer rasterizer = aggsx.ScanlineRasterizer;
                rasterizer.SetClipBox(0, 0, width, height);

                //lionShape.ApplyTransform(affTx); 
                //---------------------

                using (VgPainterArgsPool.Borrow(aggPainter, out VgPaintArgs paintArgs))
                {
                    paintArgs._currentTx = affTx;
                    paintArgs.PaintVisitHandler = (vxs, painterA) =>
                    {
                        //use external painter handler
                        //draw only outline with its fill-color. 
                        rasterizer.Reset();
                        rasterizer.AddPath(vxs);
                        aggsx.BitmapRasterizer.RenderWithColor(
                            imageClippingProxy, rasterizer,
                            aggsx.ScanlinePacked8,
                            aggPainter.FillColor); //draw line with external drawing handler
                    };

                    _spriteShape.Paint(paintArgs);
                }

                //---------------------------- 
                //lionShape.ResetTransform();
            }
            else
            {

                if (UseBuiltInAggOutlineAATech)
                {
                    aggPainter.StrokeWidth = 1;
                    aggPainter.LineRenderingTech = LineRenderingTechnique.OutlineAARenderer;

                    //------
                    using (VgPainterArgsPool.Borrow(aggPainter, out VgPaintArgs paintArgs))
                    {
                        paintArgs._currentTx = affTx;
                        paintArgs.PaintVisitHandler = (vxs, painterA) =>
                        {
                            //use external painter handler
                            //draw only outline with its fill-color.
                            Drawing.Painter m_painter = paintArgs.P;
                            Drawing.Color prevStrokeColor = m_painter.StrokeColor;
                            m_painter.StrokeColor = m_painter.FillColor;
                            m_painter.Draw(vxs);
                            m_painter.StrokeColor = prevStrokeColor;
                        };
                        _spriteShape.Paint(paintArgs);
                    }
                }
                else
                {
                    //low-level implementation
                    aggPainter.StrokeWidth = 1;

                    //-------------------------                
                    //Draw with LineProfile: 
                    //LineProfileAnitAlias lineProfile = new LineProfileAnitAlias(strokeWidth * affTx.GetScale(), new GammaNone()); //with gamma

                    LineProfileAnitAlias lineProfile = new LineProfileAnitAlias(strokeWidth * affTx.GetScale(), null);
                    OutlineRenderer outlineRenderer = new OutlineRenderer(imageClippingProxy, new PixelBlenderBGRA(), lineProfile);
                    outlineRenderer.SetClipBox(0, 0, this.Width, this.Height);

                    OutlineAARasterizer rasterizer = new OutlineAARasterizer(outlineRenderer);
                    rasterizer.LineJoin = (RenderAccurateJoins ?
                        OutlineAARasterizer.OutlineJoin.AccurateJoin
                        : OutlineAARasterizer.OutlineJoin.Round);
                    rasterizer.RoundCap = true;

                    //lionShape.ApplyTransform(affTx);
                    //----------------------------
                    using (VgPainterArgsPool.Borrow(aggPainter, out VgPaintArgs paintArgs))
                    {
                        paintArgs._currentTx = affTx;
                        paintArgs.PaintVisitHandler = (vxs, painterA) =>
                        {
                            //use external painter handler
                            //draw only outline with its fill-color.
                            rasterizer.RenderVertexSnap(
                                vxs,
                                painterA.P.FillColor);
                        };

                        _spriteShape.Paint(paintArgs);
                    }

                    //----------------------------  
                    //lionShape.ResetTransform();  
                }
            }

        }
    }
}
