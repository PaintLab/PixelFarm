//----------------------------------------------------------------------------
//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PaintLab.Svg;
using PixelFarm.Drawing;
using LayoutFarm.Svg;
using LayoutFarm.Svg.Pathing;
using PixelFarm.CpuBlit.VertexProcessing;


namespace PixelFarm.CpuBlit
{
    enum ClipingTechnique
    {
        None,
        ClipMask,
        ClipSimpleRect
    }

    public class SvgPainter
    {
        public Painter P;
        public Affine _currentTx;
    }

    public abstract class SvgRenderElementBase
    {
        public virtual void Paint(SvgPainter p)
        {
            //paint with painter interface
        }
#if DEBUG
        public bool dbugHasParent;
#endif
    }
    public class SvgTextNode : SvgRenderElementBase
    {
        public string TextContent { get; set; }
    }

    public class SvgHitTestArgs
    {
        public float X { get; set; }
        public float Y { get; set; }
        public bool WithSubPartTest { get; set; }
        //
        public bool Result { get; set; }
        public void Reset()
        {
            X = Y = 0;
            WithSubPartTest = false;
        }
    }
    public class SvgRenderElement : SvgRenderElementBase
    {
        public VgCmdPath _vxsPath;
        List<SvgRenderElementBase> _childNodes = null;
        WellknownSvgElementName _wellknownName;
        object _controller;
        public SvgVisualSpec _visualSpec;
        public SvgRenderElement(WellknownSvgElementName wellknownName, SvgVisualSpec visualSpec)
        {
            _wellknownName = wellknownName;
            _visualSpec = visualSpec;

        }
        public WellknownSvgElementName ElemName
        {
            get { return _wellknownName; }
        }

        public void SetController(object o)
        {
            _controller = o;
        }

        public void HitTest(SvgHitTestArgs hitArgs)
        {

        }
        //--------------------------------------------------------------------------
        //void Render(VgRenderVx renderVx)
        //{
        //    if (renderVx.HasBitmapSnapshot)
        //    {
        //        this.DrawImage(renderVx.BackingImage, renderVx.X, renderVx.Y);
        //        return;
        //    }

        //    Affine currentTx = null;
        //    var renderState = new TempVgRenderState();
        //    renderState.strokeColor = this.StrokeColor;
        //    this.StrokeWidth = renderState.strokeWidth = 1;//default  
        //    renderState.fillColor = this.FillColor;
        //    renderState.affineTx = currentTx;
        //    renderState.clippingTech = ClipingTechnique.None;
        //    //------------------ 

        //    int j = renderVx.VgCmdCount;

        //    TempVgRenderStateStore.GetFreeTempVgRenderState(out Stack<TempVgRenderState> vgStateStack);
        //    int i = 0;

        //    if (renderVx.PrefixCommand != null)
        //    {
        //        i = -1;
        //    }

        //    VgCmd vx = null;

        //    for (; i < j; ++i)
        //    {
        //        if (i < 0)
        //        {
        //            vx = renderVx.PrefixCommand;
        //        }
        //        else
        //        {
        //            vx = renderVx.GetVgCmd(i);
        //        }

        //        switch (vx.Name)
        //        {
        //            case VgCommandName.BeginGroup:
        //                {
        //                    //1. save current state before enter new state 
        //                    vgStateStack.Push(renderState);//save prev state
        //                    renderState.clippingTech = ClipingTechnique.None;//reset for this
        //                }
        //                break;
        //            case VgCommandName.EndGroup:
        //                {
        //                    switch (renderState.clippingTech)
        //                    {
        //                        case ClipingTechnique.None: break;
        //                        case ClipingTechnique.ClipMask:
        //                            {
        //                                //clear mask filter                               
        //                                //remove from current clip

        //                                this.EnableBuiltInMaskComposite = false;
        //                                this.TargetBufferName = TargetBufferName.AlphaMask;//swicth to mask buffer
        //                                this.Clear(Color.Black);
        //                                this.TargetBufferName = TargetBufferName.Default;
        //                            }
        //                            break;
        //                        case ClipingTechnique.ClipSimpleRect:
        //                            {
        //                                this.SetClipBox(0, 0, this.Width, this.Height);
        //                            }
        //                            break;
        //                    }

