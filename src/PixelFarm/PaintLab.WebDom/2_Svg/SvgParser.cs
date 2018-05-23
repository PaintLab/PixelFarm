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
using System.Collections.Generic;

using LayoutFarm.HtmlBoxes; //temp
using LayoutFarm.Svg;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Parser;
using LayoutFarm.WebLexer;

using LayoutFarm.Svg.Pathing;


using PixelFarm;
using PixelFarm.Drawing;
using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg.Transform;


namespace PaintLab.Svg
{


    public abstract class XmlParserBase
    {
        int parseState = 0;
        TextSnapshot textSnapshot;
        MyXmlLexer myXmlLexer = new MyXmlLexer();
        string waitingAttrName;
        string currentNodeName;
        Stack<string> openEltStack = new Stack<string>();
        TextSpan attrName;
        protected struct TextSpan
        {
            public readonly int startIndex;
            public readonly int len;
            public TextSpan(int startIndex, int len)
            {
                this.startIndex = startIndex;
                this.len = len;
            }

            public static readonly TextSpan Empty = new TextSpan();
        }


        public XmlParserBase()
        {
            myXmlLexer.LexStateChanged += MyXmlLexer_LexStateChanged;
        }
        private void MyXmlLexer_LexStateChanged(XmlLexerEvent lexEvent, int startIndex, int len)
        {

            switch (lexEvent)
            {
                default:
                    {
                        throw new NotSupportedException();
                    }
                case XmlLexerEvent.VisitOpenAngle:
                    {

                    }
                    break;
                case XmlLexerEvent.CommentContent:
                    {

                    }
                    break;
                case XmlLexerEvent.FromContentPart:
                    {
                        //text content of the element 
                        OnTextNode(new TextSpan(startIndex, len));
                    }
                    break;
                case XmlLexerEvent.AttributeValueAsLiteralString:
                    {
                        //assign value and add to parent
                        //string attrValue = textSnapshot.Substring(startIndex, len);
                        if (parseState == 11)
                        {
                            //doctype node
                            //add to its parameter
                        }
                        else
                        {
                            //add value to current attribute node
                            OnAttribute(attrName, new TextSpan(startIndex, len));
                        }
                    }
                    break;
                case XmlLexerEvent.Attribute:
                    {
                        //create attribute node and wait for its value
                        attrName = new TextSpan(startIndex, len);
                        //string attrName = textSnapshot.Substring(startIndex, len);
                    }
                    break;
                case XmlLexerEvent.NodeNameOrAttribute:
                    {
                        //the lexer dose not store state of element name or attribute name
                        //so we use parseState to decide here

                        string name = textSnapshot.Substring(startIndex, len);
                        switch (parseState)
                        {
                            case 0:
                                {
                                    //element name=> create element 
                                    if (currentNodeName != null)
                                    {
                                        OnEnteringElementBody();
                                        openEltStack.Push(currentNodeName);
                                    }

                                    currentNodeName = name;
                                    //enter new node                                   
                                    OnVisitNewElement(new TextSpan(startIndex, len));

                                    parseState = 1; //enter attribute 
                                    waitingAttrName = null;
                                }
                                break;
                            case 1:
                                {
                                    //wait for attr value 
                                    if (waitingAttrName != null)
                                    {
                                        //push waiting attr
                                        //create new attribute

                                        //eg. in html
                                        //but this is not valid in Xml

                                        throw new NotSupportedException();
                                    }
                                    waitingAttrName = name;
                                }
                                break;
                            case 2:
                                {
                                    //****
                                    //node name after open slash  </
                                    //TODO: review here,avoid direct string comparison
                                    if (currentNodeName == name)
                                    {
                                        OnExitingElementBody();

                                        if (openEltStack.Count > 0)
                                        {
                                            waitingAttrName = null;
                                            currentNodeName = openEltStack.Pop();
                                        }
                                        parseState = 3;
                                    }
                                    else
                                    {
                                        //eg. in html
                                        //but this is not valid in Xml
                                        //not match open-close tag
                                        throw new NotSupportedException();
                                    }
                                }
                                break;
                            case 4:
                                {
                                    //attribute value as id ***
                                    //eg. in Html, but not for general Xml
                                    throw new NotSupportedException();
                                }

                            case 10:
                                {
                                    //eg <! 

                                    parseState = 11;
                                }
                                break;
                            case 11:
                                {
                                    //doc 

                                }
                                break;
                            default:
                                {
                                }
                                break;
                        }
                    }
                    break;
                case XmlLexerEvent.VisitCloseAngle:
                    {
                        //close angle of current new node
                        //enter into its content 
                        if (parseState == 11)
                        {
                            //add doctype to html 
                        }
                        else
                        {

                        }
                        waitingAttrName = null;
                        parseState = 0;
                    }
                    break;
                case XmlLexerEvent.VisitAttrAssign:
                    {
                        parseState = 4;
                    }
                    break;
                case XmlLexerEvent.VisitOpenSlashAngle:
                    {
                        parseState = 2;
                    }
                    break;
                case XmlLexerEvent.VisitCloseSlashAngle:
                    {
                        //   />
                        if (openEltStack.Count > 0)
                        {
                            OnExitingElementBody();
                            //curTextNode = null;
                            //curAttr = null;
                            waitingAttrName = null;
                            currentNodeName = openEltStack.Pop();
                        }
                        parseState = 0;
                    }
                    break;
                case XmlLexerEvent.VisitOpenAngleExclimation:
                    {
                        parseState = 10;
                    }
                    break;

            }
        }

