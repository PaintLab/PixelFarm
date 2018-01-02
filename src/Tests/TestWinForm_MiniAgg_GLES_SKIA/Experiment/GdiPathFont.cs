////MIT, 2014-2018, WinterDev   

//using System;
//using System.Collections.Generic;

//using PixelFarm.Agg;
//namespace PixelFarm.Drawing.Fonts
//{
//    //this is experiment only***
//    class GdiPathFont
//    {
//        GdiPathFontFace fontface;
//        int emSizeInPoints;
//        float emSizeInPixels;

//        Agg.VertexSource.CurveFlattener curveFlattener = new Agg.VertexSource.CurveFlattener();
//        Dictionary<char, FontGlyph> cachedGlyphs = new Dictionary<char, FontGlyph>();
//        System.Drawing.Font gdiFont;
//        public GdiPathFont(GdiPathFontFace fontface, int emSizeInPoints)
//        {
//            this.fontface = fontface;
//            this.emSizeInPoints = emSizeInPoints;
//            //--------------------------------------
//            emSizeInPixels = RequestFont.ConvEmSizeInPointsToPixels(emSizeInPoints);
//            //--------------------------------------
//            //implementation
//            gdiFont = new System.Drawing.Font(fontface.FaceName, emSizeInPoints);
//        }
//        public string FontName
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public FontStyle FontStyle
//        {
//            get { throw new NotImplementedException(); }
//        }
//        public float GetAdvanceForCharacter(char c)
//        {
//            throw new NotImplementedException();
//        }

//        public float AscentInPixels
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public float DescentInPixels
//        {
//            get { throw new NotImplementedException(); }
//        }
//        public FontGlyph GetGlyphByIndex(uint glyphIndex)
//        {
//            throw new NotImplementedException();
//        }
//        public GdiPathFontFace FontFace
//        {
//            get { return this.fontface; }
//        }
//        public float SizeInPixels
//        {
//            get { return emSizeInPixels; }
//        }
//        public FontGlyph GetGlyph(char c)
//        {
//            FontGlyph found;
//            if (!this.cachedGlyphs.TryGetValue(c, out found))
//            {
//                //if not found then create new one
//                found = new FontGlyph();
//                //------------------------
//                //create vector version, using Path
//                VertexStore vxs = new VertexStore();
//                PixelFarm.Agg.GdiPathConverter.ConvertCharToVertexGlyph(gdiFont, c, vxs);
//                found.originalVxs = vxs;
//                //create flatten version  
//                found.flattenVxs = curveFlattener.MakeVxs(vxs, new VertexStore());//?
//                //-------------------------
//                //create bmp version 
//                //find vxs bound 
//                this.cachedGlyphs.Add(c, found);
//            }
//            return found;
//        }
//        //public void GetGlyphPos(char[] buffer, int start, int len, Ls[] properGlyphs)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        protected void OnDispose()
//        {
//        }
//        public float SizeInPoints
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }
//    }
//}