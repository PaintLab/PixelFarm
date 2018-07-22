//MIT, 2018-present, WinterDev
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



namespace PaintLab.Svg
{


    using Internal;


    public abstract class XmlParserBase
    {
        int parseState = 0;
        protected TextSnapshot _textSnapshot;
        MyXmlLexer myXmlLexer = new MyXmlLexer();
        string waitingAttrName;
        string currentNodeName;
        Stack<string> openEltStack = new Stack<string>();

        TextSpan nodeNamePrefix;
        bool hasNodeNamePrefix;

        TextSpan attrName;
        TextSpan attrPrefix;
        bool hasAttrPrefix;

        protected struct TextSpan
        {
            public readonly int startIndex;
            public readonly int len;
            public TextSpan(int startIndex, int len)
            {
                this.startIndex = startIndex;
                this.len = len;
            }
#if DEBUG
            public override string ToString()
            {
                return startIndex + "," + len;
            }
#endif
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
                        //enter new context
                    }
                    break;
                case XmlLexerEvent.CommentContent:
                    {

                    }
                    break;
                case XmlLexerEvent.NamePrefix:
                    {
                        //name prefix of 

#if DEBUG
                        string testStr = _textSnapshot.Substring(startIndex, len);
#endif

                        switch (parseState)
                        {
                            default:
                                throw new NotSupportedException();
                            case 0:
                                nodeNamePrefix = new TextSpan(startIndex, len);
                                hasNodeNamePrefix = true;
                                break;
                            case 1:
                                //attribute part
                                attrPrefix = new TextSpan(startIndex, len);
                                hasAttrPrefix = true;
                                break;
                            case 2: //   </a
                                nodeNamePrefix = new TextSpan(startIndex, len);
                                hasNodeNamePrefix = true;
                                break;
                        }
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
                            parseState = 1;
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

                        string name = _textSnapshot.Substring(startIndex, len);
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
                                    //comment node

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
            this._textSnapshot = textSnapshot;


            OnBegin();
            //reset
            openEltStack.Clear();
            waitingAttrName = null;
            currentNodeName = null;
            parseState = 0;

            //

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

    namespace Internal
    {

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
                        spec.Class = value;
                        break;
                    case "id":
                        spec.Id = value;
                        return true;
                    case "style":
                        AddStyle(spec, value);
                        break;
                    case "clip-path":
                        AddClipPathLink(spec, value);
                        break;
                    case "fill":
                        {
                            if (value != "none")
                            {
                                //spec.FillColor = ConvToActualColor(CssValueParser2.GetActualColor(value));
                                spec.FillColor = CssValueParser2.GetActualColor(value);
                            }
                        }
                        break;
                    case "fill-opacity":
                        {
                            //adjust fill opacity
                            //0f-1f?

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
                                //spec.StrokeColor = ConvToActualColor(CssValueParser2.GetActualColor(value));
                                spec.StrokeColor = CssValueParser2.GetActualColor(value);
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
            static void AddClipPathLink(SvgVisualSpec spec, string value)
            {
                //eg. url(#aaa)
                if (value.StartsWith("url("))
                {
                    int endAt = value.IndexOf(')', 4);
                    if (endAt > -1)
                    {
                        //get value 
                        string url_value = value.Substring(4, endAt - 4);
                        if (url_value.StartsWith("#"))
                        {
                            spec.ClipPath = new SvgAttributeLink(SvgAttributeLinkKind.Id, url_value.Substring(1));
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                }
                else
                {

                }

            }

#if DEBUG
            static int s_dbugIdCount;
#endif
            void AddStyle(SvgVisualSpec spec, string cssStyle)
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
                                        //spec.FillColor = ConvToActualColor(CssValueParser2.GetActualColor(value));
                                        spec.FillColor = CssValueParser2.GetActualColor(value);
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

            //static PixelFarm.Drawing.Color ConvToActualColor(CssColor color)
            //{
            //    return new Color(color.A, color.R, color.G, color.B);
            //}

            static void ParseTransform(string value, SvgVisualSpec spec)
            {
                //TODO: ....

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
                                spec.Transform = new SvgTransformMatrix(ParseMatrixArgs(right));
                            }
                            break;
                        case "translate":
                            {
                                //translate matrix
                                float[] matrixArgs = ParseMatrixArgs(right);
                                spec.Transform = new SvgTranslate(matrixArgs[0], matrixArgs[1]);
                            }
                            break;
                        case "rotate":
                            {
                                float[] matrixArgs = ParseMatrixArgs(right);
                                spec.Transform = new SvgRotate(matrixArgs[0]);
                            }
                            break;
                        case "scale":
                            {
                                float[] matrixArgs = ParseMatrixArgs(right);
                                spec.Transform = new SvgScale(matrixArgs[0], matrixArgs[1]);
                            }
                            break;
                        case "skewX":
                            {
                                float[] matrixArgs = ParseMatrixArgs(right);
                                spec.Transform = new SvgSkew(matrixArgs[0], 0);
                            }
                            break;
                        case "skewY":
                            {
                                float[] matrixArgs = ParseMatrixArgs(right);
                                spec.Transform = new SvgSkew(0, matrixArgs[1]);
                            }
                            break;
                    }
                }
                else
                {
                    //?
                }
            }

            static float[] ParseMatrixArgs(string matrixTransformArgs)
            {


                int close_paren = matrixTransformArgs.IndexOf(')');
                matrixTransformArgs = matrixTransformArgs.Substring(0, close_paren);
                string[] elem_string_args = matrixTransformArgs.Split(',');
                int j = elem_string_args.Length;
                float[] elem_values = new float[j];
                for (int i = 0; i < j; ++i)
                {
                    elem_values[i] = float.Parse(elem_string_args[i]);
                }
                return elem_values;
            }
            protected static void AssignValues(SvgNode svgPart, SvgVisualSpec spec)
            {

                //if (spec.HasFillColor)
                //{
                //    svgPart.FillColor = spec.FillColor;
                //}
                //if (spec.HasStrokeColor)
                //{
                //    svgPart.StrokeColor = spec.StrokeColor;
                //}
                //if (spec.HasStrokeWidth)
                //{
                //    //assume this is in pixel unit
                //    svgPart.StrokeWidth = spec.StrokeWidth.Number;
                //}
                //if (spec.Transform != null)
                //{
                //    svgPart.AffineTx = spec.Transform;
                //}
                //if (spec.ClipPath != null)
                //{
                //    //need to be resolved later

                //}
            }

        }
        /// <summary>
        /// g
        /// </summary>
        class SvgGroupParsingContext : ParsingContext
        {
            SvgGroup svgGroup;
            public override void VisitContext()
            {
                svgGroup = new SvgGroup();
                _ownerParser.AddSvgPart(svgGroup);
                base.VisitContext();
            }

            public override void EnterContent()
            {
                //AssignValues(beginVx, spec);
                base.EnterContent();
            }

            public override void ExitingContent()
            {
                base.ExitingContent();
                //_ownerParser.AddSvgPart(new SvgEndGroup());
            }
        }

        /// <summary>
        /// title
        /// </summary>
        class SvgTitleParsingeContext : ParsingContext
        {

        }

        //------------------------------------

        /// <summary>
        /// path
        /// </summary>
        class SvgPathParsingContext : ParsingContext
        {
            SvgPath svgPart;
            string d_attribute;

            public SvgPathParsingContext()
            {

            }
            public override void VisitContext()
            {
                svgPart = new SvgPath();
                _ownerParser.AddSvgPart(svgPart);
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
                //if (d_attribute != null)
                //{
                //    //create new path 
                //    AssignValues(svgPart, spec);

                //    svgPart.SetVxsAsOriginal(
                //        _ownerParser.ParseSvgPathDefinitionToVxs(
                //            d_attribute.ToCharArray()));


                //    if (svgPart.HasStrokeWidth && svgPart.StrokeWidth > 0)
                //    {
                //        //TODO: implement stroke rendering 
                //    }


                //    d_attribute = null;
                //}
            }
            public override void EnterContent()
            {

                EvaluatePathDefinition();
                base.EnterContent();
            }
        }


        /// <summary>
        ///   svg
        /// </summary>
        class SvgParsingContext : ParsingContext
        {

        }


        /// <summary>
        ///  defs
        /// </summary>
        class SvgDefsParsingContext : ParsingContext
        {
            internal List<SvgNode> _renderVxList = new List<SvgNode>();

            public override void VisitContext()
            {

                _ownerParser.BeginDefs();
                base.VisitContext();
            }
            public override void EnterContent()
            {
                base.EnterContent();
            }
            public override void ExitingContent()
            {
                base.ExitingContent();
                _ownerParser.EndDefs();
            }
        }
        /// <summary>
        /// clipPath
        /// </summary>
        class SvgClipPathParsingContext : ParsingContext
        {

            SvgClipPath _svgClipPath;
            public override void VisitContext()
            {
                _svgClipPath = new SvgClipPath();
                _ownerParser.AddSvgPart(_svgClipPath);

                base.VisitContext();
            }
            public override void EnterContent()
            {
                base.EnterContent();
            }
            public override void ExitingContent()
            {
                base.ExitingContent();
            }
        }


        class SvgShapeContext : ParsingContext
        {
            SvgShape svgPart;
            public SvgShapeContext(string shapeName)
            {
                ShapeName = shapeName;
                svgPart = new SvgShape(shapeName);
            }
            public string ShapeName { get; set; }
            public override void EnterContent()
            {
                AssignValues(svgPart, spec);
                base.EnterContent();
            }
        }

    }
 
