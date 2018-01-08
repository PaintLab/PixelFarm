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

namespace PaintLab.Svg
{
    //very simple svg parser 

    public class SvgParser
    {
        public void ReadSvgDocument(string svgFileName)
        {
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
        CssParser parser = new CssParser();
        void ParseStyle(SvgVisualSpec spec, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {

                parser.ParseCssStyleSheet(value.ToCharArray());
                //-----------------------------------
                CssDocument cssDoc = parser.OutputCssDocument;
                CssActiveSheet cssActiveDoc = new CssActiveSheet();
                cssActiveDoc.LoadCssDoc(cssDoc);
            }
        }

        bool ParseAttribute(SvgVisualSpec spec, XmlAttribute attr)
        {

            switch (attr.Name)
            {

                default:
                    return false;
                case "id":
                    spec.Id = attr.Value;
                    return true;
                case "style":
                    ParseStyle(spec, attr.Value);
                    break;
                case "fill":
                    {
                        LayoutFarm.WebDom.CssColor color =
                            CssValueParser2.GetActualColor(attr.Value);
                    }
                    break;
                case "fill-opacity":
                    {
                    }
                    break;
                case "stroke-width":
                    break;
                case "stroke":
                    break;
                case "stroke-linecap":
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
                        ParseTransform(attr.Value);
                    }
                    break;
            }
            return true;
        }
        void ParseTransform(string value)
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
        double[] ParseMatrixArgs(string matrixTransformArgs)
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
            foreach (XmlElement child in elem.ChildNodes)
            {
                ParseSvgElement(child);
            }
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

        SvgPathDataParser _svgPatgDataParser = new SvgPathDataParser();
        void ParsePath(XmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
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
                        case "d":
                            //tokenize the path definition data
                            List<SvgPathSeg> pathSegs = _svgPatgDataParser.Parse(attr.Value.ToCharArray());
                            break;
                    }
                }
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
    }

}