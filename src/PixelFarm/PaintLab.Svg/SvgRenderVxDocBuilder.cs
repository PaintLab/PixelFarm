//----------------------------------------------------------------------------
//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.Svg;
using LayoutFarm.Svg.Pathing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using LayoutFarm.WebDom;


namespace PaintLab.Svg
{
    public class SvgImageBinder : LayoutFarm.ImageBinder
    {

        public SvgImageBinder(string imgsrc) : base(imgsrc)
        {

        }
    }

    //----------------------
    public class SvgHitChain
    {
        float _rootHitX;
        float _rootHitY;
        List<SvgHitInfo> svgList = new List<SvgHitInfo>();
        public SvgHitChain()
        {
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public void SetHitTestPos(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public bool WithSubPartTest { get; set; }
        public bool MakeCopyOfHitVxs { get; set; }
        public void AddHit(SvgRenderElement svg, float x, float y, VertexStore copyOfVxs)
        {
            svgList.Add(new SvgHitInfo(svg, x, y, copyOfVxs));
        }
        public int Count
        {
            get
            {
                return this.svgList.Count;
            }
        }
        public SvgHitInfo GetHitInfo(int index)
        {
            return this.svgList[index];
        }
        public SvgHitInfo GetLastHitInfo()
        {
            return this.svgList[svgList.Count - 1];
        }
        public void Clear()
        {
            this.X = this.Y = 0;
            this._rootHitX = this._rootHitY = 0;
            this.svgList.Clear();
            MakeCopyOfHitVxs = WithSubPartTest = false;


        }
        public void SetRootGlobalPosition(float x, float y)
        {
            this._rootHitX = x;
            this._rootHitY = y;
        }
    }

    public struct SvgHitInfo
    {
        public readonly SvgRenderElement svg;
        public readonly float x;
        public readonly float y;
        public readonly VertexStore copyOfVxs;
        public SvgHitInfo(SvgRenderElement svg, float x, float y, VertexStore copyOfVxs)
        {
            this.svg = svg;
            this.x = x;
            this.y = y;
            this.copyOfVxs = copyOfVxs;
        }
        public SvgElement GetSvgElement()
        {
            return svg.GetController() as SvgElement;
        }
    }

    public class VgPaintArgs
    {
        public Painter P;
        public Affine _currentTx;
        public Action<VertexStore, VgPaintArgs> ExternalVxsVisitHandler;
        public SvgRenderElement Current;

        internal void Reset()
        {
            P = null;
            _currentTx = null;
            ExternalVxsVisitHandler = null;
        }
    }

    public static class VgPainterArgsPool
    {

        [System.ThreadStatic]
        static Stack<VgPaintArgs> s_vgPaintArgs = new Stack<VgPaintArgs>();
        public static void GetFreePainterArgs(Painter painter, out VgPaintArgs p)
        {
            if (s_vgPaintArgs.Count > 0)
            {
                p = s_vgPaintArgs.Pop();
                p.P = painter;
            }
            else
            {
                p = new VgPaintArgs { P = painter };
            }
        }
        public static void ReleasePainterArgs(ref VgPaintArgs p)
        {
            p.Reset();
            s_vgPaintArgs.Push(p);
            p = null;
        }
    }



    public abstract class SvgRenderElementBase
    {
        public virtual void Paint(VgPaintArgs p)
        {
            //paint with painter interface
        }
        public virtual void Walk(VgPaintArgs p) { }

        /// <summary>
        /// clone visual part
        /// </summary>
        /// <returns></returns>
        public abstract SvgRenderElementBase Clone();
#if DEBUG
        public bool dbugHasParent;

        public readonly int dbugId = s_dbugTotalId++;
        static int s_dbugTotalId;
#endif
    }
    public class VgTextNodeRenderElement : SvgRenderElementBase
    {
        public string TextContent { get; set; }
        public override SvgRenderElementBase Clone()
        {
            return new VgTextNodeRenderElement { TextContent = this.TextContent };
        }
    }



    class SvgPathRenderMarkers
    {
        public PointF StartMarkerPos { get; set; }
        public PointF EndMarkerPos { get; set; }
        public PointF[] AllPoints { get; set; }

        public Affine StartMarkerAffine { get; set; }
        public Affine MidMarkerAffine { get; set; }
        public Affine EndMarkerAffine { get; set; }

        public SvgRenderElement StartMarker { get; set; }
        public SvgRenderElement MidMarker { get; set; }
        public SvgRenderElement EndMarker { get; set; }
    }

    class SvgUseRenderElement : SvgRenderElement
    {
        public SvgUseRenderElement(SvgUseSpec useSpec, SvgRenderRootElement root)
            : base(WellknownSvgElementName.Use, useSpec, root)
        {

        }
        internal SvgRenderElement HRefSvgRenderElement { get; set; }
        public override void Paint(VgPaintArgs vgPainterArgs)
        {
            Painter p = vgPainterArgs.P;
            SvgUseSpec useSpec = (SvgUseSpec)this._visualSpec;
            Affine current_tx = vgPainterArgs._currentTx;

            Color color = p.FillColor;
            double strokeW = p.StrokeWidth;
            Color strokeColor = p.StrokeColor;

            if (current_tx != null)
            {
                vgPainterArgs._currentTx = Affine.NewTranslation(useSpec.X.Number, useSpec.Y.Number) * current_tx;
            }
            else
            {
                vgPainterArgs._currentTx = Affine.NewTranslation(useSpec.X.Number, useSpec.Y.Number);
            }

            if (this._visualSpec.HasFillColor)
            {
                p.FillColor = _visualSpec.FillColor;
            }

            if (this._visualSpec.HasStrokeColor)
            {
                //temp fix
                p.StrokeColor = _visualSpec.StrokeColor;
            }
            else
            {

            }

            if (this._visualSpec.HasStrokeWidth)
            {
                //temp fix
                p.StrokeWidth = _visualSpec.StrokeWidth.Number;
            }
            else
            {

            }

            HRefSvgRenderElement.Paint(vgPainterArgs);
            //base.Paint(vgPainterArgs);

            //restore
            p.FillColor = color;
            p.StrokeColor = strokeColor;
            p.StrokeWidth = strokeW;
            //
            vgPainterArgs._currentTx = current_tx;
        }
        public override void Walk(VgPaintArgs vgPainterArgs)
        {

            SvgUseSpec useSpec = (SvgUseSpec)this._visualSpec;
            Affine current_tx = vgPainterArgs._currentTx;

            if (current_tx != null)
            {
                vgPainterArgs._currentTx = Affine.NewTranslation(useSpec.X.Number, useSpec.Y.Number) * current_tx;
            }
            else
            {
                vgPainterArgs._currentTx = Affine.NewTranslation(useSpec.X.Number, useSpec.Y.Number);
            }


            HRefSvgRenderElement.Walk(vgPainterArgs);

            vgPainterArgs._currentTx = current_tx;
            //base.Walk(vgPainterArgs);
        }
    }



    public static class VgResourceIO
    {
        //IO 
        [System.ThreadStatic]
        static Action<LayoutFarm.ImageBinder, SvgRenderElement, object> s_vgIO;
        public static Action<LayoutFarm.ImageBinder, SvgRenderElement, object> VgImgIOHandler
        {
            get
            {
                return s_vgIO;
            }
            set
            {
                if (value == null)
                {
                    //clear existing value
                    s_vgIO = null;
                }
                else
                {
                    if (s_vgIO == null)
                    {
                        //please note that if the system already has one=>
                        //we do not replace it
                        s_vgIO = value;
                    }
                }
            }
        }

    }

    public class SvgRenderRootElement
    {
        internal Action<SvgRenderElement> _invalidate;
        Action<LayoutFarm.ImageBinder, SvgRenderElement, object> _imgReqHandler;
        public SvgRenderRootElement()
        {
        }
        public Action<LayoutFarm.ImageBinder, SvgRenderElement, object> ImgRequestHandler
        {
            get { return _imgReqHandler; }
            set
            {
                _imgReqHandler = value;
            }
        }
        internal void Invalidate(SvgRenderElement e)
        {
            if (_invalidate != null)
            {
                _invalidate(e);
            }
        }

        internal void RequestImageAsync(LayoutFarm.ImageBinder binder, SvgRenderElement imgRun, object requestFrom)
        {
            if (_imgReqHandler != null)
            {
                _imgReqHandler(binder, imgRun, requestFrom);
            }
            else
            {
                //ask for coment resource IO
                _imgReqHandler = VgResourceIO.VgImgIOHandler;
                if (_imgReqHandler != null)
                {
                    _imgReqHandler(binder, imgRun, requestFrom);
                }
            }
        }
    }
    public class SvgRenderElement : SvgRenderElementBase
    {

        public VertexStore _vxsPath;
        List<SvgRenderElementBase> _childNodes = null;
        WellknownSvgElementName _wellknownName;
        VertexStore _strokeVxs;
        float _latestStrokeW;
        object _controller;
        internal SvgVisualSpec _visualSpec;
        internal SvgPathRenderMarkers _pathMarkers;
        LayoutFarm.ImageBinder _imgBinder;
        SvgRenderRootElement _renderRoot;
        public SvgRenderElement(WellknownSvgElementName wellknownName,
            SvgVisualSpec visualSpec,
            SvgRenderRootElement renderRoot)
        {
            _wellknownName = wellknownName;
            _visualSpec = visualSpec;
            _renderRoot = renderRoot;
        }
        public WellknownSvgElementName ElemName
        {
            get { return _wellknownName; }
        }
        public void SetController(object o)
        {
            _controller = o;
        }
        public object GetController() { return _controller; }
        internal LayoutFarm.ImageBinder ImageBinder
        {
            get
            {
                return _imgBinder;
            }
            set
            {
                _imgBinder = value;
                if (value != null)
                {
                    //bind image change event
                    value.ImageChanged += Value_ImageChanged;
                }
            }
        }

        private void Value_ImageChanged(object sender, EventArgs e)
        {
            //
            _renderRoot.Invalidate(this);
        }

        public void HitTest(float x, float y, Action<SvgRenderElement, float, float, VertexStore> onHitSvg)
        {
            VgPainterArgsPool.GetFreePainterArgs(null, out VgPaintArgs paintArgs);
            paintArgs.ExternalVxsVisitHandler = (vxs, args) =>
            {
                if (args.Current != null &&
                   PixelFarm.CpuBlit.VertexProcessing.VertexHitTester.IsPointInVxs(vxs, x, y))
                {
                    //add actual transform vxs ... 
                    onHitSvg(args.Current, x, y, vxs);
                }
            };
            this.Walk(paintArgs);
            VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);
        }
        public bool HitTest(SvgHitChain hitChain)
        {
            VgPainterArgsPool.GetFreePainterArgs(null, out VgPaintArgs paintArgs);

            paintArgs.ExternalVxsVisitHandler = (vxs, args) =>
            {

                if (args.Current != null &&
                   PixelFarm.CpuBlit.VertexProcessing.VertexHitTester.IsPointInVxs(vxs, hitChain.X, hitChain.Y))
                {
                    //add actual transform vxs ... 
                    hitChain.AddHit(args.Current,
                        hitChain.X,
                        hitChain.Y,
                        hitChain.MakeCopyOfHitVxs ? vxs.CreateTrim() : null);
                }
            };

            this.Walk(paintArgs);
            VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);
            return hitChain.Count > 0;
        }