    public class SvgPath : SvgNode
    {

    }
    public class SvgGroup : SvgNode
    {

    }
    public class SvgShape : SvgNode
    {
        public SvgShape(string shapeName)
        {
            ShapeName = shapeName;
        }
        public string ShapeName { get; private set; }

    }
    public class SvgDocument
    {

    }

    public abstract class SvgNode
    {

    }
    class SvgDefs : SvgNode
    {
        internal List<SvgNode> _svgParts = new List<SvgNode>();
    }

    class SvgClipPath : SvgNode
    {

    }

    class SvgClipPathReference : SvgNode
    {
        public SvgClipPathReference(string pathRef)
        {

        }

        public string ReferenceName { get; set; }
    }



    public class SvgParser : XmlParserBase
    {

        List<SvgNode> _renderVxList = new List<SvgNode>();
        internal CssParser _cssParser = new CssParser();

        //CurveFlattener _curveFlattener = new CurveFlattener();
        //MySvgPathDataParser _svgPathDataParser = new MySvgPathDataParser();



        Stack<ParsingContext> openElemStack = new Stack<ParsingContext>();
        Stack<SvgDefsParsingContext> _defParsingContextStack = new Stack<SvgDefsParsingContext>();
        List<SvgDefs> _defsList = new List<SvgDefs>();
        ParsingContext _currentContex;

