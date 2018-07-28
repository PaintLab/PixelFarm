//----------------------------------------------------------------------------
//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.CpuBlit
{
    //very simple svg parser  
    public enum VgCommandName
    {
        BeginGroup,
        EndGroup,
        Path,
        ClipPath,
        FillColor,
        StrokeColor,
        StrokeWidth,
        AffineTransform,
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




    static class SimpleRectClipEvaluator
    {
        enum RectSide
        {
            None,
            Vertical,
            Horizontal
        }

        static RectSide FindRectSide(float x0, float y0, float x1, float y1)
        {
            if (x0 == x1 && y0 != y1)
            {
                return RectSide.Vertical;
            }
            else if (y0 == y1 && x0 != x1)
            {
                return RectSide.Horizontal;
            }
            return RectSide.None;
        }

        /// <summary>
        /// check if this is a simple rect
        /// </summary>
        /// <param name="vxs"></param>
        /// <returns></returns>
        public static bool EvaluateRectClip(VertexStore vxs, out RectangleF clipRect)
        {
            float x0 = 0, y0 = 0;
            float x1 = 0, y1 = 0;
            float x2 = 0, y2 = 0;
            float x3 = 0, y3 = 0;
            float x4 = 0, y4 = 0;
            clipRect = new RectangleF();

            int sideCount = 0;

            int j = vxs.Count;
            for (int i = 0; i < j; ++i)
            {
                VertexCmd cmd = vxs.GetVertex(i, out double x, out double y);
                switch (cmd)
                {
                    default: return false;
                    case VertexCmd.NoMore:
                        if (i > 6) return false;
                        break;
                    case VertexCmd.Close:
                        if (i > 5)
                        {
                            return false;
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            switch (i)
                            {
                                case 1:
                                    x1 = (float)x;
                                    y1 = (float)y;
                                    sideCount++;
                                    break;
                                case 2:
                                    x2 = (float)x;
                                    y2 = (float)y;
                                    sideCount++;
                                    break;
                                case 3:
                                    x3 = (float)x;
                                    y3 = (float)y;
                                    sideCount++;
                                    break;
                                case 4:
                                    x4 = (float)x;
                                    y4 = (float)y;
                                    sideCount++;
                                    break;
                            }
                        }
                        break;
                    case VertexCmd.MoveTo:
                        {
                            if (i == 0)
                            {
                                x0 = (float)x;
                                y0 = (float)y;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        break;
                }
            }

            if (sideCount == 4)
            {
                RectSide s0 = FindRectSide(x0, y0, x1, y1);
                if (s0 == RectSide.None) return false;
                //
                RectSide s1 = FindRectSide(x1, y1, x2, y2);
                if (s1 == RectSide.None || s0 == s1) return false;
                //
                RectSide s2 = FindRectSide(x2, y2, x3, y3);
                if (s2 == RectSide.None || s1 == s2) return false;
                //
                RectSide s3 = FindRectSide(x3, y3, x4, y4);
                if (s3 == RectSide.None || s2 == s3) return false;
                //
                if (x4 == x0 && y4 == y0)
                {

                    if (s0 == RectSide.Horizontal)
                    {
                        clipRect = new RectangleF(x0, y0, x1 - x0, y3 - y0);
                    }
                    else
                    {
                        clipRect = new RectangleF(x0, y0, x3 - x0, y3 - y0);
                    }

                    return true;

                }
            }
            return false;

        }
    }

    enum ClipingTechnique
    {
        None,
        ClipMask,
        ClipSimpleRect
    }



    static class TempVgRenderStateStore
    {

        [System.ThreadStatic]
        static Stack<Stack<TempVgRenderState>> s_tempVgRenderStates = new Stack<Stack<TempVgRenderState>>();
        public static void GetFreeTempVgRenderState(out Stack<TempVgRenderState> tmpVgStateStack)
        {
            if (s_tempVgRenderStates.Count > 0)
            {
                tmpVgStateStack = s_tempVgRenderStates.Pop();
            }
            else
            {
                tmpVgStateStack = new Stack<TempVgRenderState>();
            }
        }
        public static void ReleaseTempVgRenderState(ref Stack<TempVgRenderState> tmpVgStateStack)
        {
            tmpVgStateStack.Clear();
            s_tempVgRenderStates.Push(tmpVgStateStack);
            tmpVgStateStack = null;
        }
    }
    struct TempVgRenderState
    {
        public float strokeWidth;
        public Color strokeColor;
        public Color fillColor;
        public Affine affineTx;
        public ClipingTechnique clippingTech;
    }


    public class VgRenderVx : RenderVx
    {

        Image _backimg;
        VgCmd[] _cmds;
        RectD _boundRect;
        bool _needBoundUpdate;
        public VgRenderVx(VgCmd[] svgVxList)
        {
            //this is original version of the element 
            this._cmds = svgVxList;
            _needBoundUpdate = true;
        }
        public VgRenderVx Clone()
        {
            //make a copy of cmd stream
            int j = _cmds.Length;
            var copy = new VgCmd[j];
            for (int i = 0; i < j; ++i)
            {
                copy[i] = _cmds[i].Clone();
            }

            return new VgRenderVx(copy);
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
                int partCount = _cmds.Length;

                for (int i = 0; i < partCount; ++i)
                {
                    VgCmd vx = _cmds[i];
                    if (vx.Name != VgCommandName.Path)
                    {
                        continue;
                    }

                    RectD rectTotal = new RectD();
                    VertexStore innerVxs = ((VgCmdPath)vx).Vxs;
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
            var renderState = new TempVgRenderState();
            renderState.strokeColor = p.StrokeColor;
            p.StrokeWidth = renderState.strokeWidth = 1;//default  
            renderState.fillColor = p.FillColor;
            renderState.affineTx = currentTx;
            renderState.clippingTech = ClipingTechnique.None;
            //------------------ 
            int j = _cmds.Length;

            TempVgRenderStateStore.GetFreeTempVgRenderState(out Stack<TempVgRenderState> vgStateStack);
            int i = 0;

            if (this.PrefixCommand != null)
            {
                i = -1;
            }

            VgCmd vx = null;

            for (; i < j; ++i)
            {
                if (i < 0)
                {
                    vx = this.PrefixCommand;
                }
                else
                {
                    vx = _cmds[i];
                }

                switch (vx.Name)
                {
                    case VgCommandName.BeginGroup:
                        {
                            //1. save current state before enter new state 
                            vgStateStack.Push(renderState);//save prev state
                            renderState.clippingTech = ClipingTechnique.None;//reset for this
                        }
                        break;
                    case VgCommandName.EndGroup:
                        {
                            switch (renderState.clippingTech)
                            {
                                case ClipingTechnique.None: break;
                                case ClipingTechnique.ClipMask:
                                    {
                                        //clear mask filter                               
                                        //remove from current clip
                                        AggPainter aggPainter = (AggPainter)p;
                                        aggPainter.EnableBuiltInMaskComposite = false;
                                        aggPainter.TargetBufferName = TargetBufferName.AlphaMask;//swicth to mask buffer
                                        aggPainter.Clear(Color.Black);
                                        aggPainter.TargetBufferName = TargetBufferName.Default;
                                    }
                                    break;
                                case ClipingTechnique.ClipSimpleRect:
                                    {

                                        AggPainter aggPainter = (AggPainter)p;
                                        aggPainter.SetClipBox(0, 0, aggPainter.Width, aggPainter.Height);
                                    }
                                    break;
                            }

                            //restore to prev state
                            renderState = vgStateStack.Pop();

                            p.FillColor = renderState.fillColor;
                            p.StrokeColor = renderState.strokeColor;
                            p.StrokeWidth = renderState.strokeWidth;
                            currentTx = renderState.affineTx;

                        }
                        break;
                    case VgCommandName.FillColor:
                        p.FillColor = renderState.fillColor = ((VgCmdFillColor)vx).Color;
                        break;
                    case VgCommandName.StrokeColor:
                        p.StrokeColor = renderState.strokeColor = ((VgCmdStrokeColor)vx).Color;
                        break;
                    case VgCommandName.StrokeWidth:
                        p.StrokeWidth = renderState.strokeWidth = ((VgCmdStrokeWidth)vx).Width;
                        break;
                    case VgCommandName.AffineTransform:
                        {
                            //apply this to current tx 
                            if (currentTx != null)
                            {
                                //*** IMPORTANT : matrix transform order !***
                                currentTx = ((VgCmdAffineTransform)vx).TransformMatrix * currentTx;
                            }
                            else
                            {
                                currentTx = ((VgCmdAffineTransform)vx).TransformMatrix;
                            }
                            renderState.affineTx = currentTx;
                        }
                        break;
                    case VgCommandName.ClipPath:
                        {
                            //clip-path
                            if (p is AggPainter)
                            {
                                VgCmdClipPath clipPath = (VgCmdClipPath)vx;
                                VertexStore clipVxs = ((VgCmdPath)clipPath._svgParts[0]).Vxs;

                                //----------
                                //for optimization check if clip path is Rect
                                //if yes => do simple rect clip 
                                //----------
                                AggPainter aggPainter = (AggPainter)p;

                                if (currentTx != null)
                                {
                                    //have some tx
                                    using (VxsContext.Temp(out var v1))
                                    {
                                        currentTx.TransformToVxs(clipVxs, v1);
                                        //after transform
                                        //check if v1 is rect clip or not
                                        //if yes => then just use simple rect clip
                                        if (SimpleRectClipEvaluator.EvaluateRectClip(v1, out RectangleF clipRect))
                                        {
                                            //use simple rect technique
                                            aggPainter.SetClipBox((int)clipRect.X, (int)clipRect.Y, (int)clipRect.Right, (int)clipRect.Bottom);
                                            renderState.clippingTech = ClipingTechnique.ClipSimpleRect;

                                        }
                                        else
                                        {
                                            //not simple rect => 
                                            //use mask technique

                                            renderState.clippingTech = ClipingTechnique.ClipMask;
                                            aggPainter.TargetBufferName = TargetBufferName.AlphaMask;
                                            //aggPainter.TargetBufferName = TargetBufferName.Default; //for debug
                                            var prevColor = aggPainter.FillColor;
                                            aggPainter.FillColor = Color.White;
                                            //aggPainter.StrokeColor = Color.Black; //for debug
                                            //aggPainter.StrokeWidth = 1; //for debug  
                                            //p.Draw(v1); //for debug
                                            p.Fill(v1);

                                            aggPainter.FillColor = prevColor;
                                            aggPainter.TargetBufferName = TargetBufferName.Default;//swicth to default buffer
                                            aggPainter.EnableBuiltInMaskComposite = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //aggPainter.Draw(clipVxs); //for debug 
                                    //check if clipVxs is rect or not
                                    if (SimpleRectClipEvaluator.EvaluateRectClip(clipVxs, out RectangleF clipRect))
                                    {
                                        //use simple rect technique
                                        aggPainter.SetClipBox((int)clipRect.X, (int)clipRect.Y, (int)clipRect.Right, (int)clipRect.Bottom);
                                        renderState.clippingTech = ClipingTechnique.ClipSimpleRect;
                                    }
                                    else
                                    {
                                        //not simple rect => 
                                        //use mask technique
                                        renderState.clippingTech = ClipingTechnique.ClipMask;

                                        aggPainter.TargetBufferName = TargetBufferName.AlphaMask;
                                        //aggPainter.TargetBufferName = TargetBufferName.Default; //for debug
                                        var prevColor = aggPainter.FillColor;
                                        aggPainter.FillColor = Color.White;
                                        //aggPainter.StrokeColor = Color.Black; //for debug
                                        //aggPainter.StrokeWidth = 1; //for debug 

                                        //p.Draw(v1); //for debug
                                        aggPainter.Fill(clipVxs);

                                        aggPainter.FillColor = prevColor;
                                        aggPainter.TargetBufferName = TargetBufferName.Default;//swicth to default buffer
                                        aggPainter.EnableBuiltInMaskComposite = true;
                                    }
                                }
                            }
                        }
                        break;

                    case VgCommandName.Path:
                        {
                            VgCmdPath path = (VgCmdPath)vx;
                            VertexStore vxs = path.Vxs;

                            if (renderState.fillColor.A > 0)
                            {
                                if (currentTx == null)
                                {
                                    p.Fill(vxs);

                                    //to draw stroke
                                    //stroke width must > 0 and stroke-color must not be transparent color

                                    if (renderState.strokeWidth > 0 && renderState.strokeColor.A > 0)
                                    {
                                        //has specific stroke color  
                                        AggPainter aggPainter = p as AggPainter;
                                        if (aggPainter != null && aggPainter.LineRenderingTech == LineRenderingTechnique.OutlineAARenderer)
                                        {
                                            //TODO: review here again
                                            aggPainter.Draw(new VertexStoreSnap(vxs), renderState.strokeColor);
                                        }
                                        else
                                        {
                                            VertexStore strokeVxs = GetStrokeVxsOrCreateNew(vxs, p, (float)p.StrokeWidth);
                                            p.Fill(strokeVxs, renderState.strokeColor);
                                        }
                                    }
                                }
                                else
                                {
                                    //have some tx
                                    using (VxsContext.Temp(out var v1))
                                    {
                                        currentTx.TransformToVxs(vxs, v1);
                                        p.Fill(v1);

                                        //to draw stroke
                                        //stroke width must > 0 and stroke-color must not be transparent color 
                                        if (renderState.strokeWidth > 0 && renderState.strokeColor.A > 0)
                                        {
                                            //has specific stroke color  
                                            AggPainter aggPainter = p as AggPainter;
                                            if (aggPainter != null && aggPainter.LineRenderingTech == LineRenderingTechnique.OutlineAARenderer)
                                            {
                                                //TODO: review here again 
                                                aggPainter.Draw(new VertexStoreSnap(v1), renderState.strokeColor);
                                            }
                                            else
                                            {
                                                VertexStore strokeVxs = GetStrokeVxsOrCreateNew(v1, p, (float)p.StrokeWidth);
                                                p.Fill(strokeVxs, renderState.strokeColor);

                                                //if (currentTx == null)
                                                //{

                                                //}
                                                //else
                                                //{
                                                //    //have some tx  
                                                //    using (VxsContext.Temp(out var v2))
                                                //    {
                                                //        currentTx.TransformToVxs(strokeVxs, v2);
                                                //        p.Fill(v2, renderState.strokeColor);
                                                //    }
                                                //}
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            TempVgRenderStateStore.ReleaseTempVgRenderState(ref vgStateStack);
        }

        static VertexStore GetStrokeVxsOrCreateNew(VertexStore vxs, Painter p, float strokeW)
        {
            using (VxsContext.Temp(out var v1))
            {
                p.VectorTool.CreateStroke(vxs, strokeW, v1);
                return v1.CreateTrim();
            }
        }

        public VgCmd GetVgCmd(int index)
        {
            return _cmds[index];
        }
        public int VgCmdCount
        {
            get { return _cmds.Length; }
        }
        public VgCmd PrefixCommand { get; set; }
    }


    public class VgCmdClipPath : VgCmd
    {
        public List<VgCmd> _svgParts;
        public VgCmdClipPath()
            : base(VgCommandName.ClipPath)
        {
        }
        public override VgCmd Clone()
        {
            VgCmdClipPath clipPath = new VgCmdClipPath();
            clipPath._svgParts = new List<VgCmd>();
            int j = _svgParts.Count;
            for (int i = 0; i < j; ++i)
            {
                clipPath._svgParts[i] = _svgParts[i].Clone();
            }
            return clipPath;
        }
    }

    public class VgCmdBeginGroup : VgCmd
    {
        public VgCmdBeginGroup() : base(VgCommandName.BeginGroup)
        {
        }
        public override VgCmd Clone()
        {
            return new VgCmdBeginGroup();
        }
    }
    public class VgCmdEndGroup : VgCmd
    {
        public VgCmdEndGroup() : base(VgCommandName.EndGroup)
        {
        }
        public override VgCmd Clone()
        {
            return new VgCmdEndGroup();
        }
    }

    public class VgCmdPath : VgCmd
    {
        public VgCmdPath() : base(VgCommandName.Path)
        {
        }
        public VertexStore Vxs { get; private set; }
        public void SetVxsAsOriginal(VertexStore vxs)
        {
            Vxs = vxs;
        }
        internal VertexStore StrokeVxs { get; set; } //transient obj
        public override VgCmd Clone()
        {
            VgCmdPath vgPath = new VgCmdPath();
            vgPath.Vxs = this.Vxs.CreateTrim();
            return vgPath;
        }
    }
    //-------------------------------------------------
    public class VgCmdFillColor : VgCmd
    {
        public VgCmdFillColor(Color color) : base(VgCommandName.FillColor) { Color = color; }
        public Color Color { get; set; }
        public override VgCmd Clone()
        {
            return new VgCmdFillColor(Color);
        }
    }
    public class VgCmdStrokeColor : VgCmd
    {
        public VgCmdStrokeColor(Color color) : base(VgCommandName.StrokeColor) { Color = color; }
        public Color Color { get; set; }
        public override VgCmd Clone()
        {
            return new VgCmdStrokeColor(Color);
        }
    }
    public class VgCmdStrokeWidth : VgCmd
    {
        public VgCmdStrokeWidth(float w) : base(VgCommandName.StrokeWidth) { Width = w; }
        public float Width { get; set; }
        public override VgCmd Clone()
        {
            return new VgCmdStrokeWidth(Width);
        }
    }
    public class VgCmdAffineTransform : VgCmd
    {
        public VgCmdAffineTransform(Affine affine) : base(VgCommandName.AffineTransform)
        {
            TransformMatrix = affine;
        }
        public Affine TransformMatrix { get; private set; }

        public override VgCmd Clone()
        {
            return new VgCmdAffineTransform(this.TransformMatrix.Clone());
        }
    }

    public abstract class VgCmd
    {
        public VgCmd(VgCommandName cmdKind)
        {
            Name = cmdKind;
        }
        public VgCommandName Name { get; set; }
        public abstract VgCmd Clone();
    }


}