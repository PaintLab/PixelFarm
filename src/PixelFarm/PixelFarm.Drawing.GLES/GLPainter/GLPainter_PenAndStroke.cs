//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using System.Collections.Generic;

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
        readonly Stroke _stroke = new Stroke(1);
        readonly SimpleRectBorderBuilder _simpleBorderRectBuilder = new SimpleRectBorderBuilder();
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

        readonly Dictionary<string, DashPatternBmpCache> _bmpDashPatternsCache = new Dictionary<string, DashPatternBmpCache>();
        readonly TextureCoordVboBuilder _vboBuilder = new TextureCoordVboBuilder();

        class DashPatternBmpCache : IDisposable
        {
            public SimpleBitmapAtlas Atlas;
            public GLBitmap glBmp;
            public void Dispose()
            {
                if (Atlas != null)
                {
                    Atlas.Dispose();
                    Atlas = null;
                }

                if (glBmp != null)
                {
                    glBmp.Dispose();
                    glBmp = null;
                }
            }
        }


        DashPatternBmpCache GetCacheBmpDashOrCreate()
        {

            //this is staic pattern 
            //check if we have a cache of its dash-pattern bitmap atlas
            //if not the create a new one
            string pattern_str = _lineDashGen.GetPatternAsString() + "w" + this.StrokeWidth;

            if (_bmpDashPatternsCache.TryGetValue(pattern_str, out DashPatternBmpCache found))
            {
                return found;
            }

            //start with solid
            //in this version we use software renderer to create a 
            //dash pattern 

            DashSegment[] prebuilt_patt = _lineDashGen.GetStaticDashSegments();
            SimpleBitmapAtlasBuilder _bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
            for (int i = 0; i < prebuilt_patt.Length; ++i)
            {
                int padding_X = 4; //
                int padding_Y = 4; // 

                DashSegment seg = prebuilt_patt[i];
                if (seg.IsSolid)
                {
                    int w = (int)Math.Ceiling(seg.Len + (padding_X * 2));
                    int h = (int)Math.Ceiling(StrokeWidth + (padding_Y * 2));

                    using (MemBitmap tmpBmp = new MemBitmap(w, h))
                    using (Tools.BorrowAggPainter(tmpBmp, out AggPainter p))
                    {
                        p.Clear(Color.Black);
                        p.StrokeColor = Color.White;
                        p.StrokeWidth = this.StrokeWidth;

                        p.Line(padding_X, padding_Y, padding_X + seg.Len, padding_Y, Color.White);

                        var bmpAtlasItemSrc = new BitmapAtlasItemSource(w, h);
                        bmpAtlasItemSrc.TextureXOffset = -padding_X;
                        bmpAtlasItemSrc.TextureYOffset = -padding_Y;
                        bmpAtlasItemSrc.UniqueInt16Name = (ushort)i;
                        bmpAtlasItemSrc.SetImageBuffer(MemBitmap.CopyImgBuffer(tmpBmp));
                        _bmpAtlasBuilder.AddItemSource(bmpAtlasItemSrc);
                    }
                }
            }
            //save bmp to test

            //create bitmap atlas              
            MemBitmap memBmp = _bmpAtlasBuilder.BuildSingleImage(false);
            SimpleBitmapAtlas simpleBmpAtlas = _bmpAtlasBuilder.CreateSimpleBitmapAtlas();
            simpleBmpAtlas.SetMainBitmap(memBmp, true);

            DashPatternBmpCache cache = new DashPatternBmpCache();
            cache.Atlas = simpleBmpAtlas;
            cache.glBmp = new GLBitmap(memBmp);
            _bmpDashPatternsCache.Add(pattern_str, cache);
            return cache;
        }

        readonly ArrayList<float> _vertexList = new ArrayList<float>();
        readonly ArrayList<ushort> _indexList = new ArrayList<ushort>();

        /// <summary>
        /// we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Draw(VertexStore vxs)
        {
            if (StrokeWidth > 1)
            {
                using (Tools.BorrowVxs(out var v1))
                using (Tools.BorrowStroke(out var stroke))
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
                        //TODO: cache?
                        ushort tempLineJoinRadius = _lineDashGen.LineJoinRadius;
                        //
                        _lineDashGen.LineJoinRadius = 0;//px
                        _lineDashGen.CreateDash(vxs, v1);//a set of line dash
                        _lineDashGen.LineJoinRadius = tempLineJoinRadius;

                        if (_dashGenV2 && _lineDashGen.IsStaticPattern)
                        {
                            //generate texture backend for a dash pattern
                            //similar to text glyph
                            //we can cache the bmp pattern for later use too 

                            DashSegment[] prebuilt_patt = _lineDashGen.GetStaticDashSegments();

                            DashPatternBmpCache dashPattBmpCache = GetCacheBmpDashOrCreate();

#if DEBUG
                            //memBmp.SaveImage("dash_test.png");
#endif

                            //now generate vbo for line
                            //we walk along to
                            int n = v1.Count;
                            double px = 0, py = 0;

                            GLBitmap glbmp = dashPattBmpCache.glBmp;
                            SimpleBitmapAtlas simpleBmpAtlas = dashPattBmpCache.Atlas;

                            _vertexList.Clear();
                            _indexList.Clear();
                            _vboBuilder.SetArrayLists(_vertexList, _indexList);
                            _vboBuilder.SetTextureInfo(glbmp.Width, glbmp.Height, false, RenderSurfaceOriginKind.LeftTop);

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
                                            var srcRect = new Rectangle(atlasItem.Left - (int)atlasItem.TextureXOffset, atlasItem.Top - (int)atlasItem.TextureYOffset, atlasItem.Width - 8, atlasItem.Height - 8);
                                            //please note that : we start rect at (px,py)
                                            _vboBuilder.WriteRectWithRotation(srcRect, (float)px, (float)py, (float)Math.Atan2(y - py, x - px));
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



                            //ushort patNo = 0;
                            //for (int i = 0; i < n; ++i)
                            //{
                            //    VertexCmd cmd = v1.GetVertex(i, out double x, out double y);
                            //    switch (cmd)
                            //    {
                            //        case VertexCmd.MoveTo:

                            //            break;
                            //        case VertexCmd.LineTo:
                            //            {
                            //                //select proper img for this segment

                            //                simpleBmpAtlas.TryGetItem(patNo, out AtlasItem atlasItem);
                            //                var srcRect = new Rectangle(atlasItem.Left - (int)atlasItem.TextureXOffset, atlasItem.Top - (int)atlasItem.TextureYOffset, atlasItem.Width - 8, atlasItem.Height - 8);
                            //                //please note that : we start rect at (px,py)
                            //                _vboBuilder.WriteRectWithRotation(srcRect, (float)px, (float)py, (float)Math.Atan2(y - py, x - px));
                            //                patNo += 2;
                            //                if (patNo >= prebuilt_patt.Length)
                            //                {
                            //                    patNo = 0;//reset
                            //                }
                            //            }
                            //            break;
                            //    }
                            //    px = x;
                            //    py = y;
                            //}

                            _vboBuilder.AppendDegenerativeTriangle();
                            Color prevColor = _pcx.FontFillColor;
                            _pcx.FontFillColor = this.StrokeColor;//***
                            _pcx.DrawGlyphImageWithStecil_VBO(glbmp, _vboBuilder); //TODO: review here. in this version, use glyph renderer
                            _pcx.FontFillColor = prevColor; //restore

                            //---------------

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

            using (Tools.BorrowEllipse(out var ellipse))
            using (Tools.BorrowVxs(out var v1, out var v2))
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
                        using (Tools.BorrowVxs(out var v1))
                        using (Tools.BorrowRect(out var r))
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
        public override void DrawLine(double x0, double y0, double x1, double y1)
        {
            _pcx.StrokeColor = _strokeColor;
            if (_lineDashGen == null)
            {
                _pcx.DrawLine((float)x0, (float)y0, (float)x1, (float)y1);
            }
            else
            {
                //TODO: line dash pattern cache
                _pcx.DrawLine((float)x0, (float)y0, (float)x1, (float)y1);
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