        public SvgParser()
        {

        }
        protected override void OnBegin()
        {
            _renderVxList.Clear();
            openElemStack.Clear();
            _currentContex = null;
            base.OnBegin();
        }


        public void ReadSvgString(string svgString)
        {

            ParseDocument(new TextSnapshot(svgString));
        }
        public void ReadSvgCharBuffer(char[] svgBuffer)
        {

            ParseDocument(new TextSnapshot(svgBuffer));
        }
        public void ReadSvgFile(string svgFileName)
        {
            ReadSvgString(System.IO.File.ReadAllText(svgFileName));
        }
        //public SvgPart[] GetResult()
        //{
        //    return null;
        //}
        SvgClipPath FindSvgClipPathById(string id)
        {
            int j = _defsList.Count;
            for (int i = 0; i < j; ++i)
            {
                SvgDefs def = _defsList[i];

            }

            return null;
        }
      

        SvgDefsParsingContext _currentDefs = null;
        internal void BeginDefs()
        {

            SvgDefsParsingContext defParsingContext = new SvgDefsParsingContext();
            defParsingContext._ownerParser = this;

            if (_currentDefs != null)
            {
                _defParsingContextStack.Push(_currentDefs);
            }

            SvgDefs svgDef = new SvgDefs();
            ////temp !
            //svgDef._svgParts = defParsingContext._renderVxList;

            _defsList.Add(svgDef);

            _currentDefs = defParsingContext;
            openElemStack.Push(defParsingContext);
        }

        internal void EndDefs()
        {
            if (_defParsingContextStack.Count > 0)
            {
                _currentDefs = _defParsingContextStack.Pop();
            }
            else
            {
                _currentDefs = null;
            }
        }
        internal void AddSvgPart(SvgNode svgPart)
        {
            if (_currentDefs != null)
            {
                _currentDefs._renderVxList.Add(svgPart);
            }
            else
            {
                _renderVxList.Add(svgPart);
            }
        }
        protected override void OnVisitNewElement(TextSpan ns, TextSpan localName)
        {
            throw new NotSupportedException();
        }
        protected override void OnVisitNewElement(TextSpan localName)
        {
            string elemName = _textSnapshot.Substring(localName.startIndex, localName.len);
            if (_currentContex != null)
            {
                openElemStack.Push(_currentContex);
            }

            switch (elemName)
            {
                default:
#if DEBUG
                    Console.WriteLine("svg unimplemented element: " + elemName);
#endif
                    break;
                case "defs":
                    _currentContex = new SvgDefsParsingContext();
                    break;
                case "clipPath":
                    _currentContex = new SvgClipPathParsingContext();
                    break;
                case "svg":
                    _currentContex = new SvgParsingContext();
                    break;
                case "g":
                    _currentContex = new SvgGroupParsingContext();
                    break;
                case "title":
                    _currentContex = new SvgTitleParsingeContext();
                    break;
                case "rect":
                case "line":
                case "polyline":
                case "polygon":
                    _currentContex = new SvgShapeContext(elemName);
                    break;
                case "path":
                    _currentContex = new SvgPathParsingContext();
                    break;
            }
            _currentContex._ownerParser = this;
            _currentContex.VisitContext();
        }