        //                    //restore to prev state
        //                    renderState = vgStateStack.Pop();

        //                    this.FillColor = renderState.fillColor;
        //                    this.StrokeColor = renderState.strokeColor;
        //                    this.StrokeWidth = renderState.strokeWidth;
        //                    currentTx = renderState.affineTx;

        //                }
        //                break;
        //            case VgCommandName.FillColor:
        //                this.FillColor = renderState.fillColor = ((VgCmdFillColor)vx).Color;
        //                break;
        //            case VgCommandName.StrokeColor:
        //                this.StrokeColor = renderState.strokeColor = ((VgCmdStrokeColor)vx).Color;
        //                break;
        //            case VgCommandName.StrokeWidth:
        //                this.StrokeWidth = renderState.strokeWidth = ((VgCmdStrokeWidth)vx).Width;
        //                break;
        //            case VgCommandName.AffineTransform:
        //                {
        //                    //apply this to current tx 
        //                    if (currentTx != null)
        //                    {
        //                        //*** IMPORTANT : matrix transform order !***
        //                        currentTx = ((VgCmdAffineTransform)vx).TransformMatrix * currentTx;
        //                    }
        //                    else
        //                    {
        //                        currentTx = ((VgCmdAffineTransform)vx).TransformMatrix;
        //                    }
        //                    renderState.affineTx = currentTx;
        //                }
        //                break;
        //            case VgCommandName.ClipPath:
        //                {
        //                    //clip-path

        //                    VgCmdClipPath clipPath = (VgCmdClipPath)vx;
        //                    VertexStore clipVxs = ((VgCmdPath)clipPath._svgParts[0]).Vxs;

        //                    //----------
        //                    //for optimization check if clip path is Rect
        //                    //if yes => do simple rect clip 

        //                    if (currentTx != null)
        //                    {
        //                        //have some tx
        //                        using (VxsContext.Temp(out var v1))
        //                        {
        //                            currentTx.TransformToVxs(clipVxs, v1);
        //                            //after transform
        //                            //check if v1 is rect clip or not
        //                            //if yes => then just use simple rect clip
        //                            if (SimpleRectClipEvaluator.EvaluateRectClip(v1, out RectangleF clipRect))
        //                            {
        //                                //use simple rect technique
        //                                this.SetClipBox((int)clipRect.X, (int)clipRect.Y, (int)clipRect.Right, (int)clipRect.Bottom);
        //                                renderState.clippingTech = ClipingTechnique.ClipSimpleRect;

        //                            }
        //                            else
        //                            {
        //                                //not simple rect => 
        //                                //use mask technique

        //                                renderState.clippingTech = ClipingTechnique.ClipMask;
        //                                this.TargetBufferName = TargetBufferName.AlphaMask;
        //                                //aggPainter.TargetBufferName = TargetBufferName.Default; //for debug
        //                                var prevColor = this.FillColor;
        //                                this.FillColor = Color.White;
        //                                //aggPainter.StrokeColor = Color.Black; //for debug
        //                                //aggPainter.StrokeWidth = 1; //for debug  
        //                                //p.Draw(v1); //for debug
        //                                this.Fill(v1);

        //                                this.FillColor = prevColor;
        //                                this.TargetBufferName = TargetBufferName.Default;//swicth to default buffer
        //                                this.EnableBuiltInMaskComposite = true;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //aggPainter.Draw(clipVxs); //for debug 
        //                        //check if clipVxs is rect or not
        //                        if (SimpleRectClipEvaluator.EvaluateRectClip(clipVxs, out RectangleF clipRect))
        //                        {
        //                            //use simple rect technique
        //                            this.SetClipBox((int)clipRect.X, (int)clipRect.Y, (int)clipRect.Right, (int)clipRect.Bottom);
        //                            renderState.clippingTech = ClipingTechnique.ClipSimpleRect;
        //                        }
        //                        else
        //                        {
        //                            //not simple rect => 
        //                            //use mask technique
        //                            renderState.clippingTech = ClipingTechnique.ClipMask;

