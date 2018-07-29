//----------------------------------------------------------------------------
//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PaintLab.Svg;
using PixelFarm.Drawing;
using LayoutFarm.Svg;
using LayoutFarm.Svg.Pathing;

namespace PixelFarm.CpuBlit
{
    public class SvgRenderVxDocBuilder
    {
        SvgDocument _svgdoc;
        List<SvgElement> _defsList = new List<SvgElement>();
        MySvgPathDataParser _pathDataParser = new MySvgPathDataParser();
        PixelFarm.CpuBlit.VertexProcessing.CurveFlattener _curveFlatter = new VertexProcessing.CurveFlattener();

        Dictionary<string, VgCmdClipPath> _clipPathDic = new Dictionary<string, VgCmdClipPath>();

        public VgRenderVx CreateRenderVx(SvgDocument svgdoc)
        {
            _svgdoc = svgdoc;

            SvgElement rootElem = svgdoc.Root;
            int childCount = rootElem.ChildCount;
            List<VgCmd> cmds = new List<VgCmd>();

            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                RenderSvgElements(rootElem.GetChild(i), cmds);
            }
            VgRenderVx renderVx = new VgRenderVx(cmds.ToArray());
            return renderVx;
        }
        void RenderSvgElements(SvgElement elem, List<VgCmd> cmds)
        {
            switch (elem.WellknowElemName)
            {
                default:
                    throw new KeyNotFoundException();
                case WellknownSvgElementName.Unknown:
                    return;
                case WellknownSvgElementName.Svg:
                    break;
                case WellknownSvgElementName.Defs:
                    _defsList.Add(elem);
                    return;
                case WellknownSvgElementName.Rect:
                    RenderRectElement(elem, cmds);
                    break;
                case WellknownSvgElementName.Image:
                    RenderImageElement(elem, cmds);
                    break;
                case WellknownSvgElementName.Polyline:
                case WellknownSvgElementName.Polygon:
                    break;
                case WellknownSvgElementName.Ellipse:
                    RenderEllipseElement(elem, cmds);
                    break;
                case WellknownSvgElementName.Circle:
                    RenderCircleElement(elem, cmds);
                    break;
                case WellknownSvgElementName.Path:
                    RenderPathElement(elem, cmds);
                    return;
                case WellknownSvgElementName.ClipPath:
                    CreateClipPath(elem, cmds);
                    return;
                case WellknownSvgElementName.Group:
                    RenderGroupElement(elem, cmds);
                    return;
            }


            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                RenderSvgElements(elem.GetChild(i), cmds);
            }
        }
        void CreateClipPath(SvgElement elem, List<VgCmd> cmds)
        {

            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                RenderSvgElements(elem.GetChild(i), cmds);
            }
        }
        bool _buildDefs = false;
        void BuildDefinitionNodes()
        {
            if (_buildDefs)
            {
                return;
            }
            _buildDefs = true;

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
                        //make this as a clip path
                        List<VgCmd> cmds = new List<VgCmd>();
                        VgCmdClipPath clipPath = new VgCmdClipPath();
                        RenderSvgElements(child, cmds);

                        clipPath._svgParts = cmds;
                        _clipPathDic.Add(child._visualSpec.Id, clipPath);
                    }
                }
            }
        }

        void AssignAttributes(SvgVisualSpec spec, List<VgCmd> cmds)
        {
            if (spec.HasFillColor)
            {
                cmds.Add(new VgCmdFillColor(spec.FillColor));
            }
            if (spec.HasStrokeColor)
            {
                cmds.Add(new VgCmdStrokeColor(spec.StrokeColor));
            }
            if (spec.HasStrokeWidth)
            {
                cmds.Add(new VgCmdStrokeWidth(spec.StrokeWidth.Number));
            }
            if (spec.Transform != null)
            {
                //convert from svg transform to 
                cmds.Add(new VgCmdAffineTransform(CreateAffine(spec.Transform)));
            }
            //
            if (spec.ClipPathLink != null)
            {
                //resolve this clip
                BuildDefinitionNodes();
                if (_clipPathDic.TryGetValue(spec.ClipPathLink.Value, out VgCmdClipPath clip))
                {
                    cmds.Add(clip);
                }
            }
        }
        void RenderPathElement(SvgElement elem, List<VgCmd> cmds)
        {
            SvgPathSpec pathSpec = elem._visualSpec as SvgPathSpec;
            //d             
            VgCmdPath pathCmd = new VgCmdPath();
            pathCmd.SetVxsAsOriginal(ParseSvgPathDefinitionToVxs(pathSpec.D.ToCharArray()));
            AssignAttributes(pathSpec, cmds);

            cmds.Add(pathCmd);
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
        void RenderEllipseElement(SvgElement elem, List<VgCmd> cmds)
        {
            SvgEllipseSpec ellipseSpec = elem._visualSpec as SvgEllipseSpec;
            VgCmdPath pathCmd = new VgCmdPath();
            VectorToolBox.GetFreeEllipseTool(out VertexProcessing.Ellipse ellipse);
            ReEvaluateArgs a = new ReEvaluateArgs(500, 500, 17); //temp fix

            double x = ConvertToPx(ellipseSpec.X, ref a);
            double y = ConvertToPx(ellipseSpec.Y, ref a);
            double rx = ConvertToPx(ellipseSpec.RadiusX, ref a);
            double ry = ConvertToPx(ellipseSpec.RadiusY, ref a);

            ellipse.Set(x, y, rx, ry);////TODO: review here => temp fix for ellipse step 
            using (VxsContext.Temp(out var v1))
            {
                pathCmd.SetVxsAsOriginal(
                    PixelFarm.CpuBlit.VertexProcessing.VertexSourceExtensions.MakeVxs(ellipse, v1).CreateTrim());
            }

            VectorToolBox.ReleaseEllipseTool(ref ellipse);

            AssignAttributes(ellipseSpec, cmds);
            cmds.Add(pathCmd);
        }
        void RenderImageElement(SvgElement elem, List<VgCmd> cmds)
        {
            SvgImageSpec imgspec = elem._visualSpec as SvgImageSpec;
            VgCmdImage imgCmd = new VgCmdImage();

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
                imgCmd.SetVxsAsOriginal(rectTool.MakeVxs(v1).CreateTrim());
            }
            VectorToolBox.ReleaseRectTool(ref rectTool);
            AssignAttributes(imgspec, cmds);
            cmds.Add(imgCmd);
        }
        void RenderCircleElement(SvgElement elem, List<VgCmd> cmds)
        {
            SvgCircleSpec ellipseSpec = elem._visualSpec as SvgCircleSpec;

            VgCmdPath pathCmd = new VgCmdPath();
            VectorToolBox.GetFreeEllipseTool(out VertexProcessing.Ellipse ellipse);
            ReEvaluateArgs a = new ReEvaluateArgs(500, 500, 17); //temp fix
            double x = ConvertToPx(ellipseSpec.X, ref a);
            double y = ConvertToPx(ellipseSpec.Y, ref a);
            double r = ConvertToPx(ellipseSpec.Radius, ref a);

            ellipse.Set(x, y, r, r);////TODO: review here => temp fix for ellipse step 
            using (VxsContext.Temp(out var v1))
            {
                pathCmd.SetVxsAsOriginal(
                    PixelFarm.CpuBlit.VertexProcessing.VertexSourceExtensions.MakeVxs(ellipse, v1).CreateTrim());
            }

            VectorToolBox.ReleaseEllipseTool(ref ellipse);

            AssignAttributes(ellipseSpec, cmds);
            cmds.Add(pathCmd);
        }
        void RenderRectElement(SvgElement elem, List<VgCmd> cmds)
        {
            SvgRectSpec rectSpec = elem._visualSpec as SvgRectSpec;
            VgCmdPath pathCmd = new VgCmdPath();

            //convert rect to path

            //pathSpec.X;
            //pathSpec.Y;
            //pathSpec.Width;
            //pathSpec.Height;

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
                    pathCmd.SetVxsAsOriginal(roundRect.MakeVxs(v1).CreateTrim());
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
                    pathCmd.SetVxsAsOriginal(rectTool.MakeVxs(v1).CreateTrim());
                }
                VectorToolBox.ReleaseRectTool(ref rectTool);
            }


            AssignAttributes(rectSpec, cmds);
            cmds.Add(pathCmd);
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

        static PixelFarm.CpuBlit.VertexProcessing.Affine CreateAffine(SvgTransform transformation)
        {
            switch (transformation.TransformKind)
            {
                default: throw new NotSupportedException();

                case SvgTransformKind.Matrix:

                    SvgTransformMatrix matrixTx = (SvgTransformMatrix)transformation;
                    float[] elems = matrixTx.Elements;
                    PixelFarm.CpuBlit.VertexProcessing.Affine affine = new VertexProcessing.Affine(
                        elems[0], elems[1],
                        elems[2], elems[3],
                        elems[4], elems[5]);
                    return affine;
                case SvgTransformKind.Rotation:
                    SvgRotate rotateTx = (SvgRotate)transformation;
                    return PixelFarm.CpuBlit.VertexProcessing.Affine.NewRotation(rotateTx.Angle);

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

        void RenderGroupElement(SvgElement elem, List<VgCmd> cmds)
        {
            var beginGroup = new VgCmdBeginGroup();
            cmds.Add(beginGroup);
            AssignAttributes(elem._visualSpec, cmds);
            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                RenderSvgElements(elem.GetChild(i), cmds);
            }

            cmds.Add(new VgCmdEndGroup());
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