        public virtual void ParseDocument(TextSnapshot textSnapshot)
        {
            OnBegin();
            //reset
            openEltStack.Clear();
            waitingAttrName = null;
            currentNodeName = null;
            parseState = 0;

            //
            this.textSnapshot = textSnapshot;
            myXmlLexer.BeginLex();
            myXmlLexer.Analyze(textSnapshot);
            myXmlLexer.EndLex();

            OnFinish();
        }

        protected virtual void OnBegin()
        {

        }
        public virtual void OnFinish()
        {

        }


        //-------------------
        protected virtual void OnTextNode(TextSpan text) { }
        protected virtual void OnAttribute(TextSpan localAttr, TextSpan value) { }
        protected virtual void OnAttribute(TextSpan ns, TextSpan localAttr, TextSpan value) { }

        protected virtual void OnVisitNewElement(TextSpan ns, TextSpan localName) { }
        protected virtual void OnVisitNewElement(TextSpan localName) { }

        protected virtual void OnEnteringElementBody() { }
        protected virtual void OnExitingElementBody() { }
    }


    //TODO: optimize and refactor here 



    public class SvgParser : XmlParserBase
    {

        List<SvgPart> _renderVxList = new List<SvgPart>();
        CurveFlattener _curveFlattener = new CurveFlattener();
        MySvgPathDataParser _svgPathDataParser = new MySvgPathDataParser();
        CssParser _cssParser = new CssParser();
        Queue<PathWriter> _resuablePathWriterQueue = new Queue<PathWriter>();
        Queue<VertexStore> _reusableVertexStore = new Queue<VertexStore>();
        Stack<ParsingContext> openElemStack = new Stack<ParsingContext>();

        ParsingContext currentContex;

        public SvgParser()
        {

        }
        protected override void OnBegin()
        {
            _renderVxList.Clear();
            openElemStack.Clear();

            currentContex = null;
            base.OnBegin();

        }

        TextSnapshot _textSnapshot;
        public void ReadSvgString(string svgString)
        {
            _textSnapshot = new TextSnapshot(svgString);
            ParseDocument(_textSnapshot);
        }
        public void ReadSvgCharBuffer(char[] svgBuffer)
        {
            _textSnapshot = new TextSnapshot(svgBuffer);
            ParseDocument(_textSnapshot);
        }
        public void ReadSvgFile(string svgFileName)
        {
            ReadSvgString(System.IO.File.ReadAllText(svgFileName));
        }


        public SvgPart[] GetResult()
        {
            return _renderVxList.ToArray();
        }
        public SvgRenderVx GetResultAsRenderVx()
        {
            return new SvgRenderVx(_renderVxList.ToArray());
        }