        //                            this.TargetBufferName = TargetBufferName.AlphaMask;
        //                            //aggPainter.TargetBufferName = TargetBufferName.Default; //for debug
        //                            var prevColor = this.FillColor;
        //                            this.FillColor = Color.White;
        //                            //aggPainter.StrokeColor = Color.Black; //for debug
        //                            //aggPainter.StrokeWidth = 1; //for debug 

        //                            //p.Draw(v1); //for debug
        //                            this.Fill(clipVxs);
        //                            this.FillColor = prevColor;
        //                            this.TargetBufferName = TargetBufferName.Default;//swicth to default buffer
        //                            this.EnableBuiltInMaskComposite = true;
        //                        }
        //                    }

        //                }
        //                break;

        //            case VgCommandName.Path:
        //                {
        //                    VgCmdPath path = (VgCmdPath)vx;
        //                    VertexStore vxs = path.Vxs;


        //                    if (currentTx == null)
        //                    {
        //                        if (renderState.fillColor.A > 0)
        //                        {
        //                            this.Fill(vxs);
        //                        }
        //                        //to draw stroke
        //                        //stroke width must > 0 and stroke-color must not be transparent color

        //                        if (renderState.strokeWidth > 0 && renderState.strokeColor.A > 0)
        //                        {
        //                            //has specific stroke color  

        //                            if (this.LineRenderingTech == LineRenderingTechnique.OutlineAARenderer)
        //                            {
        //                                //TODO: review here again
        //                                this.Draw(new VertexStoreSnap(vxs), renderState.strokeColor);
        //                            }
        //                            else
        //                            {
        //                                VertexStore strokeVxs = GetStrokeVxsOrCreateNew(vxs, (float)this.StrokeWidth);
        //                                this.Fill(strokeVxs, renderState.strokeColor);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //have some tx
        //                        using (VxsContext.Temp(out var v1))
        //                        {
        //                            currentTx.TransformToVxs(vxs, v1);
        //                            if (renderState.fillColor.A > 0)
        //                            {
        //                                this.Fill(v1);
        //                            }

        //                            //to draw stroke
        //                            //stroke width must > 0 and stroke-color must not be transparent color 
        //                            if (renderState.strokeWidth > 0 && renderState.strokeColor.A > 0)
        //                            {
        //                                //has specific stroke color  

        //                                if (this.LineRenderingTech == LineRenderingTechnique.OutlineAARenderer)
        //                                {
        //                                    this.Draw(new VertexStoreSnap(v1), renderState.strokeColor);
        //                                }
        //                                else
        //                                {
        //                                    VertexStore strokeVxs = GetStrokeVxsOrCreateNew(v1, (float)this.StrokeWidth);
        //                                    this.Fill(strokeVxs, renderState.strokeColor);
        //                                }
        //                            }
        //                        }
        //                    }

        //                }
        //                break;
        //        }
        //    }
        //    TempVgRenderStateStore.ReleaseTempVgRenderState(ref vgStateStack);
        //}

