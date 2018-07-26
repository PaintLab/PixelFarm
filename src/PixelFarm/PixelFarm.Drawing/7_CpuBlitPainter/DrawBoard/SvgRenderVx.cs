//----------------------------------------------------------------------------
//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm.Drawing.PainterExtensions;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.CpuBlit
{
    //very simple svg parser  
    public enum SvgRenderVxKind
    {
        BeginGroup,
        EndGroup,
        Path,
        ClipPath,
    }

    public class VxsRenderVx : RenderVx
    {
        public VertexStore _vxs;
        public VxsRenderVx(VertexStore vxs)
        {
            _vxs = vxs;

        }

        object _resolvedObject;
        public static object GetResolvedObject(VxsRenderVx vxsRenerVx)
        {
            return vxsRenerVx._resolvedObject;
        }
        public static void SetResolvedObject(VxsRenderVx vxsRenerVx, object obj)
        {
            vxsRenerVx._resolvedObject = obj;
        }

    }



    public class SvgRenderVx : RenderVx
    {

        struct TempRenderState
        {
            public float strokeWidth;
            public Color strokeColor;
            public Color fillColor;
            public Affine affineTx;
            public SvgClipPath clipPath;

        }

        Image _backimg;
        SvgPart[] _vxList;//working vxs
        SvgPart[] _originalVxs; //original definition

        RectD _boundRect;
        bool _needBoundUpdate;



        public SvgRenderVx(SvgPart[] svgVxList)
        {
            //this is original version of the element
            this._originalVxs = svgVxList;
            this._vxList = svgVxList;
            _needBoundUpdate = true;
        }


        public void InvalidateBounds()
        {
            _needBoundUpdate = true;
            _boundRect = new RectD(this.X, this.Y, 2, 2);
        }
        public RectD GetBounds()
        {
            //find bound
            //TODO: review here
            if (_needBoundUpdate)
            {
                int partCount = _vxList.Length;

                for (int i = 0; i < partCount; ++i)
                {
                    SvgPart vx = _vxList[i];
                    if (vx.Kind != SvgRenderVxKind.Path)
                    {
                        continue;
                    }

                    RectD rectTotal = new RectD();
                    VertexStore innerVxs = vx.GetVxs();
                    BoundingRect.GetBoundingRect(new VertexStoreSnap(innerVxs), ref rectTotal);

                    _boundRect.ExpandToInclude(rectTotal);
                }

                _needBoundUpdate = false;
            }
            return _boundRect;
        }

        public bool HasBitmapSnapshot { get; internal set; }

        public Image BackingImage { get { return _backimg; } }
        public bool DisableBackingImage { get; set; }

        public void SetBitmapSnapshot(Image img)
        {
            this._backimg = img;
            HasBitmapSnapshot = img != null;
        }


        public float X { get; set; }
        public float Y { get; set; }


        public void Render(Painter p)
        {

            //
            if (HasBitmapSnapshot)
            {
                p.DrawImage(_backimg, X, Y);
                return;
            }

            Affine currentTx = null;

            var renderState = new TempRenderState();
            renderState.strokeColor = p.StrokeColor;
            renderState.strokeWidth = (float)p.StrokeWidth;
            renderState.fillColor = p.FillColor;
            renderState.affineTx = currentTx;


            //------------------ 
            int j = _vxList.Length;
            Stack<bool> _hasClipPaths = new Stack<bool>();

            for (int i = 0; i < j; ++i)
            {
                SvgPart vx = _vxList[i];
                switch (vx.Kind)
                {
                    case SvgRenderVxKind.BeginGroup:
                        {
                            //1. save current state before enter new state 
                            p.StackPushUserObject(renderState);

                            //2. enter new px context
                            if (vx.HasFillColor)
                            {
                                p.FillColor = renderState.fillColor = vx.FillColor;
                            }
                            if (vx.HasStrokeColor)
                            {
                                p.StrokeColor = renderState.strokeColor = vx.StrokeColor;
                            }
                            if (vx.HasStrokeWidth)
                            {
                                p.StrokeWidth = renderState.strokeWidth = vx.StrokeWidth;
                            }
                            if (vx.AffineTx != null)
                            {
                                //apply this to current tx
                                if (currentTx != null)
                                {
                                    //*** IMPORTANT : matrix transform order !***
                                    currentTx = vx.AffineTx * currentTx;
                                }
                                else
                                {
                                    currentTx = vx.AffineTx;
                                }
                                renderState.affineTx = currentTx;
                            }
                            //
                            // 
                            _hasClipPaths.Push((vx.ClipPath != null));

                            if (vx.ClipPath != null)
                            {
                                //implement region 
                                if (p is AggPainter)
                                {
                                    AggPainter aggPainter = (AggPainter)p;
                                    aggPainter.TargetBufferName = TargetBufferName.AlphaMask;
                                    
                                    //aggPainter.TargetBufferName = TargetBufferName.Default; //for debug

                                    var prevColor = aggPainter.FillColor;
                                    aggPainter.FillColor = Color.White;
                                    //aggPainter.StrokeColor = Color.Black; //for debug
                                    //aggPainter.StrokeWidth = 1; //for debug

                                    VertexStore clipVxs = vx.ClipPath._svgParts[0].GetVxs();
                                    if (currentTx != null)
                                    {
                                        //have some tx
                                        using (VxsContext.Temp(out var v1))
                                        {
                                            currentTx.TransformToVxs(clipVxs, v1);
                                            //p.Draw(v1); //for debug
                                            p.Fill(v1);
                                        }
                                    }
                                    else
                                    {
                                        //aggPainter.Draw(clipVxs); //for debug
                                        aggPainter.Fill(clipVxs);
                                    }

                                    aggPainter.FillColor = prevColor;
                                    aggPainter.TargetBufferName = TargetBufferName.Default;//swicth to default buffer
                                    aggPainter.EnableBuiltInMaskComposite = true;
                                }
                            }

                        }
                        break;
                    case SvgRenderVxKind.EndGroup:
                        {
                            //restore to prev state
                            renderState = (TempRenderState)p.StackPopUserObject();
                            p.FillColor = renderState.fillColor;
                            p.StrokeColor = renderState.strokeColor;
                            p.StrokeWidth = renderState.strokeWidth;
                            currentTx = renderState.affineTx;

                            bool hasClip = _hasClipPaths.Pop();
                            if (hasClip)
                            {
                                //clear mask filter                               
                                //remove from current clip
                                AggPainter aggPainter = (AggPainter)p;
                                aggPainter.EnableBuiltInMaskComposite = false;
                                aggPainter.TargetBufferName = TargetBufferName.AlphaMask;//swicth to mask buffer
                                aggPainter.Clear(Color.Black);                               
                                aggPainter.TargetBufferName = TargetBufferName.Default;
                            }
                        }
                        break;
                    case SvgRenderVxKind.Path:
                        {

                            VertexStore vxs = vx.GetVxs();
                            if (vx.HasFillColor)
                            {
                                //has specific fill color
                                if (vx.FillColor.A > 0)
                                {
                                    if (currentTx == null)
                                    {
                                        p.Fill(vxs, vx.FillColor);
                                    }
                                    else
                                    {
                                        //have some tx
                                        using (VxsContext.Temp(out var v1))
                                        {
                                            currentTx.TransformToVxs(vxs, v1);
                                            p.Fill(v1, vx.FillColor);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (p.FillColor.A > 0)
                                {
                                    if (currentTx == null)
                                    {
                                        p.Fill(vxs);
                                    }
                                    else
                                    {
                                        //have some tx

                                        using (VxsContext.Temp(out var v1))
                                        {
                                            currentTx.TransformToVxs(vxs, v1);
                                            p.Fill(v1);
                                        }
                                    }
                                }
                            }



                            if (p.StrokeWidth > 0)
                            {
                                //check if we have a stroke version of this render vx
                                //if not then request a new one  

                                //p.StrokeWidth = vx.StrokeWidth;

                                if (vx.HasStrokeColor || p.StrokeColor.A > 0)
                                {
                                    //has specific stroke color  
                                    AggPainter aggPainter = p as AggPainter;
                                    if (aggPainter != null && aggPainter.LineRenderingTech == LineRenderingTechnique.OutlineAARenderer)
                                    {
                                        //TODO: review here again
                                        aggPainter.Draw(new VertexStoreSnap(vx.GetVxs()), vx.StrokeColor);
                                    }
                                    else
                                    {
                                        VertexStore strokeVxs = GetStrokeVxsOrCreateNew(vx, p, (float)p.StrokeWidth);
                                        if (currentTx == null)
                                        {
                                            p.Fill(strokeVxs, vx.StrokeColor);
                                        }
                                        else
                                        {
                                            //have some tx 

                                            using (VxsContext.Temp(out var v1))
                                            {
                                                currentTx.TransformToVxs(strokeVxs, v1);
                                                p.Fill(v1, renderState.strokeColor);
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {

                                if (vx.HasStrokeColor || p.StrokeColor.A > 0)
                                {
                                    AggPainter aggPainter = p as AggPainter;
                                    if (aggPainter != null && aggPainter.LineRenderingTech == LineRenderingTechnique.OutlineAARenderer)
                                    {
                                        aggPainter.Draw(new VertexStoreSnap(vx.GetVxs()), vx.StrokeColor);
                                    }
                                    else
                                    {
                                        VertexStore strokeVxs = GetStrokeVxsOrCreateNew(vx, p, (float)p.StrokeWidth);
                                        p.Fill(strokeVxs);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }


        static VertexStore GetStrokeVxsOrCreateNew(SvgPart s, Painter p, float strokeW)
        {
            VertexStore strokeVxs = s.StrokeVxs;
            if (strokeVxs != null && s.StrokeWidth == strokeW)
            {
                return strokeVxs;
            }

            using (VxsContext.Temp(out var vxs))
            {
                p.VectorTool.CreateStroke(s.GetVxs(), strokeW, vxs);
                s.StrokeVxs = vxs.CreateTrim();
            }

            return s.StrokeVxs;
        }

        public SvgPart GetInnerVx(int index)
        {
            return _vxList[index];
        }
        public int SvgVxCount
        {
            get { return _vxList.Length; }
        }

        public void SetInnerVx(int index, SvgPart p)
        {
            _vxList[index] = p;
        }
        public void ResetTransform()
        {
            _vxList = _originalVxs;
        }
    }


    public class SvgClipPath : SvgPart
    {
        public List<SvgPart> _svgParts;
        public SvgClipPath()
            : base(SvgRenderVxKind.ClipPath)
        {
        }
    }

    public class SvgBeginGroup : SvgPart
    {
        public SvgBeginGroup() : base(SvgRenderVxKind.BeginGroup)
        {

        }
    }
    public class SvgEndGroup : SvgPart
    {
        public SvgEndGroup() : base(SvgRenderVxKind.EndGroup)
        {

        }
    }

    public class SvgPart
    {
        VertexStore _vxs;
        VertexStore _vxs_org;
        Color _fillColor;
        Color _strokeColor;
        float _strokeWidth;


        double _strokeVxsStrokeWidth;
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public SvgPart(SvgRenderVxKind kind)
        {

#if DEBUG
            //if (dbugId == 37)
            //{

            //}
            //Console.WriteLine(dbugId);
#endif
            this.Kind = kind;
        }
        public SvgRenderVxKind Kind
        {
            get;
            private set;
        }

        public bool HasFillColor { get; private set; }
        public bool HasStrokeColor { get; private set; }
        public bool HasStrokeWidth { get; private set; }



        public Color FillColor
        {
            get { return _fillColor; }
            set
            {
                _fillColor = value;
                HasFillColor = true;
            }
        }
        public Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                _strokeColor = value;
                HasStrokeColor = true;
            }
        }

        public void SetVxsAsOriginal(VertexStore vxs)
        {
            this._vxs = vxs;
            this._vxs_org = vxs;
            if (_vxs == null)
            {

            }
        }
        public void RestoreOrg()
        {
            _vxs = _vxs_org;
            if (_vxs == null)
            {

            }
        }
        public void SetVxs(VertexStore vxs)
        {
            this._vxs = vxs;

            if (_vxs == null)
            {

            }
        }
        public VertexStore GetVxs()
        {
#if DEBUG
            if (_vxs != null && _vxs._dbugIsChanged)
            {

            }
#endif
            return _vxs;
        }
        public float StrokeWidth
        {
            get { return _strokeWidth; }
            set
            {
                _strokeWidth = value;
                HasStrokeWidth = true;
            }
        }

        public SvgClipPath ClipPath { get; set; }
        public Affine AffineTx { get; set; }
        public VertexStore StrokeVxs
        {
            get;
            set;
        }
        public static SvgPart TransformToNew(SvgPart originalSvgVx, PixelFarm.CpuBlit.VertexProcessing.Affine tx)
        {
            SvgPart newSx = new SvgPart(originalSvgVx.Kind);
            if (originalSvgVx._vxs != null)
            {
                using (VxsContext.Temp(out var vxs))
                {
                    tx.TransformToVxs(originalSvgVx._vxs, vxs);
                    newSx._vxs = vxs.CreateTrim();
                }
            }

            if (originalSvgVx.HasFillColor)
            {
                newSx.FillColor = originalSvgVx._fillColor;
            }
            if (originalSvgVx.HasStrokeColor)
            {
                newSx.StrokeColor = originalSvgVx.StrokeColor;
            }
            if (originalSvgVx.HasStrokeWidth)
            {
                newSx.StrokeWidth = originalSvgVx.StrokeWidth;
            }
            return newSx;
        }
        public static SvgPart TransformToNew(SvgPart originalSvgVx, PixelFarm.CpuBlit.VertexProcessing.Bilinear tx)
        {
            SvgPart newSx = new SvgPart(originalSvgVx.Kind);
            newSx.SetVxsAsOriginal(originalSvgVx.GetVxs());

            if (newSx._vxs != null)
            {
                using (VxsContext.Temp(out var vxs))
                {
                    tx.TransformToVxs(originalSvgVx._vxs, vxs);
                    newSx._vxs = vxs.CreateTrim();
                }
            }

            if (originalSvgVx.HasFillColor)
            {
                newSx._fillColor = originalSvgVx._fillColor;
            }
            if (originalSvgVx.HasStrokeColor)
            {
                newSx.StrokeColor = originalSvgVx.StrokeColor;
            }
            if (originalSvgVx.HasStrokeWidth)
            {
                newSx.StrokeWidth = originalSvgVx.StrokeWidth;
            }


            return newSx;
        }


        //
        object _resolvedObject; //platform's object
        public static object GetResolvedObject(SvgPart vxsRenerVx)
        {
            return vxsRenerVx._resolvedObject;
        }
        public static void SetResolvedObject(SvgPart vxsRenerVx, object obj)
        {
            vxsRenerVx._resolvedObject = obj;
        }
    }

}