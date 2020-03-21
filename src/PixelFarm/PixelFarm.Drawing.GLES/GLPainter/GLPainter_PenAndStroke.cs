//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.CpuBlit.BitmapAtlas;

namespace PixelFarm.DrawingGL
{
    partial class GLPainter
    {
        Color _strokeColor;
        Pen _currentPen;
        Stroke _stroke = new Stroke(1);
        SimpleRectBorderBuilder _simpleBorderRectBuilder = new SimpleRectBorderBuilder();
        LineDashGenerator _lineDashGen;

        float[] _reuseableRectBordersXYs = new float[16];

        public override Pen CurrentPen
        {
            get => _currentPen;
            set => _currentPen = value;
        }
        public override Color StrokeColor
        {
            get => _strokeColor;
            set
            {
                _strokeColor = value;
                _pcx.StrokeColor = value;
            }
        }

        public override double StrokeWidth
        {
            get => _pcx.StrokeWidth;
            set
            {
                _pcx.StrokeWidth = (float)value;
                _stroke.Width = (float)value;
            }
        }

        bool _dashGenV2 = true;

        /// <summary>
        /// we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Draw(VertexStore vxs)
        {
            if (StrokeWidth > 1)
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                using (VectorToolBox.Borrow(out Stroke stroke))
                {

                    if (_lineDashGen == null)
                    {
                        //convert large stroke to vxs

                        stroke.Width = StrokeWidth;
                        stroke.MakeVxs(vxs, v1);

                        Color prevColor = this.FillColor;
                        FillColor = this.StrokeColor;
                        Fill(v1);
                        FillColor = prevColor;
                    }
                    else
                    {

                        _lineDashGen.CreateDash(vxs, v1);
                        if (_dashGenV2 && _lineDashGen.IsPrebuiltPattern)
                        {
                            //generate texture backend for a dash pattern
                            //similar to text glyph
                            //we can cache the bmp pattern for later use too 
                            float[] prebuilt_patt = _lineDashGen.GetPrebuiltPattern();

                            //start with solid
                            //in this version we use software renderer to create a 
                            //dash pattern 

                            SimpleBitmapAtlasBuilder _bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
                            for (int i = 0; i < prebuilt_patt.Length;)
                            {
                                int y = 4; // 
                                int x = 4; //

                                int w = (int)Math.Ceiling(prebuilt_patt[i] + (x * 2));
                                int h = (int)Math.Ceiling(StrokeWidth + (y * 2));

                                using (MemBitmap tmpBmp = new MemBitmap(w, h))
                                using (AggPainterPool.Borrow(tmpBmp, out AggPainter p))
                                {
                                    p.Clear(Color.Black);
                                    p.StrokeColor = Color.White;
                                    p.StrokeWidth = this.StrokeWidth;
                                    p.Line(x, y, x + prebuilt_patt[i], y, Color.White);

                                    var bmpAtlasItemSrc = new BitmapAtlasItemSource(w, h);
                                    bmpAtlasItemSrc.TextureXOffset = x;
                                    bmpAtlasItemSrc.TextureYOffset = y;
                                    bmpAtlasItemSrc.UniqueInt16Name = (ushort)i;
                                    bmpAtlasItemSrc.SetImageBuffer(MemBitmap.CopyImgBuffer(tmpBmp));
                                    _bmpAtlasBuilder.AddItemSource(bmpAtlasItemSrc);
                                }
                                i += 2;

                            }
                            //save bmp to test

                            //create bitmap atlas  
                            MemBitmap memBmp = _bmpAtlasBuilder.BuildSingleImage(false);
                            SimpleBitmapAtlas simpleBmpAtlas = _bmpAtlasBuilder.CreateSimpleBitmapAtlas();
#if DEBUG
                            //memBmp.SaveImage("dash_test.png");
#endif


                            //now generate vbo for line
                            //we walk along to
                            int n = v1.Count;
                            double px = 0, py = 0;

                            var vboBuilder = new TextureCoordVboBuilder();
                            vboBuilder.SetTextureInfo(memBmp.Width, memBmp.Height, false, RenderSurfaceOrientation.LeftTop);

                            ushort patNo = 0;
                            for (int i = 0; i < n; ++i)
                            {
                                VertexCmd cmd = v1.GetVertex(i, out double x, out double y);
                                switch (cmd)
                                {
                                    case VertexCmd.MoveTo:

                                        break;
                                    case VertexCmd.LineTo:
                                        {
                                            //select proper img for this segment
                                            simpleBmpAtlas.TryGetItem(patNo, out AtlasItem atlasItem);

                                            var srcRect = new Rectangle(atlasItem.Left + 4, atlasItem.Top + 4, atlasItem.Width - 8, atlasItem.Height - 8);

                                            //vboBuilder.WriteRect(ref srcRect, (float)x, (float)y, 1);
                                            //degree

                                            vboBuilder.WriteRect(ref srcRect, (float)x, (float)y, 1, (float)Math.Atan2(y - py, x - px));

                                            patNo += 2;
                                            if (patNo >= prebuilt_patt.Length)
                                            {
                                                patNo = 0;//reset
                                            }
                                        }
                                        break;
                                }
                                px = x;
                                py = y;
                            }

                            vboBuilder.AppendDegenerativeTrinagle();
                            GLBitmap glBmp = new GLBitmap(memBmp);

                            _pcx.FontFillColor = Color.Red;

                            _pcx.DrawGlyphImageWithStecil_VBO(glBmp, vboBuilder);
                            glBmp.Dispose();

                        }
                        else
                        {

                            int n = v1.Count;
                            double px = 0, py = 0;

                            LineDashGenerator tmp = _lineDashGen;
                            _lineDashGen = null;

                            for (int i = 0; i < n; ++i)
                            {
                                VertexCmd cmd = v1.GetVertex(i, out double x, out double y);
                                switch (cmd)
                                {
                                    case VertexCmd.MoveTo:

                                        break;
                                    case VertexCmd.LineTo:
                                        this.DrawLine(px, py, x, y);
                                        break;
                                }
                                px = x;
                                py = y;
                            }
                            _lineDashGen = tmp;
                        }
                    }

                }
            }
            else
            {
                //?
                using (PathRenderVx vx = PathRenderVx.Create(_pathRenderVxBuilder.Build(vxs)))
                {
                    _pcx.DrawGfxPath(_strokeColor, vx);
                }
            }
        }