        static PixelFarm.CpuBlit.VertexProcessing.Affine CreateAffine(SvgTransform transformation)
        {
            switch (transformation.TransformKind)
            {
                default: throw new NotSupportedException();

                case SvgTransformKind.Matrix:

                    SvgTransformMatrix matrixTx = (SvgTransformMatrix)transformation;
                    float[] elems = matrixTx.Elements;
                    return new VertexProcessing.Affine(
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
                        return VertexProcessing.Affine.NewMatix(
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

            using (VxsContext.Temp(out var v1))
            {
                PixelFarm.CpuBlit.TempStrokeTool.GetFreeStroke(out Stroke stroke);
                stroke.Width = strokeW;
                stroke.MakeVxs(vxs, v1);
                VertexStore vx = v1.CreateTrim();
                PixelFarm.CpuBlit.TempStrokeTool.ReleaseStroke(ref stroke);
                return vx;
            }
        }

        public override void Paint(SvgPainter svgPainter)
        {
            //save
            Painter p = svgPainter.P;
            Color color = p.FillColor;
            double strokeW = p.StrokeWidth;
            Color strokeColor = p.StrokeColor;

            VertexProcessing.Affine prevTx = svgPainter._currentTx; //backup
            VertexProcessing.Affine currentTx = svgPainter._currentTx;
            bool hasClip = false;

            if (_visualSpec != null)
            {

                if (_visualSpec.Transform != null)
                {
                    VertexProcessing.Affine latest = CreateAffine(_visualSpec.Transform);
                    if (currentTx != null)
                    {
                        //*** IMPORTANT : matrix transform order !***                         
                        currentTx = latest * svgPainter._currentTx;
                    }
                    else
                    {
                        currentTx = latest;
                    }
                    svgPainter._currentTx = currentTx;
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
                    VertexStore clipVxs = ((SvgRenderElement)clipPath.GetChildNode(0))._vxsPath.Vxs;
                    //VertexStore clipVxs = ((VgCmdPath)clipPath._svgParts[0]).Vxs;

                    //----------
                    //for optimization check if clip path is Rect
                    //if yes => do simple rect clip 

                    if (currentTx != null)
                    {
                        //have some tx
                        using (VxsContext.Temp(out var v1))
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
                            if (p.FillColor.A > 0)
                            {
                                p.Fill(_vxsPath.Vxs);
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
                                VertexStore strokeVxs = GetStrokeVxsOrCreateNew(
                                    _vxsPath.Vxs,
                                    (float)p.StrokeWidth);
                                p.Fill(strokeVxs, p.StrokeColor);
                                //}
                            }
                        }
                        else
                        {
                            //have some tx
                            using (VxsContext.Temp(out var v1))
                            {
                                currentTx.TransformToVxs(_vxsPath.Vxs, v1);
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
                                    VertexStore strokeVxs = GetStrokeVxsOrCreateNew(v1, (float)p.StrokeWidth);
                                    p.Fill(strokeVxs, p.StrokeColor);
                                    //}
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
                var node = GetChildNode(i) as PixelFarm.CpuBlit.SvgRenderElement;
                if (node != null)
                {
                    node.Paint(svgPainter);
                }
            }

            //restore
            p.FillColor = color;
            p.StrokeColor = strokeColor;
            p.StrokeWidth = strokeW;
            //
            svgPainter._currentTx = prevTx;
            if (hasClip)
            {
                p.SetClipRgn(null);
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


    }




    public class SvgRenderVxDocBuilder
    {
        SvgDocument _svgdoc;
        List<SvgElement> _defsList = new List<SvgElement>();
        MySvgPathDataParser _pathDataParser = new MySvgPathDataParser();
        VertexProcessing.CurveFlattener _curveFlatter = new VertexProcessing.CurveFlattener();

        Dictionary<string, SvgRenderElement> _clipPathDic = new Dictionary<string, SvgRenderElement>();

        public VgRenderVx CreateRenderVx(SvgDocument svgdoc)
        {
            _svgdoc = svgdoc;

            //create visual element for the svg
            SvgElement rootElem = svgdoc.Root;
            SvgRenderElement rootSvgElem = new SvgRenderElement(WellknownSvgElementName.RootSvg, null);
            rootElem.SetVisualElement(rootSvgElem);

            int childCount = rootElem.ChildCount;

            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                EvalOtherElem(rootSvgElem, rootElem.GetChild(i));
            }

            //
            VgRenderVx renderVx = new VgRenderVx(rootSvgElem);
            return renderVx;
        }
        SvgRenderElement EvalOtherElem(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement renderE = null;
            switch (elem.WellknowElemName)
            {
                default:
                    throw new KeyNotFoundException();
                case WellknownSvgElementName.Unknown:
                    return null;
                case WellknownSvgElementName.Text:
                    {
                        //text node of the parent

                        return null;
                    }
                case WellknownSvgElementName.Svg:
                    renderE = new SvgRenderElement(WellknownSvgElementName.Svg, null);
                    break;
                case WellknownSvgElementName.Defs:
                    _defsList.Add(elem);
                    return null;
                case WellknownSvgElementName.Rect:
                    renderE = EvalRect(parentNode, elem);
                    break;
                case WellknownSvgElementName.Image:
                    renderE = EvalImage(parentNode, elem);
                    break;
                case WellknownSvgElementName.Polyline:
                    renderE = EvalPolyline(parentNode, elem);
                    break;
                case WellknownSvgElementName.Polygon:
                    renderE = EvalPolygon(parentNode, elem);
                    break;
                case WellknownSvgElementName.Ellipse:
                    renderE = EvalEllipse(parentNode, elem);
                    break;
                case WellknownSvgElementName.Circle:
                    renderE = EvalCircle(parentNode, elem);
                    break;
                case WellknownSvgElementName.Path:
                    renderE = EvalPath(parentNode, elem);
                    return renderE;
                case WellknownSvgElementName.ClipPath:
                    renderE = EvalClipPath(parentNode, elem);
                    return renderE;
                case WellknownSvgElementName.Group:
                    renderE = EvalGroup(parentNode, elem);
                    return renderE;
            }

            parentNode.AddChildElement(renderE);
            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                EvalOtherElem(renderE, elem.GetChild(i));
            }

            return renderE;

        }
        SvgRenderElement EvalClipPath(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement clipPath = new SvgRenderElement(WellknownSvgElementName.ClipPath, elem._visualSpec);
            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                SvgRenderElement path = EvalOtherElem(clipPath, elem.GetChild(i));
#if DEBUG
                if (!path.dbugHasParent)
                {
                    clipPath.AddChildElement(path);
                }
#endif

            }
            parentNode.AddChildElement(clipPath);
            return clipPath;
        }
        bool _buildDefs = false;
        void BuildDefinitionNodes()
        {
            if (_buildDefs)
            {
                return;
            }
            _buildDefs = true;


            SvgRenderElement definitionRoot = new SvgRenderElement(WellknownSvgElementName.Defs, null);

            int j = _defsList.Count;
            for (int i = 0; i < j; ++i)
            {
                SvgElement defsElem = _defsList[i];
                //get definition content
                int childCount = defsElem.ChildCount;
                for (int c = 0; c < childCount; ++c)
                {
                    SvgElement child = defsElem.GetChild(c);
                    if (child.WellknowElemName == WellknownSvgElementName.ClipPath)
                    {
                        //clip path definition  
                        //make this as a clip path 
                        SvgRenderElement renderE = EvalOtherElem(definitionRoot, child);
                        _clipPathDic.Add(child._visualSpec.Id, renderE);
                    }
                }
            }
        }

        void AssignAttributes(SvgVisualSpec spec)
        {
            //if (spec.HasFillColor)
            //{
            //    cmds.Add(new VgCmdFillColor(spec.FillColor));
            //}
            //if (spec.HasStrokeColor)
            //{
            //    cmds.Add(new VgCmdStrokeColor(spec.StrokeColor));
            //}
            //if (spec.HasStrokeWidth)
            //{
            //    cmds.Add(new VgCmdStrokeWidth(spec.StrokeWidth.Number));
            //}
            //if (spec.Transform != null)
            //{
            //    //convert from svg transform to  
            //    cmds.Add(new VgCmdAffineTransform(CreateAffine(spec.Transform)));
            //}
            //

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
        SvgRenderElement EvalPath(SvgRenderElement parentNode, SvgElement elem)
        {

            SvgRenderElement path = new SvgRenderElement(WellknownSvgElementName.Path, elem._visualSpec); //**
            SvgPathSpec pathSpec = elem._visualSpec as SvgPathSpec;
            //d             
            VgCmdPath pathCmd = new VgCmdPath();
            pathCmd.SetVxs(ParseSvgPathDefinitionToVxs(pathSpec.D.ToCharArray()));
            AssignAttributes(pathSpec);

            path._vxsPath = pathCmd;

            parentNode.AddChildElement(path);
            return path;
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
        SvgRenderElement EvalEllipse(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement ellipseRenderE = new SvgRenderElement(WellknownSvgElementName.Ellipse, elem._visualSpec);
            SvgEllipseSpec ellipseSpec = elem._visualSpec as SvgEllipseSpec;
            VgCmdPath pathCmd = new VgCmdPath();
            ellipseRenderE._vxsPath = pathCmd;
            VectorToolBox.GetFreeEllipseTool(out VertexProcessing.Ellipse ellipse);
            ReEvaluateArgs a = new ReEvaluateArgs(500, 500, 17); //temp fix

            double x = ConvertToPx(ellipseSpec.X, ref a);
            double y = ConvertToPx(ellipseSpec.Y, ref a);
            double rx = ConvertToPx(ellipseSpec.RadiusX, ref a);
            double ry = ConvertToPx(ellipseSpec.RadiusY, ref a);

            ellipse.Set(x, y, rx, ry);////TODO: review here => temp fix for ellipse step 
            using (VxsContext.Temp(out var v1))
            {
                pathCmd.SetVxs(
                    PixelFarm.CpuBlit.VertexProcessing.VertexSourceExtensions.MakeVxs(ellipse, v1).CreateTrim());
            }

            VectorToolBox.ReleaseEllipseTool(ref ellipse);

            AssignAttributes(ellipseSpec);

            return ellipseRenderE;
        }
        SvgRenderElement EvalImage(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement img = new SvgRenderElement(WellknownSvgElementName.Image, elem._visualSpec);
            SvgImageSpec imgspec = elem._visualSpec as SvgImageSpec;


            VectorToolBox.GetFreeRectTool(out VertexProcessing.SimpleRect rectTool);

            ReEvaluateArgs a = new ReEvaluateArgs(500, 500, 17);//temp fix
            rectTool.SetRect(
                ConvertToPx(imgspec.X, ref a),
                ConvertToPx(imgspec.Y, ref a) + ConvertToPx(imgspec.Height, ref a),
                ConvertToPx(imgspec.X, ref a) + ConvertToPx(imgspec.Width, ref a),
                ConvertToPx(imgspec.Y, ref a));
            //
            using (VxsContext.Temp(out var v1))
            {
                //imgCmd.SetVxsAsOriginal(rectTool.MakeVxs(v1).CreateTrim());
            }
            VectorToolBox.ReleaseRectTool(ref rectTool);
            AssignAttributes(imgspec);

            return img;
        }
        SvgRenderElement EvalPolygon(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement polygon = new SvgRenderElement(WellknownSvgElementName.Polygon, elem._visualSpec);

            SvgPolygonSpec polygonSpec = elem._visualSpec as SvgPolygonSpec;

            VgCmdPath pathCmd = new VgCmdPath();
            polygon._vxsPath = pathCmd;



            PointF[] points = polygonSpec.Points;
            int j = points.Length;
            if (j > 1)
            {
                using (VxsContext.Temp(out VertexStore v1))
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

                    pathCmd.SetVxs(v1.CreateTrim());
                }
                AssignAttributes(polygonSpec);
            }
            return polygon;
        }
        SvgRenderElement EvalPolyline(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Polyline, elem._visualSpec);
            SvgPolylineSpec polylineSpec = elem._visualSpec as SvgPolylineSpec;
            VgCmdPath pathCmd = new VgCmdPath();
            renderE._vxsPath = pathCmd;
            PointF[] points = polylineSpec.Points;
            int j = points.Length;
            if (j > 1)
            {
                using (VxsContext.Temp(out VertexStore v1))
                {
                    PointF p = points[0];
                    v1.AddMoveTo(p.X, p.Y);
                    for (int i = 1; i < j; ++i)
                    {
                        p = points[i];
                        v1.AddLineTo(p.X, p.Y);
                    }
                    pathCmd.SetVxs(v1.CreateTrim());
                }

                AssignAttributes(polylineSpec);
            }
            return renderE;
        }
        SvgRenderElement EvalCircle(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement cir = new SvgRenderElement(WellknownSvgElementName.Circle, elem._visualSpec);
            SvgCircleSpec ellipseSpec = elem._visualSpec as SvgCircleSpec;

            VgCmdPath pathCmd = new VgCmdPath();
            cir._vxsPath = pathCmd;
            VectorToolBox.GetFreeEllipseTool(out VertexProcessing.Ellipse ellipse);
            ReEvaluateArgs a = new ReEvaluateArgs(500, 500, 17); //temp fix
            double x = ConvertToPx(ellipseSpec.X, ref a);
            double y = ConvertToPx(ellipseSpec.Y, ref a);
            double r = ConvertToPx(ellipseSpec.Radius, ref a);

            ellipse.Set(x, y, r, r);////TODO: review here => temp fix for ellipse step 
            using (VxsContext.Temp(out var v1))
            {
                pathCmd.SetVxs(
                    PixelFarm.CpuBlit.VertexProcessing.VertexSourceExtensions.MakeVxs(ellipse, v1).CreateTrim());
            }

            VectorToolBox.ReleaseEllipseTool(ref ellipse);

            AssignAttributes(ellipseSpec);

            return cir;
        }


        SvgRenderElement EvalRect(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement rect = new SvgRenderElement(WellknownSvgElementName.Rect, elem._visualSpec);
            SvgRectSpec rectSpec = elem._visualSpec as SvgRectSpec;

            VgCmdPath pathCmd = new VgCmdPath();
            rect._vxsPath = pathCmd;

            if (!rectSpec.CornerRadiusX.IsEmpty || !rectSpec.CornerRadiusY.IsEmpty)
            {
                VectorToolBox.GetFreeRoundRectTool(out VertexProcessing.RoundedRect roundRect);
                ReEvaluateArgs a = new ReEvaluateArgs(500, 500, 17); //temp fix
                roundRect.SetRect(
                    ConvertToPx(rectSpec.X, ref a),
                    ConvertToPx(rectSpec.Y, ref a) + ConvertToPx(rectSpec.Height, ref a),
                    ConvertToPx(rectSpec.X, ref a) + ConvertToPx(rectSpec.Width, ref a),
                    ConvertToPx(rectSpec.Y, ref a));

                roundRect.SetRadius(ConvertToPx(rectSpec.CornerRadiusX, ref a), ConvertToPx(rectSpec.CornerRadiusY, ref a));

                using (VxsContext.Temp(out var v1))
                {
                    pathCmd.SetVxs(roundRect.MakeVxs(v1).CreateTrim());
                }
                VectorToolBox.ReleaseRoundRect(ref roundRect);
            }
            else
            {
                VectorToolBox.GetFreeRectTool(out VertexProcessing.SimpleRect rectTool);
                ReEvaluateArgs a = new ReEvaluateArgs(500, 500, 17);//temp fix
                rectTool.SetRect(
                    ConvertToPx(rectSpec.X, ref a),
                    ConvertToPx(rectSpec.Y, ref a) + ConvertToPx(rectSpec.Height, ref a),
                    ConvertToPx(rectSpec.X, ref a) + ConvertToPx(rectSpec.Width, ref a),
                    ConvertToPx(rectSpec.Y, ref a));
                //
                using (VxsContext.Temp(out var v1))
                {
                    pathCmd.SetVxs(rectTool.MakeVxs(v1).CreateTrim());
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
            using (VxsContext.Temp(out var flattenVxs))
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



        SvgRenderElement EvalGroup(SvgRenderElement parentNode, SvgElement elem)
        {
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Group, elem._visualSpec);

            AssignAttributes(elem._visualSpec);
            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                SvgRenderElement child = EvalOtherElem(renderE, elem.GetChild(i));
                if (child != null)
                {
                    if (!child.dbugHasParent)
                    {
                        renderE.AddChildElement(child);
                    }
                }
            }

            parentNode.AddChildElement(renderE);
            return renderE;
        }
    }
    public static class SvgRenderVxDocBuilderExt
    {
        public static VgRenderVx CreateRenderVx(this SvgDocument svgdoc)
        {
            //create svg render vx from svgdoc
            //resolve the svg 
            SvgRenderVxDocBuilder builder = new SvgRenderVxDocBuilder();
            return builder.CreateRenderVx(svgdoc);
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