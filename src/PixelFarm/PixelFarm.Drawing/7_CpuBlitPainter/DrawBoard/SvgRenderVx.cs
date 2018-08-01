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
        Image,
        TextSpan,
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


    static class TempStrokeTool
    {

        [System.ThreadStatic]
        static Stack<Stroke> s_tempStrokes = new Stack<Stroke>();
        public static void GetFreeStroke(out Stroke tmpStroke)
        {
            if (s_tempStrokes.Count > 0)
            {
                tmpStroke = s_tempStrokes.Pop();
            }
            else
            {
                tmpStroke = new Stroke(1);
            }
        }
        public static void ReleaseStroke(ref Stroke s)
        {
            s.Width = 1;//reset
            s_tempStrokes.Push(s);
            s = null;
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
        //VgCmd[] _cmds;
        RectD _boundRect;
        bool _needBoundUpdate;
        public object _renderE;

        public VgRenderVx(object renderE)
        {
            _renderE = renderE;
            ////this is original version of the element 
            //this._cmds = cmds;
            //_needBoundUpdate = true;
        }
        public VgRenderVx Clone()
        {
            //make a copy of cmd stream
            //int j = _cmds.Length;
            //var copy = new VgCmd[j];
            //for (int i = 0; i < j; ++i)
            //{
            //    copy[i] = _cmds[i].Clone();
            //}

            return new VgRenderVx(null);
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
            return new RectD(0, 0, 100, 100);

            //if (_needBoundUpdate)
            //{
            //    int partCount = _cmds.Length;

            //    for (int i = 0; i < partCount; ++i)
            //    {
            //        VgCmd vx = _cmds[i];
            //        if (vx.Name != VgCommandName.Path)
            //        {
            //            continue;
            //        }

            //        RectD rectTotal = new RectD();
            //        VertexStore innerVxs = ((VgCmdPath)vx).Vxs;
            //        BoundingRect.GetBoundingRect(new VertexStoreSnap(innerVxs), ref rectTotal);

            //        _boundRect.ExpandToInclude(rectTotal);
            //    }

            //    _needBoundUpdate = false;
            //}
            //return _boundRect;
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
        //public VgCmd GetVgCmd(int index)
        //{
        //    return _cmds[index];
        //}
        //public int VgCmdCount
        //{
        //    get { return _cmds.Length; }
        //}
        //public VgCmd PrefixCommand { get; set; }
    }


    public abstract class VgCmd
    {
        public VgCmd(VgCommandName name)
        {
            Name = name;
        }
        public VgCommandName Name { get; set; }
        public virtual VgCmd Clone()
        {
            return null;
        }
    }

    public class VgCmdPath : VgCmd
    {
        public VgCmdPath() : base(VgCommandName.Path)
        {
        }
        public VertexStore Vxs { get; private set; }
        public void SetVxs(VertexStore vxs)
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

    //public class VgCmdBeginGroup : VgCmd
    //{
    //    public VgCmdBeginGroup() : base(VgCommandName.BeginGroup)
    //    {
    //    }
    //    public override VgCmd Clone()
    //    {
    //        return new VgCmdBeginGroup();
    //    }
    //}
    //public class VgCmdEndGroup : VgCmd
    //{
    //    public VgCmdEndGroup() : base(VgCommandName.EndGroup)
    //    {
    //    }
    //    public override VgCmd Clone()
    //    {
    //        return new VgCmdEndGroup();
    //    }
    //}


    //public class VgCmdTextSpan : VgCmd
    //{
    //    public VgCmdTextSpan() : base(VgCommandName.TextSpan)
    //    {
    //    }
    //    public override VgCmd Clone()
    //    {
    //        return new VgCmdTextSpan();
    //    }
    //}

    //public class VgCmdImage : VgCmd
    //{
    //    public VgCmdImage() : base(VgCommandName.Image)
    //    {
    //    }
    //    public Image Image { get; set; }
    //    public VertexStore Vxs { get; private set; }
    //    public void SetVxsAsOriginal(VertexStore vxs)
    //    {
    //        Vxs = vxs;
    //    }

    //    public override VgCmd Clone()
    //    {
    //        VgCmdImage vgImg = new VgCmdImage();
    //        vgImg.Image = this.Image;
    //        vgImg.Vxs = this.Vxs.CreateTrim();
    //        return vgImg;
    //    }
    //}
    ////-------------------------------------------------
    //public class VgCmdFillColor : VgCmd
    //{
    //    public VgCmdFillColor(Color color) : base(VgCommandName.FillColor) { Color = color; }
    //    public Color Color { get; set; }
    //    public override VgCmd Clone()
    //    {
    //        return new VgCmdFillColor(Color);
    //    }
    //}
    //public class VgCmdStrokeColor : VgCmd
    //{
    //    public VgCmdStrokeColor(Color color) : base(VgCommandName.StrokeColor) { Color = color; }
    //    public Color Color { get; set; }
    //    public override VgCmd Clone()
    //    {
    //        return new VgCmdStrokeColor(Color);
    //    }
    //}
    //public class VgCmdStrokeWidth : VgCmd
    //{
    //    public VgCmdStrokeWidth(float w) : base(VgCommandName.StrokeWidth) { Width = w; }
    //    public float Width { get; set; }
    //    public override VgCmd Clone()
    //    {
    //        return new VgCmdStrokeWidth(Width);
    //    }
    //}
    //public class VgCmdAffineTransform : VgCmd
    //{
    //    public VgCmdAffineTransform(Affine affine) : base(VgCommandName.AffineTransform)
    //    {
    //        TransformMatrix = affine;
    //    }
    //    public Affine TransformMatrix { get; private set; }

    //    public override VgCmd Clone()
    //    {
    //        return new VgCmdAffineTransform(this.TransformMatrix.Clone());
    //    }
    //}

    //public abstract class VgCmd
    //{
    //    public VgCmd(VgCommandName cmdKind)
    //    {
    //        Name = cmdKind;
    //    }
    //    public VgCommandName Name { get; set; }
    //    public abstract VgCmd Clone();
    //}


}