        public override SvgRenderElementBase Clone()
        {
            SvgRenderElement clone = new SvgRenderElement(_wellknownName, _visualSpec, _renderRoot);
            if (_vxsPath != null)
            {
                clone._vxsPath = this._vxsPath.CreateTrim();
            }
            if (_childNodes != null)
            {
                //deep clone
                int j = _childNodes.Count;
                List<SvgRenderElementBase> cloneChildNodes = new List<SvgRenderElementBase>(j);
                for (int i = 0; i < j; ++i)
                {
                    cloneChildNodes.Add(_childNodes[i].Clone());
                }
                clone._childNodes = cloneChildNodes;
            }
            //assign the same controller
            clone._controller = _controller;
            return clone;
        }

        internal float _imgW;
        internal float _imgH;
        internal float _imgX;
        internal float _imgY;

        static PixelFarm.CpuBlit.VertexProcessing.Affine CreateAffine(SvgTransform transformation)
        {
            switch (transformation.TransformKind)
            {
                default: throw new NotSupportedException();

                case SvgTransformKind.Matrix:

                    SvgTransformMatrix matrixTx = (SvgTransformMatrix)transformation;
                    float[] elems = matrixTx.Elements;
                    return new Affine(
                         elems[0], elems[1],
                         elems[2], elems[3],
                         elems[4], elems[5]);
                case SvgTransformKind.Rotation:
                    SvgRotate rotateTx = (SvgRotate)transformation;
                    if (rotateTx.SpecificRotationCenter)
                    {
                        //https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/transform
                        //svg's rotation=> angle in degree, so convert to rad ...

                        //translate to center 
                        //rotate and the translate back
                        return Affine.NewMatix(
                                PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-rotateTx.CenterX, -rotateTx.CenterY),
                                PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Rotate(AggMath.deg2rad(rotateTx.Angle)),
                                PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(rotateTx.CenterX, rotateTx.CenterY)
                            );
                    }
                    else
                    {
                        return PixelFarm.CpuBlit.VertexProcessing.Affine.NewRotation(AggMath.deg2rad(rotateTx.Angle));
                    }
                case SvgTransformKind.Scale:
                    SvgScale scaleTx = (SvgScale)transformation;
                    return PixelFarm.CpuBlit.VertexProcessing.Affine.NewScaling(scaleTx.X, scaleTx.Y);
                case SvgTransformKind.Shear:
                    SvgShear shearTx = (SvgShear)transformation;
                    return PixelFarm.CpuBlit.VertexProcessing.Affine.NewSkewing(shearTx.X, shearTx.Y);
                case SvgTransformKind.Translation:
                    SvgTranslate translateTx = (SvgTranslate)transformation;
                    return PixelFarm.CpuBlit.VertexProcessing.Affine.NewTranslation(translateTx.X, translateTx.Y);
            }
        }
        static VertexStore GetStrokeVxsOrCreateNew(VertexStore vxs, float strokeW)
        {

            using (VxsTemp.Borrow(out var v1))
            {
                PixelFarm.CpuBlit.TempStrokeTool.GetFreeStroke(out Stroke stroke);
                stroke.Width = strokeW;
                stroke.MakeVxs(vxs, v1);
                VertexStore vx = v1.CreateTrim();
                PixelFarm.CpuBlit.TempStrokeTool.ReleaseStroke(ref stroke);
                return vx;
            }
        }
        public override void Walk(VgPaintArgs vgPainterArgs)
        {
            if (vgPainterArgs.ExternalVxsVisitHandler == null)
            {
                return;
            }

            //----------------------------------------------------
            Affine prevTx = vgPainterArgs._currentTx; //backup
            Affine currentTx = vgPainterArgs._currentTx;

            if (_visualSpec != null)
            {

                if (_visualSpec.Transform != null)
                {
                    Affine latest = CreateAffine(_visualSpec.Transform);
                    if (currentTx != null)
                    {
                        //*** IMPORTANT : matrix transform order !***                         
                        currentTx = latest * vgPainterArgs._currentTx;
                    }
                    else
                    {
                        currentTx = latest;
                    }
                    vgPainterArgs._currentTx = currentTx;
                }

                //***SKIP CLIPPING***
                //if (_visualSpec.ResolvedClipPath != null)
                //{
                //    //clip-path
                //    hasClip = true;

                //    SvgRenderElement clipPath = (SvgRenderElement)_visualSpec.ResolvedClipPath;
                //    VertexStore clipVxs = ((SvgRenderElement)clipPath.GetChildNode(0))._vxsPath;
                //    //----------
                //    //for optimization check if clip path is Rect
                //    //if yes => do simple rect clip  
                //    if (currentTx != null)
                //    {
                //        //have some tx
                //        using (VxsContext.Temp(out var v1))
                //        {
                //            currentTx.TransformToVxs(clipVxs, v1);
                //            p.SetClipRgn(v1);
                //        }
                //    }
                //    else
                //    {
                //        p.SetClipRgn(clipVxs);
                //    }
                //}
                //***SKIP CLIPPING***
            }


            switch (this.ElemName)
            {
                default:
                    //unknown
                    break;
                case WellknownSvgElementName.Text:

                    break;
                case WellknownSvgElementName.Group:
                case WellknownSvgElementName.RootSvg:
                case WellknownSvgElementName.Svg:
                    break;
                case WellknownSvgElementName.Image:
                case WellknownSvgElementName.Path:
                case WellknownSvgElementName.Line:
                case WellknownSvgElementName.Ellipse:
                case WellknownSvgElementName.Circle:
                case WellknownSvgElementName.Polygon:
                case WellknownSvgElementName.Polyline:
                case WellknownSvgElementName.Rect:
                    {
                        //render with rect spec 

                        if (currentTx == null)
                        {
                            vgPainterArgs.Current = this;
                            vgPainterArgs.ExternalVxsVisitHandler(_vxsPath, vgPainterArgs);
                            vgPainterArgs.Current = null;
                        }
                        else
                        {
                            //have some tx
                            using (VxsTemp.Borrow(out var v1))
                            {
                                currentTx.TransformToVxs(_vxsPath, v1);
                                vgPainterArgs.Current = this;
                                vgPainterArgs.ExternalVxsVisitHandler(v1, vgPainterArgs);
                                vgPainterArgs.Current = null;
                            }
                        }
                        //------
                        if (this._pathMarkers != null)
                        {
                            //render each marker
                            if (_pathMarkers.StartMarker != null)
                            {
                                //draw this
                                //*** IMPORTANT : matrix transform order !***                 
                                //*** IMPORTANT : matrix transform order !***    
                                int cc = _pathMarkers.StartMarker.ChildCount;
                                for (int i = 0; i < cc; ++i)
                                {
                                    //temp fix
                                    //set offset   
                                    if (_pathMarkers.StartMarkerAffine != null)
                                    {
                                        vgPainterArgs._currentTx = Affine.NewTranslation(_pathMarkers.StartMarkerPos.X, _pathMarkers.StartMarkerPos.Y) * _pathMarkers.StartMarkerAffine;
                                    }

                                    _pathMarkers.StartMarker.GetChildNode(i).Walk(vgPainterArgs);
                                }
                                vgPainterArgs._currentTx = currentTx;

                            }
                            if (_pathMarkers.EndMarker != null)
                            {
                                //draw this 
                                int cc = _pathMarkers.EndMarker.ChildCount;
                                for (int i = 0; i < cc; ++i)
                                {
                                    //temp fix 
                                    if (_pathMarkers.EndMarkerAffine != null)
                                    {
                                        vgPainterArgs._currentTx = Affine.NewTranslation(_pathMarkers.EndMarkerPos.X, _pathMarkers.EndMarkerPos.Y) * _pathMarkers.EndMarkerAffine;
                                    }
                                    _pathMarkers.EndMarker.GetChildNode(i).Walk(vgPainterArgs);
                                }
                                vgPainterArgs._currentTx = currentTx;
                            }
                        }
                    }
                    break;
            }

            //-------------------------------------------------------
            int childCount = this.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                var node = GetChildNode(i) as SvgRenderElement;
                if (node != null)
                {
                    node.Walk(vgPainterArgs);
                }
            }


