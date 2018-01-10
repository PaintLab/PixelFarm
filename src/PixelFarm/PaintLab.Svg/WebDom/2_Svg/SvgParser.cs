//MIT, 2018, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.3
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// SVG parser.
//
//----------------------------------------------------------------------------
using System;
using System.Xml;
using System.Collections.Generic;

using LayoutFarm.HtmlBoxes; //temp
using LayoutFarm.Svg;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Parser;


using LayoutFarm.Svg.Pathing;


using PixelFarm;
using PixelFarm.Drawing;
using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;
using LayoutFarm.Svg.Transforms;
using PixelFarm.Agg.Transform;


namespace PaintLab.Svg
{
    //very simple svg parser 


    public enum SvgRenderVxKind
    {
        BeginGroup,
        EndGroup,
        Path
    }



    public class SvgRenderVx : RenderVx
    {

        struct TempRenderState
        {
            public float strokeWidth;
            public Color strokeColor;
            public Color fillColor;
            public PixelFarm.Agg.Transform.Affine affineTx;
        } 
        VertexStore tempVxs = new VertexStore();
        Stack<TempRenderState> _renderStateContext = new Stack<TempRenderState>();

        SvgVx[] vxList;
        internal SvgRenderVx(SvgVx[] svgVxList)
        {
            this.vxList = svgVxList;
        }
        public void Render(Painter p)
        {
            //

            int j = vxList.Length;
            PixelFarm.Agg.Transform.Affine currentTx = null;
            TempRenderState renderState = new TempRenderState();
            renderState.strokeColor = p.StrokeColor;
            renderState.strokeWidth = (float)p.StrokeWidth;
            renderState.fillColor = p.FillColor;
            renderState.affineTx = currentTx;

            //

            for (int i = 0; i < j; ++i)
            {
                SvgVx vx = vxList[i];
                switch (vx.Kind)
                {
                    case SvgRenderVxKind.BeginGroup:
                        {
                            //1. save current state before enter new state
                            _renderStateContext.Push(renderState);
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
                                    currentTx = currentTx * vx.AffineTx;
                                }
                                else
                                {
                                    currentTx = vx.AffineTx;
                                }
                                renderState.affineTx = currentTx;
                            }
                        }
                        break;
                    case SvgRenderVxKind.EndGroup:
                        {
                            //restore to prev state
                            renderState = _renderStateContext.Pop();
                            p.FillColor = renderState.fillColor;
                            p.StrokeColor = renderState.strokeColor;
                            p.StrokeWidth = renderState.strokeWidth;
                            currentTx = renderState.affineTx;
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
                                        tempVxs.Clear();
                                        currentTx.TransformToVxs(vxs, tempVxs);
                                        p.Fill(tempVxs, vx.FillColor);
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
                                        tempVxs.Clear();
                                        currentTx.TransformToVxs(vxs, tempVxs);
                                        p.Fill(tempVxs);
                                    }

                                }
                            }