        protected override void OnAttribute(TextSpan localAttr, TextSpan value)
        {
            string attrLocalName = _textSnapshot.Substring(localAttr.startIndex, localAttr.len);
            string attrValue = _textSnapshot.Substring(value.startIndex, value.len);

            _currentContex.AddAttribute(attrLocalName, attrValue);
        }
        protected override void OnAttribute(TextSpan ns, TextSpan localAttr, TextSpan value)
        {
            string attrLocalName = _textSnapshot.Substring(localAttr.startIndex, localAttr.len);
            string attrValue = _textSnapshot.Substring(value.startIndex, value.len);
            _currentContex.AddAttribute(attrLocalName, attrValue);
        }
        protected override void OnEnteringElementBody()
        {
            _currentContex.EnterContent();
            base.OnEnteringElementBody();
        }
        protected override void OnExitingElementBody()
        {
            _currentContex.ExitingContent();
            if (openElemStack.Count > 0)
            {
                _currentContex = openElemStack.Pop();
            }
        }
    }

    //class MySvgPathDataParser : SvgPathDataParser
    //{
    //    PathWriter _writer;
    //    public void SetPathWriter(PathWriter writer)
    //    {
    //        this._writer = writer;
    //        _writer.StartFigure();
    //    }
    //    protected override void OnArc(float r1, float r2, float xAxisRotation, int largeArcFlag, int sweepFlags, float x, float y, bool isRelative)
    //    {

    //        //TODO: implement arc again
    //        throw new NotSupportedException();
    //        //base.OnArc(r1, r2, xAxisRotation, largeArcFlag, sweepFlags, x, y, isRelative);
    //    }
    //    protected override void OnCloseFigure()
    //    {
    //        _writer.CloseFigure();

    //    }
    //    protected override void OnCurveToCubic(
    //        float x1, float y1,
    //        float x2, float y2,
    //        float x, float y, bool isRelative)
    //    {

    //        if (isRelative)
    //        {
    //            _writer.Curve4Rel(x1, y1, x2, y2, x, y);
    //        }
    //        else
    //        {
    //            _writer.Curve4(x1, y1, x2, y2, x, y);
    //        }
    //    }
    //    protected override void OnCurveToCubicSmooth(float x2, float y2, float x, float y, bool isRelative)
    //    {
    //        if (isRelative)
    //        {
    //            _writer.SmoothCurve4Rel(x2, y2, x, y);
    //        }
    //        else
    //        {
    //            _writer.SmoothCurve4(x2, y2, x, y);
    //        }

    //    }
    //    protected override void OnCurveToQuadratic(float x1, float y1, float x, float y, bool isRelative)
    //    {
    //        if (isRelative)
    //        {
    //            _writer.Curve3Rel(x1, y1, x, y);
    //        }
    //        else
    //        {
    //            _writer.Curve3(x1, y1, x, y);
    //        }
    //    }
    //    protected override void OnCurveToQuadraticSmooth(float x, float y, bool isRelative)
    //    {
    //        if (isRelative)
    //        {
    //            _writer.SmoothCurve3Rel(x, y);
    //        }
    //        else
    //        {
    //            _writer.SmoothCurve3(x, y);
    //        }

    //    }
    //    protected override void OnHLineTo(float x, bool relative)
    //    {
    //        if (relative)
    //        {
    //            _writer.HorizontalLineToRel(x);
    //        }
    //        else
    //        {
    //            _writer.HorizontalLineTo(x);
    //        }
    //    }

    //    protected override void OnLineTo(float x, float y, bool relative)
    //    {
    //        if (relative)
    //        {
    //            _writer.LineToRel(x, y);
    //        }
    //        else
    //        {
    //            _writer.LineTo(x, y);
    //        }
    //    }
    //    protected override void OnMoveTo(float x, float y, bool relative)
    //    {

    //        if (relative)
    //        {
    //            _writer.MoveToRel(x, y);
    //        }
    //        else
    //        {


    //            _writer.MoveTo(x, y);
    //        }
    //    }
    //    protected override void OnVLineTo(float y, bool relative)
    //    {
    //        if (relative)
    //        {
    //            _writer.VerticalLineToRel(y);
    //        }
    //        else
    //        {
    //            _writer.VerticalLineTo(y);
    //        }
    //    }
    //}
}