            vgPainterArgs._currentTx = prevTx;
            //***SKIP CLIPPING***
            //if (hasClip)
            //{
            //    p.SetClipRgn(null);
            //}
            //***SKIP CLIPPING***
        }

        public SvgVisualSpec VisualSpec
        {
            get { return _visualSpec; }
        }

        //---------------------------
        //TODO: review here again
        //a COPY from Typography.OpenFont.Typeface  
        const int pointsPerInch = 72;

        /// <summary>
        /// convert from point-unit value to pixel value
        /// </summary>
        /// <param name="pointSize"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        static float ConvPointsToPixels(float pointSize, int resolution = 96)
        {
            //http://stackoverflow.com/questions/139655/convert-pixels-to-points
            //points = pixels * 72 / 96
            //------------------------------------------------
            //pixels = targetPointSize * 96 /72
            //pixels = targetPointSize * resolution / pointPerInch
            return pointSize * resolution / pointsPerInch;
        }
        static float ConvPixelsToPoints(float pixelSize, int resolution = 96)
        {
            //http://stackoverflow.com/questions/139655/convert-pixels-to-points
            //points = pixels * 72 / 96
            //------------------------------------------------
            //pixels = targetPointSize * 96 /72
            //pixels = targetPointSize * resolution / pointPerInch
            return pixelSize * pointsPerInch / resolution;
        }
        //---------------------------
        public override void Paint(VgPaintArgs vgPainterArgs)
        {
            //save
            Painter p = vgPainterArgs.P;
            Color color = p.FillColor;
            double strokeW = p.StrokeWidth;
            Color strokeColor = p.StrokeColor;
            RequestFont currentFont = p.CurrentFont;

            Affine prevTx = vgPainterArgs._currentTx; //backup
            Affine currentTx = vgPainterArgs._currentTx;
            bool hasClip = false;
            bool newFontReq = false;


            if (_visualSpec != null)
            {

                if (_visualSpec.Transform != null)
                {
                    Affine latest = CreateAffine(_visualSpec.Transform);
                    if (currentTx != null)
                    {
                        //*** IMPORTANT : matrix transform order !***                         
                        currentTx = latest * vgPainterArgs._currentTx;
                    }
                    else
                    {
                        currentTx = latest;
                    }
                    vgPainterArgs._currentTx = currentTx;
                }
                //apply this to current tx 

                if (this._visualSpec.HasFillColor)
                {
                    p.FillColor = _visualSpec.FillColor;
                }

                if (this._visualSpec.HasStrokeColor)
                {
                    //temp fix
                    p.StrokeColor = _visualSpec.StrokeColor;
                }
                else
                {

                }

                if (this._visualSpec.HasStrokeWidth)
                {
                    //temp fix
                    p.StrokeWidth = _visualSpec.StrokeWidth.Number;
                }
                else
                {

                }

                if (_visualSpec.ResolvedClipPath != null)
                {
                    //clip-path
                    hasClip = true;

                    SvgRenderElement clipPath = (SvgRenderElement)_visualSpec.ResolvedClipPath;
                    VertexStore clipVxs = ((SvgRenderElement)clipPath.GetChildNode(0))._vxsPath;

                    //----------
                    //for optimization check if clip path is Rect
                    //if yes => do simple rect clip 

                    if (currentTx != null)
                    {
                        //have some tx
                        using (VxsTemp.Borrow(out var v1))
                        {
                            currentTx.TransformToVxs(clipVxs, v1);
                            p.SetClipRgn(v1);
                        }
                    }
                    else
                    {
                        p.SetClipRgn(clipVxs);
                    }
                }
            }


            switch (this.ElemName)
            {
                default:
                    //unknown
                    break;
                case WellknownSvgElementName.Group:
                case WellknownSvgElementName.RootSvg:
                case WellknownSvgElementName.Svg:
                    break;
                case WellknownSvgElementName.Image:
                    {
                        SvgImageSpec imgSpec = this._visualSpec as SvgImageSpec;
                        //request from resource
                        if (imgSpec.ImageSrc != null)
                        {
                            if (this.ImageBinder == null)
                            {
                                //create new 
                                this.ImageBinder = new SvgImageBinder(imgSpec.ImageSrc);
                            }
                            bool tryLoadOnce = false;
                            EVAL_STATE:
                            switch (this.ImageBinder.State)
                            {
                                case LayoutFarm.BinderState.Unload:
                                    if (!tryLoadOnce)
                                    {
                                        tryLoadOnce = true;

                                        _renderRoot.RequestImageAsync(this.ImageBinder, this, this);
                                        goto EVAL_STATE;
                                    }
                                    break;
                                case LayoutFarm.BinderState.Loading: { } break;
                                //
                                case LayoutFarm.BinderState.Loaded:
                                    {
                                        //check if we need scale or not

                                        Image img = this.ImageBinder.Image;
                                        if (this._imgW == 0 || this._imgH == 0)
                                        {
                                            //only X,and Y
                                            RenderQualtity prevQ = p.RenderQuality;
                                            p.RenderQuality = RenderQualtity.Fast;
                                            p.DrawImage(this.ImageBinder.Image, this._imgX, this._imgY);
                                            p.RenderQuality = prevQ;
                                        }
                                        else if (_imgW == img.Width && _imgH == img.Height)
                                        {
                                            RenderQualtity prevQ = p.RenderQuality;
                                            p.RenderQuality = RenderQualtity.Fast;
                                            p.DrawImage(this.ImageBinder.Image, this._imgX, this._imgY);
                                            p.RenderQuality = prevQ;
                                        }
                                        else
                                        {
                                            //TODO:  impl img scale here
                                            RenderQualtity prevQ = p.RenderQuality;
                                            p.RenderQuality = RenderQualtity.Fast;
                                            p.DrawImage(this.ImageBinder.Image, this._imgX, this._imgY);
                                            p.RenderQuality = prevQ;
                                        }
                                    }
                                    break;

                            }
                        }
                    }
                    break;
                case WellknownSvgElementName.Text:
                    {
                        //TODO: review here
                        //temp fix 
                        SvgTextSpec textSpec = this._visualSpec as SvgTextSpec;
                        if (textSpec != null)
                        {
                            Color prevColor = p.FillColor;
                            if (textSpec.HasFillColor)
                            {
                                p.FillColor = textSpec.FillColor;
                            }
                            if (!textSpec.FontSize.IsEmpty && textSpec.FontFace != null)
                            {
                                //TODO: review this with CssValue Parser again
                                //check if input text size is in point or pixel
                                if (textSpec.FontSize.UnitOrNames == LayoutFarm.Css.CssUnitOrNames.Points)
                                {
                                    p.CurrentFont = new RequestFont(
                                      textSpec.FontFace,
                                      textSpec.FontSize.Number);
                                }
                                else
                                {
                                    //assum pixel unit , so we convert it to point
                                    p.CurrentFont = new RequestFont(
                                      textSpec.FontFace,
                                      ConvPixelsToPoints(textSpec.FontSize.Number));
                                }
                                newFontReq = true;
                            }
                            else if (textSpec.FontFace != null)
                            {
                                if (textSpec.FontSize.UnitOrNames == LayoutFarm.Css.CssUnitOrNames.Points)
                                {
                                    p.CurrentFont = new RequestFont(
                                      textSpec.FontFace,
                                      textSpec.FontSize.Number);
                                }
                                else
                                {
                                    //assum pixel unit , so we convert it to point
                                    p.CurrentFont = new RequestFont(
                                      textSpec.FontFace,
                                      ConvPixelsToPoints(textSpec.FontSize.Number));
                                }
                                newFontReq = true;
                            }
                            else if (!textSpec.FontSize.IsEmpty)
                            {
                                p.CurrentFont = new RequestFont(
                                     currentFont.Name,
                                     textSpec.FontSize.Number); //TODO: number, size in pts vs in px
                                newFontReq = true;
                            }

                            p.DrawString(textSpec.TextContent, textSpec.ActualX, textSpec.ActualY);
                            p.FillColor = prevColor;//restore back
                                                    //change font or not
                        }
                    }
                    break;
                case WellknownSvgElementName.Path:
                case WellknownSvgElementName.Line:
                case WellknownSvgElementName.Ellipse:
                case WellknownSvgElementName.Circle:
                case WellknownSvgElementName.Polygon:
                case WellknownSvgElementName.Polyline:
                case WellknownSvgElementName.Rect:
                case WellknownSvgElementName.Marker:
                    {
                        //render with rect spec 

                        if (currentTx == null)
                        {
                            if (vgPainterArgs.ExternalVxsVisitHandler == null)
                            {
                                if (p.FillColor.A > 0)
                                {
                                    p.Fill(_vxsPath);
                                }
                                //to draw stroke
                                //stroke width must > 0 and stroke-color must not be transparent color
                                if (p.StrokeWidth > 0 && p.StrokeColor.A > 0)
                                {
                                    //has specific stroke color   
                                    //temp1
                                    //if (p.LineRenderingTech == LineRenderingTechnique.OutlineAARenderer)
                                    //{
                                    //    //TODO: review here again
                                    //    p.Draw(new VertexStoreSnap(_vxsPath.Vxs), p.StrokeColor);
                                    //}
                                    //else
                                    //{ 
                                    //check if we need to create a new stroke or not
                                    if (_strokeVxs == null || _latestStrokeW != (float)p.StrokeWidth)
                                    {
                                        //TODO: review here again***
                                        //vxs caching 

                                        _latestStrokeW = (float)p.StrokeWidth;
                                        _strokeVxs = GetStrokeVxsOrCreateNew(_vxsPath, (float)p.StrokeWidth);
                                        p.Fill(_strokeVxs, p.StrokeColor);
                                    }
                                    //}
                                }
                            }
                            else
                            {
                                vgPainterArgs.ExternalVxsVisitHandler(_vxsPath, vgPainterArgs);
                            }


                            //----------------------------------------------------------------------
                            if (this._pathMarkers != null)
                            {
                                //render each marker
                                if (_pathMarkers.StartMarker != null)
                                {
                                    //draw this
                                    //*** IMPORTANT : matrix transform order !***                 
                                    //*** IMPORTANT : matrix transform order !***    
                                    //Color prevFillColor = p.FillColor;
                                    //p.FillColor = Color.Red;
                                    int cc = _pathMarkers.StartMarker.ChildCount;
                                    for (int i = 0; i < cc; ++i)
                                    {
                                        //temp fix 
                                        if (_pathMarkers.StartMarkerAffine != null)
                                        {
                                            vgPainterArgs._currentTx = _pathMarkers.StartMarkerAffine * Affine.NewTranslation(_pathMarkers.StartMarkerPos.X, _pathMarkers.StartMarkerPos.Y);
                                        }
                                        else
                                        {
                                            vgPainterArgs._currentTx = Affine.NewTranslation(_pathMarkers.StartMarkerPos.X, _pathMarkers.StartMarkerPos.Y);
                                        }
                                        _pathMarkers.StartMarker.GetChildNode(i).Paint(vgPainterArgs);

                                    }
                                    //p.FillColor = prevFillColor;
                                    vgPainterArgs._currentTx = currentTx;
                                }

                                if (_pathMarkers.MidMarker != null)
                                {
                                    //draw this
                                    //vgPainterArgs._currentTx = Affine.IdentityMatrix;// _pathMarkers.StartMarkerAffine;
                                    //Color prevFillColor = p.FillColor;
                                    //p.FillColor = Color.Red;

                                    PointF[] allPoints = _pathMarkers.AllPoints;
                                    int allPointCount = allPoints.Length;
                                    if (allPointCount > 2)
                                    {
                                        //draw between first and last node

                                        for (int mm = 1; mm < allPointCount - 1; ++mm)
                                        {
                                            int cc = _pathMarkers.MidMarker.ChildCount;
                                            PointF btwPoint = allPoints[mm];
                                            for (int i = 0; i < cc; ++i)
                                            {
                                                //temp fix   

                                                if (_pathMarkers.MidMarkerAffine != null)
                                                {
                                                    vgPainterArgs._currentTx = _pathMarkers.MidMarkerAffine * Affine.NewTranslation(btwPoint.X, btwPoint.Y);
                                                }
                                                else
                                                {
                                                    vgPainterArgs._currentTx = Affine.NewTranslation(btwPoint.X, btwPoint.Y);
                                                }

                                                _pathMarkers.MidMarker.GetChildNode(i).Paint(vgPainterArgs);
                                            }
                                        }
                                    }

                                    //p.FillColor = prevFillColor;
                                    vgPainterArgs._currentTx = currentTx;
                                }

                                if (_pathMarkers.EndMarker != null)
                                {
                                    //draw this
                                    //vgPainterArgs._currentTx = Affine.IdentityMatrix;// _pathMarkers.StartMarkerAffine;
                                    //Color prevFillColor = p.FillColor;
                                    //p.FillColor = Color.Red;
                                    int cc = _pathMarkers.EndMarker.ChildCount;
                                    for (int i = 0; i < cc; ++i)
                                    {
                                        //temp fix 
                                        if (_pathMarkers.EndMarkerAffine != null)
                                        {
                                            vgPainterArgs._currentTx = _pathMarkers.EndMarkerAffine * Affine.NewTranslation(_pathMarkers.EndMarkerPos.X, _pathMarkers.EndMarkerPos.Y);
                                        }
                                        else
                                        {
                                            vgPainterArgs._currentTx = Affine.NewTranslation(_pathMarkers.EndMarkerPos.X, _pathMarkers.EndMarkerPos.Y);
                                        }
                                        _pathMarkers.EndMarker.GetChildNode(i).Paint(vgPainterArgs);
                                    }
                                    //p.FillColor = prevFillColor;
                                    vgPainterArgs._currentTx = currentTx;
                                }
                            }
                        }
                        else
                        {
                            //have some tx
                            using (VxsTemp.Borrow(out var v1))
                            {
                                currentTx.TransformToVxs(_vxsPath, v1);

                                if (vgPainterArgs.ExternalVxsVisitHandler == null)
                                {
                                    if (p.FillColor.A > 0)
                                    {
                                        p.Fill(v1);
                                    }

                                    //to draw stroke
                                    //stroke width must > 0 and stroke-color must not be transparent color 
                                    if (p.StrokeWidth > 0 && p.StrokeColor.A > 0)
                                    {
                                        //has specific stroke color  

                                        //if (this.LineRenderingTech == LineRenderingTechnique.OutlineAARenderer)
                                        //{
                                        //    p.Draw(new VertexStoreSnap(v1), p.StrokeColor);
                                        //}
                                        //else
                                        //{
                                        //TODO: review this again***

                                        VertexStore strokeVxs = GetStrokeVxsOrCreateNew(v1, (float)p.StrokeWidth);
                                        p.Fill(strokeVxs, p.StrokeColor);
                                        //}
                                    }
                                }
                                else
                                {
                                    vgPainterArgs.ExternalVxsVisitHandler(v1, vgPainterArgs);
                                }
                            }

                            if (this._pathMarkers != null)
                            {
                                //render each marker
                                if (_pathMarkers.StartMarker != null)
                                {
                                    //draw this
                                    //*** IMPORTANT : matrix transform order !***                 
                                    //*** IMPORTANT : matrix transform order !***                        

                                    vgPainterArgs._currentTx = _pathMarkers.StartMarkerAffine;
                                    _pathMarkers.StartMarker.Paint(vgPainterArgs);
                                    vgPainterArgs._currentTx = currentTx;

                                }
                                if (_pathMarkers.EndMarker != null)
                                {
                                    //draw this

                                }

                            }
                        }
                    }
                    break;
            }

            //-------------------------------------------------------
            int childCount = this.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                var node = GetChildNode(i) as SvgRenderElement;
                if (node != null)
                {
                    node.Paint(vgPainterArgs);
                }
            }

            //restore
            p.FillColor = color;
            p.StrokeColor = strokeColor;
            p.StrokeWidth = strokeW;
            //
            vgPainterArgs._currentTx = prevTx;
            if (hasClip)
            {
                p.SetClipRgn(null);
            }
            if (newFontReq)
            {
                p.CurrentFont = currentFont;
            }
        }


        public void AddChildElement(SvgRenderElementBase renderE)
        {
            if (renderE == null) return;
            //
#if DEBUG
            if (renderE.dbugHasParent)
            {
                throw new NotSupportedException();
            }
            renderE.dbugHasParent = true;
#endif
            if (_childNodes == null)
            {
                _childNodes = new List<SvgRenderElementBase>();
            }
            _childNodes.Add(renderE);

        }
        public int ChildCount
        {
            get
            {
                return (_childNodes == null) ? 0 : _childNodes.Count;
            }
        }
        public SvgRenderElementBase GetChildNode(int index)
        {
            return _childNodes[index];
        }

        public void RemoveAt(int index)
        {
            _childNodes.RemoveAt(index);
        }
        public void Clear()
        {
            if (_childNodes != null)
            {
                _childNodes.Clear();
            }
        }
    }

    public class SvgForeignNode : SvgRenderElementBase
    {
        public object _foriegnNode;
        public override SvgRenderElementBase Clone()
        {
            return new SvgForeignNode { _foriegnNode = this._foriegnNode };
        }

    }



    public class VgRenderVx : RenderVx
    {

        Image _backimg;
        RectD _boundRect;
        bool _needBoundUpdate;
        public SvgRenderElement _renderE;

        public VgRenderVx(SvgRenderElement svgRenderE)
        {
            _renderE = svgRenderE;
            _needBoundUpdate = true;

        }
        public VgRenderVx Clone()
        {
            return new VgRenderVx((SvgRenderElement)_renderE.Clone());
        }

        public void InvalidateBounds()
        {
            _needBoundUpdate = true;
            _boundRect = new RectD(this.X, this.Y, 2, 2);
        }

        public RectD GetBounds()
        {

            //***
            if (_needBoundUpdate)
            {
                VgPainterArgsPool.GetFreePainterArgs(null, out VgPaintArgs paintArgs);
                RectD rectTotal = RectD.ZeroIntersection;
                bool evaluated = false;

                paintArgs.ExternalVxsVisitHandler = (vxs, args) =>
                {
                    evaluated = true;//once 
                    BoundingRect.GetBoundingRect(vxs, ref rectTotal);
                };

                _renderE.Walk(paintArgs);
                VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);

                _needBoundUpdate = false;
                return this._boundRect = evaluated ? rectTotal : new RectD();
            }

            return this._boundRect;

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
    }


    public class SvgRenderVxDocBuilder
    {
        SvgDocument _svgdoc;
        List<SvgElement> _defsList = new List<SvgElement>();
        List<SvgElement> _styleList = new List<SvgElement>();


        MySvgPathDataParser _pathDataParser = new MySvgPathDataParser();
        CurveFlattener _curveFlatter = new CurveFlattener();


        List<SvgRenderElement> _waitingList = new List<SvgRenderElement>();
        Dictionary<string, SvgRenderElement> _registeredElemsById = new Dictionary<string, SvgRenderElement>();

        Dictionary<string, SvgRenderElement> _clipPathDic = new Dictionary<string, SvgRenderElement>();
        Dictionary<string, SvgRenderElement> _markerDic = new Dictionary<string, SvgRenderElement>();

        float _containerWidth = 500;//default?
        float _containerHeight = 500;//default?
        float _emHeight = 17;//default
        LayoutFarm.WebDom.CssActiveSheet _activeSheet1; //temp fix1 
        SvgRenderRootElement _renderRoot;

        public SvgRenderVxDocBuilder()
        {

        }

        public SvgRenderElement CreateSvgRenderElement(SvgDocument svgdoc, Action<SvgRenderElement> invalidate)
        {
            _svgdoc = svgdoc;
            _activeSheet1 = svgdoc.CssActiveSheet;

            _renderRoot = new SvgRenderRootElement();
            _renderRoot._invalidate = invalidate;

            // 
            //create visual element for the svg
            SvgElement rootElem = svgdoc.Root;
            SvgRenderElement rootSvgElem = new SvgRenderElement(WellknownSvgElementName.RootSvg, null, _renderRoot);
            int childCount = rootElem.ChildCount;

            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                CreateSvgRenderElement(rootSvgElem, rootElem.GetChild(i));
            }

            //resolve
            int j = _waitingList.Count;
            for (int i = 0; i < j; ++i)
            {
                //resolve
                SvgUseRenderElement useRenderE = (SvgUseRenderElement)_waitingList[i];
                if (useRenderE.HRefSvgRenderElement == null)
                {
                    //resolve
                    SvgUseSpec useSpec = (SvgUseSpec)useRenderE._visualSpec;
                    if (_registeredElemsById.TryGetValue(useSpec.Href.Value, out SvgRenderElement result))
                    {
                        useRenderE.HRefSvgRenderElement = result;
                    }
                }
            }

            return rootSvgElem;
        }
        public VgRenderVx CreateRenderVx(SvgDocument svgdoc, Action<SvgRenderElement> invalidateAction = null)
        {
            return new VgRenderVx(CreateSvgRenderElement(svgdoc, invalidateAction));
        }

        public void SetContainerSize(float width, float height)
        {
            _containerWidth = width;
            _containerHeight = height;
        }

        SvgRenderElement CreateSvgRenderElement(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement renderE = null;
            switch (elem.WellknowElemName)
            {
                default:
                    throw new KeyNotFoundException();
                //-----------------
                case WellknownSvgElementName.Defs:
                    _defsList.Add(elem);
                    return null;
                case WellknownSvgElementName.Style:
                    _styleList.Add(elem);
                    return null;
                case WellknownSvgElementName.Marker:
                    renderE = CreateMarker(parentNode, (SvgMarkerSpec)elem.ElemSpec);
                    break;
                //-----------------
                case WellknownSvgElementName.Unknown:
                    return null;
                case WellknownSvgElementName.Use:
                    return CreateUseElement(parentNode, (SvgUseSpec)elem.ElemSpec);
                case WellknownSvgElementName.Text:
                    return CreateTextElem(parentNode, (SvgTextSpec)elem.ElemSpec);
                case WellknownSvgElementName.Svg:
                    renderE = new SvgRenderElement(WellknownSvgElementName.Svg, (SvgVisualSpec)elem.ElemSpec, _renderRoot);
                    break;
                case WellknownSvgElementName.Rect:
                    renderE = CreateRect(parentNode, (SvgRectSpec)elem.ElemSpec);
                    break;
                case WellknownSvgElementName.Image:
                    renderE = CreateImage(parentNode, (SvgImageSpec)elem.ElemSpec);
                    break;
                case WellknownSvgElementName.Polyline:
                    renderE = CreatePolyline(parentNode, (SvgPolylineSpec)elem.ElemSpec);
                    break;
                case WellknownSvgElementName.Polygon:
                    renderE = CreatePolygon(parentNode, (SvgPolygonSpec)elem.ElemSpec);
                    break;
                case WellknownSvgElementName.Ellipse:
                    renderE = CreateEllipse(parentNode, (SvgEllipseSpec)elem.ElemSpec);
                    break;
                case WellknownSvgElementName.Circle:
                    renderE = CreateCircle(parentNode, (SvgCircleSpec)elem.ElemSpec);
                    break;
                case WellknownSvgElementName.Path:
                    renderE = CreatePath(parentNode, (SvgPathSpec)elem.ElemSpec);
                    return renderE;
                case WellknownSvgElementName.ClipPath:
                    renderE = CreateClipPath(parentNode, (SvgVisualSpec)elem.ElemSpec);
                    break;
                case WellknownSvgElementName.Group:
                    renderE = CreateGroup(parentNode, (SvgVisualSpec)elem.ElemSpec);
                    break;
            }

            if (renderE._visualSpec != null)
            {
                string id = renderE._visualSpec.Id;
                if (id != null)
                {
                    //replace duplicated item
                    _registeredElemsById[id] = renderE;
                }
            }

            renderE.SetController(elem);
            parentNode.AddChildElement(renderE);
            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                CreateSvgRenderElement(renderE, elem.GetChild(i));
            }

            return renderE;
        }

        SvgRenderElement CreateClipPath(SvgRenderElement parentNode, SvgVisualSpec visualSpec)
        {
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.ClipPath, visualSpec, _renderRoot);
            AssignAttributes(visualSpec);
            return renderE;
        }
        SvgRenderElement CreateMarker(SvgRenderElement parentNode, SvgMarkerSpec visualSpec)
        {
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Marker, visualSpec, _renderRoot);
            AssignAttributes(visualSpec);
            return renderE;
        }
        bool _buildDefs = false;
        void BuildDefinitionNodes()
        {
            if (_buildDefs)
            {
                return;
            }
            _buildDefs = true;


            SvgRenderElement definitionRoot = new SvgRenderElement(WellknownSvgElementName.Defs, null, _renderRoot);

            int j = _defsList.Count;
            for (int i = 0; i < j; ++i)
            {
                SvgElement defsElem = _defsList[i];
                //get definition content
                int childCount = defsElem.ChildCount;
                for (int c = 0; c < childCount; ++c)
                {
                    SvgElement child = defsElem.GetChild(c);
                    switch (child.WellknowElemName)
                    {
                        case WellknownSvgElementName.ClipPath:
                            {
                                //clip path definition  
                                //make this as a clip path 
                                SvgRenderElement renderE = CreateSvgRenderElement(definitionRoot, child);
                                _clipPathDic.Add(child.ElemSpecId, renderE);
                            }
                            break;
                        case WellknownSvgElementName.Marker:
                            {
                                //clip path definition  
                                //make this as a clip path 
                                SvgRenderElement renderE = CreateSvgRenderElement(definitionRoot, child);
                                _markerDic.Add(child.ElemSpecId, renderE);
                            }
                            break;
                    }
                }
            }
        }

        void AssignAttributes(SvgVisualSpec spec)
        {

            if (spec.ClipPathLink != null)
            {
                //resolve this clip
                BuildDefinitionNodes();
                if (_clipPathDic.TryGetValue(spec.ClipPathLink.Value, out SvgRenderElement clip))
                {
                    spec.ResolvedClipPath = clip;
                    //cmds.Add(clip);
                }
            }
        }
        SvgRenderElement CreatePath(SvgRenderElement parentNode, SvgPathSpec pathSpec)
        {
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, pathSpec, _renderRoot); //**

            //d             
            AssignAttributes(pathSpec);
            renderE._vxsPath = ParseSvgPathDefinitionToVxs(pathSpec.D.ToCharArray());
            ResolveMarkers(renderE, pathSpec);

            if (renderE._pathMarkers != null)
            {
                //create primary instance plan for this 

            }
            parentNode.AddChildElement(renderE);
            return renderE;
        }

        struct ReEvaluateArgs
        {
            public readonly float containerW;
            public readonly float containerH;
            public readonly float emHeight;

            public ReEvaluateArgs(float containerW, float containerH, float emHeight)
            {
                this.containerW = containerW;
                this.containerH = containerH;
                this.emHeight = emHeight;
            }
        }
        SvgRenderElement CreateEllipse(SvgRenderElement parentNode, SvgEllipseSpec ellipseSpec)
        {

            SvgRenderElement ellipseRenderE = new SvgRenderElement(WellknownSvgElementName.Ellipse, ellipseSpec, _renderRoot);
            VectorToolBox.GetFreeEllipseTool(out Ellipse ellipse);
            
            ReEvaluateArgs a = new ReEvaluateArgs(_containerWidth, _containerHeight, _emHeight); //temp fix

            double x = ConvertToPx(ellipseSpec.X, ref a);
            double y = ConvertToPx(ellipseSpec.Y, ref a);
            double rx = ConvertToPx(ellipseSpec.RadiusX, ref a);
            double ry = ConvertToPx(ellipseSpec.RadiusY, ref a);

            ellipse.Set(x, y, rx, ry);////TODO: review here => temp fix for ellipse step 
            using (VxsTemp.Borrow(out var v1))
            {
                ellipseRenderE._vxsPath = VertexSourceExtensions.MakeVxs(ellipse, v1).CreateTrim();
            }

            VectorToolBox.ReleaseEllipseTool(ref ellipse);

            AssignAttributes(ellipseSpec);

            return ellipseRenderE;
        }
        SvgRenderElement CreateImage(SvgRenderElement parentNode, SvgImageSpec imgspec)
        {
            SvgRenderElement img = new SvgRenderElement(WellknownSvgElementName.Image, imgspec, _renderRoot);
            VectorToolBox.GetFreeRectTool(out SimpleRect rectTool);

            ReEvaluateArgs a = new ReEvaluateArgs(_containerWidth, _containerHeight, _emHeight); //temp fix
            img._imgX = ConvertToPx(imgspec.X, ref a);
            img._imgY = ConvertToPx(imgspec.Y, ref a);
            img._imgW = ConvertToPx(imgspec.Width, ref a);
            img._imgH = ConvertToPx(imgspec.Height, ref a);


            rectTool.SetRect(
                img._imgX,
                img._imgY + img._imgH,
                img._imgX + img._imgW,
                img._imgY);


            //
            using (VxsTemp.Borrow(out var v1))
            {
                img._vxsPath = rectTool.MakeVxs(v1).CreateTrim();
            }
            VectorToolBox.ReleaseRectTool(ref rectTool);
            AssignAttributes(imgspec);

            return img;
        }
        SvgRenderElement CreatePolygon(SvgRenderElement parentNode, SvgPolygonSpec polygonSpec)
        {
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Polygon, polygonSpec, _renderRoot);

            PointF[] points = polygonSpec.Points;
            int j = points.Length;
            if (j > 1)
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                {
                    PointF p = points[0];
                    PointF p0 = p;
                    v1.AddMoveTo(p.X, p.Y);

                    for (int i = 1; i < j; ++i)
                    {
                        p = points[i];
                        v1.AddLineTo(p.X, p.Y);
                    }
                    //close
                    v1.AddMoveTo(p0.X, p0.Y);
                    v1.AddCloseFigure();

                    renderE._vxsPath = v1.CreateTrim();
                }
                AssignAttributes(polygonSpec);
                ResolveMarkers(renderE, polygonSpec);
                if (renderE._pathMarkers != null)
                {
                    //create primary instance plan for this 

                }
            }
            return renderE;
        }
        SvgRenderElement CreatePolyline(SvgRenderElement parentNode, SvgPolylineSpec polylineSpec)
        {
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Polyline, polylineSpec, _renderRoot);
            PointF[] points = polylineSpec.Points;
            int j = points.Length;
            if (j > 1)
            {
                using (VxsTemp.Borrow(out VertexStore v1))
                {
                    PointF p = points[0];
                    v1.AddMoveTo(p.X, p.Y);
                    for (int i = 1; i < j; ++i)
                    {
                        p = points[i];
                        v1.AddLineTo(p.X, p.Y);
                    }
                    renderE._vxsPath = v1.CreateTrim();
                }
                AssignAttributes(polylineSpec);

                //--------------------------------------------------------------------
                ResolveMarkers(renderE, polylineSpec);
                if (renderE._pathMarkers != null)
                {

                    //create primary instance plan for this polyline
                    SvgPathRenderMarkers pathMarkers = renderE._pathMarkers;
                    pathMarkers.AllPoints = points;

                    //start, mid, end
                    if (pathMarkers.StartMarker != null)
                    {
                        //turn marker to the start direction
                        PointF p0 = points[0];
                        PointF p1 = points[1];
                        //find rotation angle
                        double rotateRad = Math.Atan2(p0.Y - p1.Y, p0.X - p1.X);
                        SvgMarkerSpec markerSpec = (SvgMarkerSpec)pathMarkers.StartMarker._visualSpec;

                        //create local-transformation matrix
                        pathMarkers.StartMarkerPos = new PointF(p0.X, p0.Y);
                        pathMarkers.StartMarkerAffine = Affine.NewMatix(
                            AffinePlan.Translate(-markerSpec.RefX.Number, -markerSpec.RefY.Number), //move to the ref point
                            AffinePlan.Rotate(rotateRad) //rotate                            
                        );
                    }
                    //-------------------------------
                    if (pathMarkers.MidMarker != null)
                    {
                        SvgMarkerSpec markerSpec = (SvgMarkerSpec)pathMarkers.StartMarker._visualSpec;
                        pathMarkers.MidMarkerAffine = Affine.NewTranslation(-markerSpec.RefX.Number, -markerSpec.RefY.Number);
                    }
                    //-------------------------------
                    if (pathMarkers.EndMarker != null)
                    {
                        //turn marker to the start direction
                        PointF p0 = points[j - 2]; //before the last one
                        PointF p1 = points[j - 1];//the last one
                        //find rotation angle
                        double rotateRad = Math.Atan2(p1.Y - p0.Y, p1.X - p0.X);
                        SvgMarkerSpec markerSpec = (SvgMarkerSpec)pathMarkers.EndMarker._visualSpec;


                        //create local-transformation matrix
                        pathMarkers.EndMarkerPos = new PointF(p1.X, p1.Y);
                        pathMarkers.EndMarkerAffine = Affine.NewMatix(
                            AffinePlan.Translate(-markerSpec.RefX.Number, -markerSpec.RefY.Number), //move to the ref point
                            AffinePlan.Rotate(rotateRad) //rotate                            
                        );
                    }
                }

            }
            return renderE;
        }
        void ResolveMarkers(SvgRenderElement svgRenderE, IMayHaveMarkers mayHasMarkers)
        {
            //TODO: review here again***
            //assume marker link by id

            SvgPathRenderMarkers pathRenderMarkers = svgRenderE._pathMarkers;

            if (mayHasMarkers.MarkerStart != null)
            {
                if (pathRenderMarkers == null)
                {
                    svgRenderE._pathMarkers = pathRenderMarkers = new SvgPathRenderMarkers();
                }
                BuildDefinitionNodes();

                if (_markerDic.TryGetValue(mayHasMarkers.MarkerStart.Value, out SvgRenderElement marker))
                {
                    pathRenderMarkers.StartMarker = marker;
                }

            }
            if (mayHasMarkers.MarkerMid != null)
            {
                if (pathRenderMarkers == null)
                {
                    svgRenderE._pathMarkers = pathRenderMarkers = new SvgPathRenderMarkers();
                }
                BuildDefinitionNodes();

                if (_markerDic.TryGetValue(mayHasMarkers.MarkerMid.Value, out SvgRenderElement marker))
                {
                    pathRenderMarkers.MidMarker = marker;
                }
            }
            if (mayHasMarkers.MarkerEnd != null)
            {
                if (pathRenderMarkers == null)
                {
                    svgRenderE._pathMarkers = pathRenderMarkers = new SvgPathRenderMarkers();
                }
                BuildDefinitionNodes();

                if (_markerDic.TryGetValue(mayHasMarkers.MarkerEnd.Value, out SvgRenderElement marker))
                {
                    pathRenderMarkers.EndMarker = marker;
                }
            }
        }

        SvgRenderElement CreateCircle(SvgRenderElement parentNode, SvgCircleSpec cirSpec)
        {

            SvgRenderElement cir = new SvgRenderElement(WellknownSvgElementName.Circle, cirSpec, _renderRoot);
            VectorToolBox.GetFreeEllipseTool(out Ellipse ellipse);
            ReEvaluateArgs a = new ReEvaluateArgs(_containerWidth, _containerHeight, _emHeight); //temp fix
            double x = ConvertToPx(cirSpec.X, ref a);
            double y = ConvertToPx(cirSpec.Y, ref a);
            double r = ConvertToPx(cirSpec.Radius, ref a);

            ellipse.Set(x, y, r, r);////TODO: review here => temp fix for ellipse step 
            using (VxsTemp.Borrow(out var v1))
            {
                cir._vxsPath = VertexSourceExtensions.MakeVxs(ellipse, v1).CreateTrim();
            }

            VectorToolBox.ReleaseEllipseTool(ref ellipse);

            AssignAttributes(cirSpec);

            return cir;
        }


        SvgRenderElement CreateUseElement(SvgRenderElement parentNode, SvgUseSpec spec)
        {
            SvgUseRenderElement renderE = new SvgUseRenderElement(spec, _renderRoot);
            AssignAttributes(spec);
            if (spec.Href != null)
            {
                //add to waiting list
                _waitingList.Add(renderE);
            }

            //text x,y
            parentNode.AddChildElement(renderE);
            return renderE;
        }
        SvgRenderElement CreateTextElem(SvgRenderElement parentNode, SvgTextSpec textspec)
        {
            //text render element  
            SvgRenderElement textRenderElem = new SvgRenderElement(WellknownSvgElementName.Text, textspec, _renderRoot);
            //some att

            if (textspec.Class != null && _activeSheet1 != null)
            {
                //resolve style definition
                LayoutFarm.WebDom.CssRuleSetGroup ruleSetGroup = _activeSheet1.GetRuleSetForClassName(textspec.Class);
                if (ruleSetGroup != null)
                {
                    //assign
                    foreach (LayoutFarm.WebDom.CssPropertyDeclaration propDecl in ruleSetGroup.GetPropertyDeclIter())
                    {
                        switch (propDecl.WellknownPropertyName)
                        {
                            case LayoutFarm.WebDom.WellknownCssPropertyName.Font:
                                //set font detail 
                                break;
                            case LayoutFarm.WebDom.WellknownCssPropertyName.FontStyle:
                                //convert font style
                                break;
                            case LayoutFarm.WebDom.WellknownCssPropertyName.FontSize:
                                textspec.FontSize = propDecl.GetPropertyValue(0).AsLength();
                                break;
                            case LayoutFarm.WebDom.WellknownCssPropertyName.FontFamily:
                                textspec.FontFace = propDecl.GetPropertyValue(0).ToString();
                                break;
                            case LayoutFarm.WebDom.WellknownCssPropertyName.Fill:
                                textspec.FillColor = LayoutFarm.HtmlBoxes.CssValueParser2.ParseCssColor(propDecl.GetPropertyValue(0).ToString());
                                break;
                            case LayoutFarm.WebDom.WellknownCssPropertyName.Unknown:
                                {
                                    switch (propDecl.UnknownRawName)
                                    {
                                        case "fill":
                                            //svg 
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }


            ReEvaluateArgs a = new ReEvaluateArgs(_containerWidth, _containerHeight, _emHeight); //temp fix
            textspec.ActualX = ConvertToPx(textspec.X, ref a);
            textspec.ActualY = ConvertToPx(textspec.Y, ref a);


            AssignAttributes(textspec);

            //text x,y


            parentNode.AddChildElement(textRenderElem);
            return textRenderElem;
        }
        SvgRenderElement CreateRect(SvgRenderElement parentNode, SvgRectSpec rectSpec)
        {

            SvgRenderElement rect = new SvgRenderElement(WellknownSvgElementName.Rect, rectSpec, _renderRoot);

            if (!rectSpec.CornerRadiusX.IsEmpty || !rectSpec.CornerRadiusY.IsEmpty)
            {
                VectorToolBox.GetFreeRoundRectTool(out RoundedRect roundRect);
                ReEvaluateArgs a = new ReEvaluateArgs(_containerWidth, _containerHeight, _emHeight); //temp fix
                roundRect.SetRect(
                    ConvertToPx(rectSpec.X, ref a),
                    ConvertToPx(rectSpec.Y, ref a) + ConvertToPx(rectSpec.Height, ref a),
                    ConvertToPx(rectSpec.X, ref a) + ConvertToPx(rectSpec.Width, ref a),
                    ConvertToPx(rectSpec.Y, ref a));

                roundRect.SetRadius(ConvertToPx(rectSpec.CornerRadiusX, ref a), ConvertToPx(rectSpec.CornerRadiusY, ref a));

                using (VxsTemp.Borrow(out var v1))
                {

                    rect._vxsPath = roundRect.MakeVxs(v1).CreateTrim();
                }
                VectorToolBox.ReleaseRoundRect(ref roundRect);
            }
            else
            {
                VectorToolBox.GetFreeRectTool(out SimpleRect rectTool);
                ReEvaluateArgs a = new ReEvaluateArgs(_containerWidth, _containerHeight, _emHeight); //temp fix
                rectTool.SetRect(
                    ConvertToPx(rectSpec.X, ref a),
                    ConvertToPx(rectSpec.Y, ref a) + ConvertToPx(rectSpec.Height, ref a),
                    ConvertToPx(rectSpec.X, ref a) + ConvertToPx(rectSpec.Width, ref a),
                    ConvertToPx(rectSpec.Y, ref a));
                //
                using (VxsTemp.Borrow(out var v1))
                {
                    rect._vxsPath = rectTool.MakeVxs(v1).CreateTrim();
                }
                VectorToolBox.ReleaseRectTool(ref rectTool);
            }

            AssignAttributes(rectSpec);
            return rect;
        }

        static float ConvertToPx(LayoutFarm.Css.CssLength length, ref ReEvaluateArgs args)
        {
            //Return zero if no length specified, zero specified      
            switch (length.UnitOrNames)
            {
                case LayoutFarm.Css.CssUnitOrNames.EmptyValue:
                    return 0;
                case LayoutFarm.Css.CssUnitOrNames.Percent:
                    return (length.Number / 100f) * args.containerW;
                case LayoutFarm.Css.CssUnitOrNames.Ems:
                    return length.Number * args.emHeight;
                case LayoutFarm.Css.CssUnitOrNames.Ex:
                    return length.Number * (args.emHeight / 2);
                case LayoutFarm.Css.CssUnitOrNames.Pixels:
                    //atodo: check support for hi dpi
                    return length.Number;
                case LayoutFarm.Css.CssUnitOrNames.Milimeters:
                    return length.Number * 3.779527559f; //3 pixels per millimeter      
                case LayoutFarm.Css.CssUnitOrNames.Centimeters:
                    return length.Number * 37.795275591f; //37 pixels per centimeter 
                case LayoutFarm.Css.CssUnitOrNames.Inches:
                    return length.Number * 96f; //96 pixels per inch 
                case LayoutFarm.Css.CssUnitOrNames.Points:
                    return length.Number * (96f / 72f); // 1 point = 1/72 of inch   
                case LayoutFarm.Css.CssUnitOrNames.Picas:
                    return length.Number * 16f; // 1 pica = 12 points 
                default:
                    return 0;
            }
        }

        VertexStore ParseSvgPathDefinitionToVxs(char[] buffer)
        {
            using (VxsTemp.Borrow(out var flattenVxs))
            {
                VectorToolBox.GetFreePathWriter(out PathWriter pathWriter);
                _pathDataParser.SetPathWriter(pathWriter);
                _pathDataParser.Parse(buffer);
                _curveFlatter.MakeVxs(pathWriter.Vxs, flattenVxs);

                //create a small copy of the vxs 
                VectorToolBox.ReleasePathWriter(ref pathWriter);
                return flattenVxs.CreateTrim();
            }
        }
        SvgRenderElement CreateGroup(SvgRenderElement parentNode, SvgVisualSpec visSpec)
        {

            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Group, visSpec, _renderRoot);
            AssignAttributes(visSpec);
            return renderE;
        }
    }




    class MySvgPathDataParser : SvgPathDataParser
    {
        PathWriter _writer;
        public void SetPathWriter(PathWriter writer)
        {
            this._writer = writer;
            _writer.StartFigure();
        }
        protected override void OnArc(float r1, float r2, float xAxisRotation, int largeArcFlag, int sweepFlags, float x, float y, bool isRelative)
        {

            //TODO: implement arc again
            throw new NotSupportedException();
            //base.OnArc(r1, r2, xAxisRotation, largeArcFlag, sweepFlags, x, y, isRelative);
        }
        protected override void OnCloseFigure()
        {
            _writer.CloseFigure();

        }
        protected override void OnCurveToCubic(
            float x1, float y1,
            float x2, float y2,
            float x, float y, bool isRelative)
        {

            if (isRelative)
            {
                _writer.Curve4Rel(x1, y1, x2, y2, x, y);
            }
            else
            {
                _writer.Curve4(x1, y1, x2, y2, x, y);
            }
        }
        protected override void OnCurveToCubicSmooth(float x2, float y2, float x, float y, bool isRelative)
        {
            if (isRelative)
            {
                _writer.SmoothCurve4Rel(x2, y2, x, y);
            }
            else
            {
                _writer.SmoothCurve4(x2, y2, x, y);
            }

        }
        protected override void OnCurveToQuadratic(float x1, float y1, float x, float y, bool isRelative)
        {
            if (isRelative)
            {
                _writer.Curve3Rel(x1, y1, x, y);
            }
            else
            {
                _writer.Curve3(x1, y1, x, y);
            }
        }
        protected override void OnCurveToQuadraticSmooth(float x, float y, bool isRelative)
        {
            if (isRelative)
            {
                _writer.SmoothCurve3Rel(x, y);
            }
            else
            {
                _writer.SmoothCurve3(x, y);
            }

        }
        protected override void OnHLineTo(float x, bool relative)
        {
            if (relative)
            {
                _writer.HorizontalLineToRel(x);
            }
            else
            {
                _writer.HorizontalLineTo(x);
            }
        }

        protected override void OnLineTo(float x, float y, bool relative)
        {
            if (relative)
            {
                _writer.LineToRel(x, y);
            }
            else
            {
                _writer.LineTo(x, y);
            }
        }
        protected override void OnMoveTo(float x, float y, bool relative)
        {

            if (relative)
            {
                _writer.MoveToRel(x, y);
            }
            else
            {


                _writer.MoveTo(x, y);
            }
        }
        protected override void OnVLineTo(float y, bool relative)
        {
            if (relative)
            {
                _writer.VerticalLineToRel(y);
            }
            else
            {
                _writer.VerticalLineTo(y);
            }
        }
    }
}