                            if (p.StrokeWidth > 0)
                            {
                                //check if we have a stroke version of this render vx
                                //if not then request a new one 
                                VertexStore strokeVxs = vx.GetStrokeVxsOrCreateNew(p.StrokeWidth);
                                if (vx.HasStrokeColor)
                                {
                                    //has speciic stroke color 
                                    p.StrokeWidth = vx.StrokeWidth;
                                    if (currentTx == null)
                                    {
                                        p.Fill(strokeVxs, vx.StrokeColor);
                                    }
                                    else
                                    {
                                        //have some tx
                                        tempVxs.Clear();
                                        currentTx.TransformToVxs(strokeVxs, tempVxs);
                                        p.Fill(tempVxs, vx.StrokeColor);
                                    }

                                }
                                else if (p.StrokeColor.A > 0)
                                {
                                    if (currentTx == null)
                                    {
                                        p.Fill(strokeVxs, p.StrokeColor);
                                    }
                                    else
                                    {
                                        tempVxs.Clear();
                                        currentTx.TransformToVxs(strokeVxs, tempVxs);
                                        p.Fill(tempVxs, p.StrokeColor);
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {

                                if (vx.HasStrokeColor)
                                {
                                    VertexStore strokeVxs = vx.GetStrokeVxsOrCreateNew(p.StrokeWidth);
                                    p.Fill(strokeVxs);
                                }
                                else if (p.StrokeColor.A > 0)
                                {
                                    VertexStore strokeVxs = vx.GetStrokeVxsOrCreateNew(p.StrokeWidth);
                                    p.Fill(strokeVxs, p.StrokeColor);
                                }
                            }
                        }
                        break;
                }
            }


        }
    }


    public class SvgVx
    {
        VertexStore _vxs;
        Color _fillColor;
        Color _strokeColor;
        float _strokeWidth;
        public SvgVx(SvgRenderVxKind kind)
        {
            this.Kind = kind;
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
        public void SetVxs(VertexStore vxs)
        {
            this._vxs = vxs;
        }
        public VertexStore GetVxs()
        {
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
        public SvgRenderVxKind Kind
        {
            get;
            private set;
        }
        public Affine AffineTx { get; set; }
        VertexStore _strokeVxs;
        double _strokeVxsStrokeWidth;
        public VertexStore GetStrokeVxsOrCreateNew(double strokeWidth)
        {
            if (_strokeVxs != null && _strokeWidth == strokeWidth)
            {
                //use the cache
                return _strokeVxs;
            }

            //if not create a new one,
            //review here again
            Stroke aggStrokeGen = new Stroke(_strokeVxsStrokeWidth = strokeWidth);
            _strokeVxs = new VertexStore();
            aggStrokeGen.MakeVxs(_vxs, _strokeVxs);
            return _strokeVxs;
        }

    }

    public class SvgParser
    {

        List<SvgVx> renderVxList = new List<SvgVx>();

        public void ReadSvgString(string svgString)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(svgString);
            XmlElement docElem = xmldoc.DocumentElement;
            //then parse 
            if (docElem.Name == "svg")
            {
                //parse its content

                foreach (XmlElement elem in docElem.ChildNodes)
                {
                    ParseSvgElement(elem);
                }
            }
        }
        public void ReadSvgFile(string svgFileName)
        {
            renderVxList.Clear();
            //create simple svg dom
            //iterate all child
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(svgFileName);
            //
            XmlElement docElem = xmldoc.DocumentElement;
            //then parse 
            if (docElem.Name == "svg")
            {
                //parse its content

                foreach (XmlElement elem in docElem.ChildNodes)
                {
                    ParseSvgElement(elem);
                }
            }
        }

        public SvgVx[] GetResult()
        {
            return renderVxList.ToArray();
        }
        public SvgRenderVx GetResultAsRenderVx()
        {
            return new SvgRenderVx(renderVxList.ToArray());
        }

        void ParseSvgElement(XmlElement elem)
        {
            switch (elem.Name)
            {
                default:
                    throw new NotSupportedException();
                case "g":
                    ParseGroup(elem);
                    break;
                case "title":
                    ParseTitle(elem);
                    break;
                case "rect":
                    ParseRect(elem);
                    break;
                case "path":
                    ParsePath(elem);
                    break;
                case "line":
                    ParseLine(elem);
                    break;
                case "polyline":
                    ParsePolyline(elem);
                    break;
                case "polygon":
                    ParsePolygon(elem);
                    break;
            }
        }

        CssParser _cssParser = new CssParser();
        void ParseStyle(SvgVisualSpec spec, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {

                _cssParser.ParseCssStyleSheet(value.ToCharArray());
                //-----------------------------------
                CssDocument cssDoc = _cssParser.OutputCssDocument;
                CssActiveSheet cssActiveDoc = new CssActiveSheet();
                cssActiveDoc.LoadCssDoc(cssDoc);
            }
        }


        static PixelFarm.Drawing.Color ConvToActualColor(CssColor color)
        {
            return new Color(color.A, color.R, color.G, color.B);
        }
        bool ParseAttribute(SvgVisualSpec spec, XmlAttribute attr)
        {
            switch (attr.Name)
            {

                default:
                    return false;
                case "class":
                    spec.Id = attr.Value;
                    break;
                case "id":
                    spec.Id = attr.Value;
                    return true;
                case "style":
                    ParseStyle(spec, attr.Value);
                    break;
                case "fill":
                    {
                        if (attr.Value != "none")
                        {
                            spec.FillColor = ConvToActualColor(CssValueParser2.GetActualColor(attr.Value));
                        }
                    }
                    break;
                case "fill-opacity":
                    {
                        //adjust fill opacity
                    }
                    break;
                case "stroke-width":
                    {
                        spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                    }
                    break;
                case "stroke":
                    {
                        if (attr.Value != "none")
                        {
                            spec.StrokeColor = ConvToActualColor(CssValueParser2.GetActualColor(attr.Value));
                        }
                    }
                    break;
                case "stroke-linecap":
                    //set line-cap and line join again

                    break;
                case "stroke-linejoin":

                    break;
                case "stroke-miterlimit":

                    break;
                case "stroke-opacity":

                    break;
                case "transform":
                    {
                        //parse trans
                        ParseTransform(attr.Value, spec);
                    }
                    break;
            }
            return true;
        }
        void ParseTransform(string value, SvgVisualSpec spec)
        {
            int openParPos = value.IndexOf('(');
            if (openParPos > -1)
            {
                string right = value.Substring(openParPos + 1, value.Length - (openParPos + 1)).Trim();
                string left = value.Substring(0, openParPos);
                switch (left)
                {
                    default:
                        break;
                    case "matrix":
                        {
                            //read matrix args
                            double[] matrixArgs = ParseMatrixArgs(right);
                            //create affine matrix 
                            spec.Transform = Affine.NewCustomMatrix(
                                matrixArgs[0], matrixArgs[1],
                                matrixArgs[2], matrixArgs[3],
                                matrixArgs[4], matrixArgs[5]
                                );
                        }
                        break;
                    case "translate":
                        {
                            double[] matrixArgs = ParseMatrixArgs(right);
                        }
                        break;
                    case "rotate":
                        {
                            double[] matrixArgs = ParseMatrixArgs(right);
                        }
                        break;
                    case "scale":
                        {
                            double[] matrixArgs = ParseMatrixArgs(right);
                        }
                        break;
                    case "skewX":
                        {
                            double[] matrixArgs = ParseMatrixArgs(right);
                        }
                        break;
                    case "skewY":
                        {
                            double[] matrixArgs = ParseMatrixArgs(right);
                        }
                        break;
                }
            }
            else
            {
                //?
            }
        }

        static double[] ParseMatrixArgs(string matrixTransformArgs)
        {
            int close_paren = matrixTransformArgs.IndexOf(')');
            matrixTransformArgs = matrixTransformArgs.Substring(0, close_paren);
            string[] elem_string_args = matrixTransformArgs.Split(',');
            int j = elem_string_args.Length;
            double[] elem_values = new double[j];
            for (int i = 0; i < j; ++i)
            {
                elem_values[i] = double.Parse(elem_string_args[i]);
            }
            return elem_values;
        }
        void ParseGroup(XmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
            //read group property
            foreach (XmlAttribute attr in elem.Attributes)
            {
                //translate each attr
                if (!ParseAttribute(spec, attr))
                {
                    switch (attr.Name)
                    {
                        default:
                            break;
                    }

                }
            }

            SvgGroupElement group = new SvgGroupElement(spec, null);

            //--------
            SvgVx beginVx = new SvgVx(SvgRenderVxKind.BeginGroup);
            AssignValues(beginVx, spec);
            renderVxList.Add(beginVx);
            foreach (XmlElement child in elem.ChildNodes)
            {
                ParseSvgElement(child);
            }
            //--------
            renderVxList.Add(new SvgVx(SvgRenderVxKind.EndGroup));

        }
        void ParseTitle(XmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
            foreach (XmlAttribute attr in elem.Attributes)
            {
                //translate each attr 
                if (!ParseAttribute(spec, attr))
                {

                }
            }
            foreach (XmlElement child in elem.ChildNodes)
            {
                ParseSvgElement(child);
            }
        }
        void ParseRect(XmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
            foreach (XmlAttribute attr in elem.Attributes)
            {
                //translate each attr 
                if (!ParseAttribute(spec, attr))
                {
                    switch (attr.Name)
                    {

                    }
                }
            }
            foreach (XmlElement child in elem.ChildNodes)
            {
                ParseSvgElement(child);
            }
        }

        MySvgPathDataParser _svgPatgDataParser = new MySvgPathDataParser();
        static void AssignValues(SvgVx svgRenderVx, SvgVisualSpec spec)
        {

            if (spec.HasFillColor)
            {
                svgRenderVx.FillColor = spec.FillColor;
            }

            if (spec.HasStrokeColor)
            {
                svgRenderVx.StrokeColor = spec.StrokeColor;
            }

            if (spec.HasStrokeWidth)
            {
                //assume this is in pixel unit
                svgRenderVx.StrokeWidth = spec.StrokeWidth.Number;
            }
            if (spec.Transform != null)
            {
                svgRenderVx.AffineTx = spec.Transform;
            }
        }

        CurveFlattener curveFlattener = new CurveFlattener();

        void ParsePath(XmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
            XmlAttribute pathDefAttr = null;


            foreach (XmlAttribute attr in elem.Attributes)
            {
                //translate each attr 
                if (!ParseAttribute(spec, attr))
                {
                    // The <path> tag can consist of the path itself ("d=") 
                    // as well as of other parameters like "style=", "transform=", etc.
                    // In the last case we simply rely on the function of parsing 
                    // attributes (see 'else' branch).
                    switch (attr.Name)
                    {
                        default:

                            break;
                        case "d":
                            //process later ..
                            pathDefAttr = attr;
                            break;
                    }
                }
            }
            //--------------
            //translate and evaluate values 
            if (pathDefAttr != null)
            {

                SvgVx svgRenderVx = new SvgVx(SvgRenderVxKind.Path);
                AssignValues(svgRenderVx, spec);



                VertexStore vxs = new VertexStore();
                PathWriter pathWriter = new PathWriter(vxs);
                _svgPatgDataParser.SetPathWriter(pathWriter);
                //tokenize the path definition data
                _svgPatgDataParser.Parse(pathDefAttr.Value.ToCharArray());

                //
                VertexStore flattenVxs = new VertexStore();
                curveFlattener.MakeVxs(vxs, flattenVxs);


                if (svgRenderVx.HasStrokeWidth && svgRenderVx.StrokeWidth > 0)
                {
                    //generate stroke for this too

                }


                svgRenderVx.SetVxs(flattenVxs);
                this.renderVxList.Add(svgRenderVx);
            }


            foreach (XmlElement child in elem.ChildNodes)
            {
                ParseSvgElement(child);
            }
        }
        void ParseLine(XmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
            foreach (XmlAttribute attr in elem.Attributes)
            {
                //translate each attr 
                if (!ParseAttribute(spec, attr))
                {

                }
            }
            foreach (XmlElement child in elem.ChildNodes)
            {
                ParseSvgElement(child);
            }
        }
        void ParsePolyline(XmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
            foreach (XmlAttribute attr in elem.Attributes)
            {
                //translate each attr 
                if (!ParseAttribute(spec, attr))
                {

                }
            }
            foreach (XmlElement child in elem.ChildNodes)
            {
                ParseSvgElement(child);
            }
        }
        void ParsePolygon(XmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
            foreach (XmlAttribute attr in elem.Attributes)
            {
                //translate each attr 
                if (!ParseAttribute(spec, attr))
                {

                }
            }
            foreach (XmlElement child in elem.ChildNodes)
            {
                ParseSvgElement(child);
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

}