        abstract class ParsingContext
        {
            protected SvgVisualSpec spec;
            internal SvgParser _ownerParser;
            public ParsingContext()
            {

            }
            public virtual void VisitContext()
            {
                spec = new SvgVisualSpec();
            }
            public virtual bool AddAttribute(string name, string value)
            {
                switch (name)
                {

                    default:
                        return false;
                    case "class":
                        spec.Id = value;
                        break;
                    case "id":
                        spec.Id = value;
                        return true;
                    case "style":
                        ParseStyle(spec, value);
                        break;
                    case "fill":
                        {
                            if (value != "none")
                            {
                                spec.FillColor = ConvToActualColor(CssValueParser2.GetActualColor(value));
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
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(value);
                        }
                        break;
                    case "stroke":
                        {
                            if (value != "none")
                            {
                                spec.StrokeColor = ConvToActualColor(CssValueParser2.GetActualColor(value));
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
                            ParseTransform(value, spec);
                        }
                        break;
                }
                return true;
            }
            public virtual void EnterContent()
            {

            }
            public virtual void ExitingContent()
            {

            }

#if DEBUG
            static int s_dbugIdCount;
#endif
            void ParseStyle(SvgVisualSpec spec, string cssStyle)
            {
                if (!String.IsNullOrEmpty(cssStyle))
                {

#if DEBUG
                    s_dbugIdCount++;

#endif
                    //***                
                    CssRuleSet cssRuleSet = _ownerParser._cssParser.ParseCssPropertyDeclarationList(cssStyle.ToCharArray());

                    foreach (CssPropertyDeclaration propDecl in cssRuleSet.GetAssignmentIter())
                    {
                        switch (propDecl.UnknownRawName)
                        {

                            default:
                                break;
                            case "fill":
                                {

                                    int valueCount = propDecl.ValueCount;
                                    //1
                                    string value = propDecl.GetPropertyValue(0).ToString();
                                    if (value != "none")
                                    {
                                        spec.FillColor = ConvToActualColor(CssValueParser2.GetActualColor(value));
                                    }
                                }
                                break;
                            case "fill-opacity":
                                {
                                    //TODO:
                                    //adjust fill opacity
                                }
                                break;
                            case "stroke-width":
                                {
                                    int valueCount = propDecl.ValueCount;
                                    //1
                                    string value = propDecl.GetPropertyValue(0).ToString();

                                    spec.StrokeWidth = UserMapUtil.ParseGenericLength(value);
                                }
                                break;
                            case "stroke":
                                {
                                    //TODO:
                                    //if (attr.Value != "none")
                                    //{
                                    //    spec.StrokeColor = ConvToActualColor(CssValueParser2.GetActualColor(attr.Value));
                                    //}
                                }
                                break;
                            case "stroke-linecap":
                                //set line-cap and line join again
                                //TODO:
                                break;
                            case "stroke-linejoin":
                                //TODO:
                                break;
                            case "stroke-miterlimit":
                                //TODO:
                                break;
                            case "stroke-opacity":
                                //TODO:
                                break;
                            case "transform":
                                {
                                    ////parse trans
                                    //ParseTransform(attr.Value, spec);
                                }
                                break;
                        }
                    }
                }
            }

            static PixelFarm.Drawing.Color ConvToActualColor(CssColor color)
            {
                return new Color(color.A, color.R, color.G, color.B);
            }

            static void ParseTransform(string value, SvgVisualSpec spec)
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


            protected static void AssignValues(SvgPart svgPart, SvgVisualSpec spec)
            {

                if (spec.HasFillColor)
                {
                    svgPart.FillColor = spec.FillColor;
                }

                if (spec.HasStrokeColor)
                {
                    svgPart.StrokeColor = spec.StrokeColor;
                }

                if (spec.HasStrokeWidth)
                {
                    //assume this is in pixel unit
                    svgPart.StrokeWidth = spec.StrokeWidth.Number;
                }
                if (spec.Transform != null)
                {
                    svgPart.AffineTx = spec.Transform;
                }
            }

        }

        class SvgGroupContext : ParsingContext
        {
            SvgPart beginVx;
            public override void EnterContent()
            {
                AssignValues(beginVx, spec);
                base.EnterContent();
            }
            public override void VisitContext()
            {
                beginVx = new SvgPart(SvgRenderVxKind.BeginGroup);
                _ownerParser._renderVxList.Add(beginVx);
                base.VisitContext();
            }
            public override void ExitingContent()
            {
                base.ExitingContent();
                _ownerParser._renderVxList.Add(new SvgPart(SvgRenderVxKind.EndGroup));
            }
        }
        class SvgTitleContext : ParsingContext
        {

        }
        class SvgShapeContext : ParsingContext
        {
            SvgPart svgPart;
            public SvgShapeContext(string shapeName)
            {
                ShapeName = shapeName;
                svgPart = new SvgPart(SvgRenderVxKind.Path);
            }
            public string ShapeName { get; set; }
            public override void EnterContent()
            {
                AssignValues(svgPart, spec);
                base.EnterContent();
            }
        }

        //------------------------------------

        public VertexStore ParseSvgPathDefinitionToVxs(char[] buffer)
        {

            PathWriter pathWriter = GetFreePathWriter();
            _svgPathDataParser.SetPathWriter(pathWriter);
            //tokenize the path definition data
            _svgPathDataParser.Parse(buffer);

            //
            VertexStore flattenVxs = GetFreeVxs();
            _curveFlattener.MakeVxs(pathWriter.Vxs, flattenVxs);
            //------------------------------------------------- 

            //create a small copy of the vxs 
            VertexStore vxs = flattenVxs.CreateTrim();

            ReleaseVertexStore(flattenVxs);
            ReleasePathWriter(pathWriter);


            return vxs;
        }

        PathWriter GetFreePathWriter()
        {
            if (_resuablePathWriterQueue.Count > 0)
            {
                return _resuablePathWriterQueue.Dequeue();
            }
            else
            {
                return new PathWriter(new VertexStore());
            }
        }
        void ReleasePathWriter(PathWriter p)
        {
            p.Clear();
            _resuablePathWriterQueue.Enqueue(p);
        }
        VertexStore GetFreeVxs()
        {
            if (_reusableVertexStore.Count > 0)
            {
                return _reusableVertexStore.Dequeue();
            }
            else
            {
                return new VertexStore();
            }
        }
        void ReleaseVertexStore(VertexStore vxs)
        {
            vxs.Clear();
            _reusableVertexStore.Enqueue(vxs);
        }


        //------------------------------------




        class SvgPathContext : ParsingContext
        {
            SvgPart svgPart;
            string d_attribute;

            public SvgPathContext()
            {

            }
            public override void VisitContext()
            {
                svgPart = new SvgPart(SvgRenderVxKind.Path);
                _ownerParser._renderVxList.Add(svgPart);
                base.VisitContext();
            }
            public override bool AddAttribute(string name, string value)
            {
                if (name == "d")
                {
                    d_attribute = value;
                    return true;
                }
                return base.AddAttribute(name, value);
            }
            public override void ExitingContent()
            {
                EvaluatePathDefinition();
                base.ExitingContent();
            }
            void EvaluatePathDefinition()
            {
                if (d_attribute != null)
                {
                    //create new path 
                    AssignValues(svgPart, spec);

                    svgPart.SetVxsAsOriginal(
                        _ownerParser.ParseSvgPathDefinitionToVxs(d_attribute.ToCharArray()));


                    if (svgPart.HasStrokeWidth && svgPart.StrokeWidth > 0)
                    {
                        //TODO: implement stroke rendering 
                    }


                    d_attribute = null;
                }
            }
            public override void EnterContent()
            {

                EvaluatePathDefinition();
                base.EnterContent();
            }
        }


        class SvgContext : ParsingContext
        {

        }

        protected override void OnVisitNewElement(TextSpan ns, TextSpan localName)
        {
            throw new NotSupportedException();
        }
        protected override void OnVisitNewElement(TextSpan localName)
        {
            string elemName = _textSnapshot.Substring(localName.startIndex, localName.len);
            if (currentContex != null)
            {
                openElemStack.Push(currentContex);
            }

            switch (elemName)
            {
                default:
#if DEBUG
                    Console.WriteLine("unimplemented element: " + elemName);
#endif
                    break;
                case "svg":
                    currentContex = new SvgContext();
                    break;
                case "g":
                    currentContex = new SvgGroupContext();
                    break;
                case "title":
                    currentContex = new SvgTitleContext();
                    break;
                case "rect":
                case "line":
                case "polyline":
                case "polygon":
                    currentContex = new SvgShapeContext(elemName);
                    break;
                case "path":
                    currentContex = new SvgPathContext();
                    break;
            }
            currentContex._ownerParser = this;
            currentContex.VisitContext();
        }

        protected override void OnAttribute(TextSpan localAttr, TextSpan value)
        {
            string attrLocalName = _textSnapshot.Substring(localAttr.startIndex, localAttr.len);
            string attrValue = _textSnapshot.Substring(value.startIndex, value.len);

            currentContex.AddAttribute(attrLocalName, attrValue);
        }
        protected override void OnAttribute(TextSpan ns, TextSpan localAttr, TextSpan value)
        {
            string attrLocalName = _textSnapshot.Substring(localAttr.startIndex, localAttr.len);
            string attrValue = _textSnapshot.Substring(value.startIndex, value.len);
            currentContex.AddAttribute(attrLocalName, attrValue);
        }
        protected override void OnEnteringElementBody()
        {
            currentContex.EnterContent();
            base.OnEnteringElementBody();
        }
        protected override void OnExitingElementBody()
        {
            currentContex.ExitingContent();
            if (openElemStack.Count > 0)
            {
                currentContex = openElemStack.Pop();
            }
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