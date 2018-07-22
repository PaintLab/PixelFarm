//----------------------------------------------------------------------------
//MIT, 2014-present, WinterDev


using System.Collections.Generic;
using PaintLab.Svg;
using LayoutFarm.Svg;

namespace PixelFarm.CpuBlit
{
    public class SvgRenderVxDocBuilder
    {
        SvgDocument _svgdoc;
        List<SvgElement> _defsList = new List<SvgElement>();


        public SvgRenderVx CreateRenderVx(SvgDocument svgdoc)
        {
            _svgdoc = svgdoc;

            int childCount = svgdoc.Root.ChildCount;
            List<SvgPart> parts = new List<SvgPart>();

            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                RenderSvgElements(svgdoc.Root.GetChild(i), parts);
            }
            SvgRenderVx renderVx = new SvgRenderVx(parts.ToArray());
            return renderVx;
        }
        void RenderSvgElements(SvgElement elem, List<SvgPart> parts)
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
                case WellknownSvgElementName.Polyline:
                case WellknownSvgElementName.Polygon:
                case WellknownSvgElementName.Ellipse:
                    break;
                case WellknownSvgElementName.Path:
                    RenderPathElement(elem, parts);
                    return;
                case WellknownSvgElementName.ClipPath:
                    break;
                case WellknownSvgElementName.Group:
                    RenderGroupElement(elem, parts);
                    return;
            }


            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                RenderSvgElements(elem.GetChild(i), parts);
            }
        }

        void AssignAttributes(SvgPart part)
        {

        }
        void RenderPathElement(SvgElement elem, List<SvgPart> parts)
        {
            SvgPathSpec pathSpec = elem._visualSpec as SvgPathSpec;
            //d

        }
        void RenderGroupElement(SvgElement elem, List<SvgPart> parts)
        {
            var beginGroup = new SvgBeginGroup();

            parts.Add(beginGroup);
            //
            int childCount = elem.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                //translate SvgElement to  
                //command stream?
                RenderSvgElements(elem.GetChild(i), parts);
            }

            parts.Add(new SvgEndGroup());
        }
    }
    public static class SvgRenderVxDocBuilderExt
    {
        public static SvgRenderVx CreateRenderVx(this SvgDocument svgdoc)
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

    //public static class SvgParserExt
    //{
    //    //public static SvgRenderVx GetResultAsRenderVx(this PaintLab.Svg.SvgParser parser)
    //    //{

    //    //    ////resolve all elements...
    //    //    //int j = _renderVxList.Count;

    //    //    ////TODO: review here
    //    //    ////temp fix 
    //    //    //for (int i = 0; i < j; ++i)
    //    //    //{
    //    //    //    SvgPart svgpart = _renderVxList[i];
    //    //    //    if (svgpart.SvgClipPath != null)
    //    //    //    {
    //    //    //        //var clipPathRef = svgpart.SvgClipPath as SvgClipPathReference;
    //    //    //        //if (clipPathRef.ResolvedClip == null)
    //    //    //        //{
    //    //    //        //    //resolve this clip
    //    //    //        //    SvgClipPath clipPath = FindSvgClipPathById(clipPathRef.RefName);
    //    //    //        //    if (clipPath != null)
    //    //    //        //    {
    //    //    //        //        SvgClipPath foundClip = FindSvgClipPathById(clipPathRef.RefName);
    //    //    //        //    }
    //    //    //        //    else
    //    //    //        //    {

    //    //    //        //    }
    //    //    //        //}
    //    //    //    }
    //    //    //}

    //    //    //var result = new SvgRenderVx(_renderVxList.ToArray());
    //    //    ////result.SetSvgDefs(_defsList.ToArray());
    //    //    //return result;
    //    //    return null;
    //    //}


    //    ////   ------------------------------------

    //    //public static VertexStore ParseSvgPathDefinitionToVxs(char[] buffer)
    //    //{

    //    //    using (VxsContext.Temp(out var flattenVxs))
    //    //    {
    //    //        VectorToolBox.GetFreePathWriter(out PathWriter pathWriter);
    //    //        _svgPathDataParser.SetPathWriter(pathWriter);
    //    //        _svgPathDataParser.Parse(buffer);

    //    //        _curveFlattener.MakeVxs(pathWriter.Vxs, flattenVxs);

    //    //        //create a small copy of the vxs 
    //    //        VectorToolBox.ReleasePathWriter(ref pathWriter);

    //    //        return flattenVxs.CreateTrim();
    //    //    }
    //    //}

    //}

}