        public override void DrawEllipse(double left, double top, double width, double height)
        {
            double x = (left + width / 2);
            double y = (top + height / 2);
            double rx = Math.Abs(width / 2);
            double ry = Math.Abs(height / 2);

            using (VectorToolBox.Borrow(out Ellipse ellipse))
            using (VxsTemp.Borrow(out var v1, out var v2))
            {
                ellipse.Set(x, y, rx, ry);

                ellipse.MakeVxs(v1);
                _stroke.MakeVxs(v1, v2);
                //***
                //we fill the stroke's path
                using (PathRenderVx vx = PathRenderVx.Create(_pathRenderVxBuilder.Build(v2)))
                {
                    _pcx.FillGfxPath(_strokeColor, vx);
                }
            }


        }

        public override void DrawRect(double left, double top, double width, double height)
        {
            switch (_pcx.SmoothMode)
            {
                case SmoothMode.Smooth:
                    {
                        _pcx.StrokeColor = this.StrokeColor;
                        using (PixelFarm.Drawing.VxsTemp.Borrow(out Drawing.VertexStore v1))
                        using (PixelFarm.Drawing.VectorToolBox.Borrow(out CpuBlit.VertexProcessing.SimpleRect r))
                        {
                            r.SetRect(left + 0.5f, top + height + 0.5f, left + width - 0.5f, top - 0.5f);
                            r.MakeVxs(v1);
                            Draw(v1);
                        }
                    }
                    break;
                default:
                    {
                        //draw boarder with
                        if (StrokeWidth > 0 && StrokeColor.A > 0)
                        {
                            _simpleBorderRectBuilder.SetBorderWidth((float)StrokeWidth);
                            //_simpleBorderRectBuilder.BuildAroundInnerRefBounds(
                            //    (float)left, (float)top + (float)height, (float)left + (float)width, (float)top,
                            //    _reuseableRectBordersXYs);
                            _simpleBorderRectBuilder.BuildAroundInnerRefBounds(
                               (float)left, (float)top, (float)width, (float)height,
                               _reuseableRectBordersXYs);
                            //
                            _pcx.FillTessArea(StrokeColor,
                                _reuseableRectBordersXYs,
                                _simpleBorderRectBuilder.GetPrebuiltRectTessIndices());
                        }
                    }
                    break;
            }
        }
        //

        public override void DrawRenderVx(RenderVx renderVx)
        {
            _pcx.DrawRenderVx(_strokeColor, renderVx);
        }
        public override void DrawLine(double x1, double y1, double x2, double y2)
        {
            _pcx.StrokeColor = _strokeColor;
            if (_lineDashGen == null)
            {
                _pcx.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
            }
            else
            {
                //TODO: line dash pattern cache
                _pcx.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
            }
        }

        public override LineJoin LineJoin
        {
            get => _stroke.LineJoin;
            set => _stroke.LineJoin = value;
        }
        public override LineCap LineCap
        {
            get => _stroke.LineCap;
            set => _stroke.LineCap = value;
        }

        public override IDashGenerator LineDashGen
        {
            get => _lineDashGen;
            set => _lineDashGen = (LineDashGenerator)value;
        }

        public void DrawCircle(float centerX, float centerY, double radius)
        {
            DrawEllipse(centerX - radius, centerY - radius, radius + radius, radius + radius);
        }

    }
}