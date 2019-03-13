////MIT, 2016-present, WinterDev
//using System;
////
//using PixelFarm.CpuBlit;
//using PixelFarm.Drawing;
//using PixelFarm.Drawing.Fonts;

//namespace PixelFarm.DrawingGL
//{
//    public class CpuBlitTextSpanPrinter : ITextPrinter
//    {
//        MemBitmap _memBmp;
//        AggPainter _aggPainter;
//        VxsTextPrinter _vxsTextPrinter;
//        int _bmpWidth;
//        int _bmpHeight;
//        GLPainterContext _pcx;
//        GLPainter _canvasPainter;
//        LayoutFarm.OpenFontTextService _openFontTextServices;

//        public CpuBlitTextSpanPrinter(GLPainter canvasPainter, int w, int h)
//        {
//            //this class print long text into agg canvas
//            //then copy pixel buffer from agg canvas to gl-bmp
//            //then draw the  gl-bmp into target gl canvas


//            //TODO: review here
//            _canvasPainter = canvasPainter;
//            _pcx = canvasPainter.PainterContext;
//            _bmpWidth = w;
//            _bmpHeight = h;

//            _memBmp = new MemBitmap(_bmpWidth, _bmpHeight);
//#if DEBUG
//            _memBmp._dbugNote = "CpuBlitTextSpanPrinter.ctor";
//#endif
//            _aggPainter = AggPainter.Create(_memBmp);
//            _aggPainter.FillColor = Color.Black;
//            _aggPainter.StrokeColor = Color.Black;

//            //set default1
//            _aggPainter.CurrentFont = canvasPainter.CurrentFont;
//            _openFontTextServices = new LayoutFarm.OpenFontTextService();
//            _vxsTextPrinter = new VxsTextPrinter(_aggPainter, _openFontTextServices);
//            _aggPainter.TextPrinter = _vxsTextPrinter;
//        }
//        public bool StartDrawOnLeftTop { get; set; }
//        public Typography.Contours.HintTechnique HintTechnique
//        {
//            get => _vxsTextPrinter.HintTechnique;
//            set => _vxsTextPrinter.HintTechnique = value;
//        }
//        public bool UseSubPixelRendering
//        {
//            get => _aggPainter.UseSubPixelLcdEffect;
//            set => _aggPainter.UseSubPixelLcdEffect = value;
//        }
//        public void ChangeFont(RequestFont font)
//        {
//            _aggPainter.CurrentFont = font;
//        }
//        public void ChangeFillColor(Color fillColor)
//        {
//            //we use agg canvas to draw a font glyph
//            //so we must set fill color for this
//            _aggPainter.FillColor = fillColor;
//        }
//        public void ChangeStrokeColor(Color strokeColor)
//        {
//            //we use agg canvas to draw a font glyph
//            //so we must set fill color for this
//            _aggPainter.StrokeColor = strokeColor;
//        }
//        public void MeasureString(char[] buffer, int startAt, int len, out int w, out int h)
//        {
//            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);
//            Size size = _openFontTextServices.MeasureString(ref textBufferSpan, _aggPainter.CurrentFont);
//            w = size.Width;
//            h = size.Height;
//        }

//        public void DrawString(char[] text, int startAt, int len, double x, double y)
//        {
//            if (this.UseSubPixelRendering)
//            {
//                //1. clear prev drawing result
//                _aggPainter.Clear(Drawing.Color.Empty);
//                //aggPainter.Clear(Drawing.Color.White);
//                //aggPainter.Clear(Drawing.Color.FromArgb(0, 0, 0, 0));
//                //2. print text span into Agg Canvas
//                _vxsTextPrinter.DrawString(text, startAt, len, 0, 0);
//                //3.copy to gl bitmap
//                //byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
//                //------------------------------------------------------
//                //TODO: review here, use reusable-bitmap instead of current new one everytime.
//                GLBitmap glBmp = new GLBitmap(new PixelFarm.Drawing.MemBitmapBinder(_memBmp, false));
//                glBmp.IsYFlipped = false;
//                //TODO: review font height
//                if (StartDrawOnLeftTop)
//                {
//                    y -= _vxsTextPrinter.FontLineSpacingPx;
//                }
//                _pcx.DrawGlyphImageWithSubPixelRenderingTechnique(glBmp, (float)x, (float)y);
//                glBmp.Dispose();
//            }
//            else
//            {

//                //1. clear prev drawing result
//                _aggPainter.Clear(Drawing.Color.White);
//                _aggPainter.StrokeColor = Color.Black;

//                //2. print text span into Agg Canvas
//                _vxsTextPrinter.StartDrawOnLeftTop = false;

//                float dyOffset = _vxsTextPrinter.FontDescedingPx;
//                _vxsTextPrinter.DrawString(text, startAt, len, 0, -dyOffset);
//                //------------------------------------------------------
//                //debug save image from agg's buffer
//#if DEBUG
//                //actualImage.dbugSaveToPngFile("d:\\WImageTest\\aa1.png");
//#endif
//                //------------------------------------------------------

//                //3.copy to gl bitmap
//                //byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
//                //------------------------------------------------------
//                //debug save image from agg's buffer 

//                //------------------------------------------------------
//                //GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, true);

//                //TODO: review here again ***
//                //use cache buffer instead of creating the buffer every time

//                GLBitmap glBmp = new GLBitmap(new PixelFarm.Drawing.MemBitmapBinder(_memBmp, false));
//                glBmp.IsYFlipped = false;
//                //TODO: review font height 
//                //if (StartDrawOnLeftTop)
//                //{
//                y += _vxsTextPrinter.FontLineSpacingPx;
//                //}
//                _pcx.DrawGlyphImage(glBmp, (float)x, (float)y + dyOffset);
//                glBmp.Dispose();
//            }
//        }
//        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] text, int start, int len)
//        {
//            GLRenderVxFormattedString glRenderVx = (GLRenderVxFormattedString)renderVx;
//            glRenderVx.buff = text;
//            glRenderVx.start = start;
//            glRenderVx.len = len;


//        }
//        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
//        {
//            //1. clear prev drawing result
//            //float x = 0;
//            //float y = 0;

//            GLRenderVxFormattedString glRenderVx = (GLRenderVxFormattedString)renderVx;
//            _aggPainter.Clear(Drawing.Color.White);
//            _aggPainter.FillColor = Color.Black;
//            _vxsTextPrinter.DrawString(glRenderVx.buff, glRenderVx.start, glRenderVx.len, 0, 0);
//            MeasureString(glRenderVx.buff, glRenderVx.start, glRenderVx.len, out int width, out int height);
//            //3.copy to gl bitmap
//            //byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
//            //------------------------------------------------------
//            //TODO: review here, use reusable-bitmap instead of current new one everytime.
//            GLBitmap glBmp = new GLBitmap(new PixelFarm.Drawing.MemBitmapBinder(_memBmp, false));
//            glBmp.IsYFlipped = false;
//            //TODO: review font height
//            if (StartDrawOnLeftTop)
//            {
//                y -= _vxsTextPrinter.FontLineSpacingPx;
//            }
//            _pcx.DrawSubImage(glBmp, 0, 0, width, height, (float)x, (float)y);

//            glBmp.Dispose();
//        }
//    }


//    public class CpuBlitTextSpanPrinter2 : ITextPrinter
//    {
//        MemBitmap _memBmp;
//        AggPainter _aggPainter;
//        FontAtlasTextPrinter _fontAtlasTextPrinter;
//        int _bmpWidth;
//        int _bmpHeight;
//        GLPainterContext _pcx;
//        GLPainter _canvasPainter;
//        LayoutFarm.OpenFontTextService _openFontTextServices;

//        public CpuBlitTextSpanPrinter2(GLPainter canvasPainter, int w, int h, LayoutFarm.OpenFontTextService textServices)
//        {
//            //this class print long text into agg canvas
//            //then copy pixel buffer from agg canvas to gl-bmp
//            //then draw the  gl-bmp into target gl canvas

//            StartDrawOnLeftTop = true;
//            //TODO: review here
//            _canvasPainter = canvasPainter;
//            _pcx = canvasPainter.PainterContext;
//            _bmpWidth = w;
//            _bmpHeight = h;

//            _memBmp = new MemBitmap(_bmpWidth, _bmpHeight);
//#if DEBUG
//            _memBmp._dbugNote = "CpuBlitTextSpanPrinter2.ctor";
//#endif
//            _aggPainter = AggPainter.Create(_memBmp);
//            _aggPainter.FillColor = Color.Black;
//            _aggPainter.StrokeColor = Color.Black;

//            //set default1
//            _aggPainter.CurrentFont = canvasPainter.CurrentFont;
//            //_openFontTextServices = new LayoutFarm.OpenFontTextService();
//            _openFontTextServices = textServices;
//            _fontAtlasTextPrinter = new FontAtlasTextPrinter(_aggPainter);
//            _aggPainter.TextPrinter = _fontAtlasTextPrinter;

//            this.HintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction_VerticalOnly;
//            this.UseSubPixelRendering = true;
//        }
//        public bool StartDrawOnLeftTop { get; set; }
//        public Typography.Contours.HintTechnique HintTechnique
//        {
//            get => Typography.Contours.HintTechnique.None;
//            set { }
//        }
//        public bool UseSubPixelRendering
//        {
//            get => _aggPainter.UseSubPixelLcdEffect;
//            set => _aggPainter.UseSubPixelLcdEffect = value;
//        }
//        public void ChangeFont(RequestFont font)
//        {
//            _aggPainter.CurrentFont = font;
//        }
//        public void ChangeFillColor(Color fillColor)
//        {
//            //we use agg canvas to draw a font glyph
//            //so we must set fill color for this
//            _aggPainter.FillColor = fillColor;
//        }
//        public void ChangeStrokeColor(Color strokeColor)
//        {
//            //we use agg canvas to draw a font glyph
//            //so we must set fill color for this
//            _aggPainter.StrokeColor = strokeColor;
//        }
//        public void MeasureString(char[] buffer, int startAt, int len, out int w, out int h)
//        {
//            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);
//            Size size = _openFontTextServices.MeasureString(ref textBufferSpan, _aggPainter.CurrentFont);
//            w = size.Width;
//            h = size.Height;
//        }

//        public void DrawString(char[] text, int startAt, int len, double x, double y)
//        {
//            if (this.UseSubPixelRendering)
//            {
//                //1. clear prev drawing result
//                _aggPainter.Clear(Drawing.Color.Empty);
//                //aggPainter.Clear(Drawing.Color.White);
//                //aggPainter.Clear(Drawing.Color.FromArgb(0, 0, 0, 0));
//                //2. print text span into Agg Canvas
//                //_vxsTextPrinter.DrawString(text, startAt, len, 0, 0);
//                _fontAtlasTextPrinter.DrawString(text, startAt, len, 0, 0);
//                //3.copy to gl bitmap
//                //byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
//                //------------------------------------------------------
//                //TODO: review here, use reusable-bitmap instead of current new one everytime.
//                GLBitmap glBmp = new GLBitmap(new PixelFarm.Drawing.MemBitmapBinder(_memBmp, false));
//                glBmp.IsYFlipped = false;
//                //TODO: review font height
//                if (StartDrawOnLeftTop)
//                {
//                    y -= _fontAtlasTextPrinter.FontLineSpacingPx;
//                }
//                _pcx.DrawGlyphImageWithSubPixelRenderingTechnique(glBmp, (float)x, (float)y);
//                glBmp.Dispose();
//            }
//            else
//            {

//                //1. clear prev drawing result
//                _aggPainter.Clear(Drawing.Color.White);
//                _aggPainter.StrokeColor = Color.Black;

//                //2. print text span into Agg Canvas
//                _fontAtlasTextPrinter.StartDrawOnLeftTop = false;

//                float dyOffset = _fontAtlasTextPrinter.FontDescedingPx;
//                _fontAtlasTextPrinter.DrawString(text, startAt, len, 0, -dyOffset);
//                //------------------------------------------------------
//                //debug save image from agg's buffer
//#if DEBUG
//                //actualImage.dbugSaveToPngFile("d:\\WImageTest\\aa1.png");
//#endif
//                //------------------------------------------------------

//                //3.copy to gl bitmap
//                //byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
//                //------------------------------------------------------
//                //debug save image from agg's buffer 

//                //------------------------------------------------------
//                //GLBitmap glBmp = new GLBitmap(bmpWidth, bmpHeight, buffer, true);

//                //TODO: review here again ***
//                //use cache buffer instead of creating the buffer every time

//                GLBitmap glBmp = new GLBitmap(new PixelFarm.Drawing.MemBitmapBinder(_memBmp, false));
//                glBmp.IsYFlipped = false;
//                //TODO: review font height 
//                //if (StartDrawOnLeftTop)
//                //{
//                y += _fontAtlasTextPrinter.FontLineSpacingPx;
//                //}
//                _pcx.DrawGlyphImage(glBmp, (float)x, (float)y + dyOffset);
//                glBmp.Dispose();
//            }
//        }
//        public void PrepareStringForRenderVx(RenderVxFormattedString renderVx, char[] text, int start, int len)
//        {
//            GLRenderVxFormattedString glRenderVx = (GLRenderVxFormattedString)renderVx;
//            glRenderVx.buff = text;
//            glRenderVx.start = start;
//            glRenderVx.len = len;


//        }
//        public void DrawString(RenderVxFormattedString renderVx, double x, double y)
//        {
//            //1. clear prev drawing result
//            //float x = 0;
//            //float y = 0;

//            GLRenderVxFormattedString glRenderVx = (GLRenderVxFormattedString)renderVx;
//            _aggPainter.Clear(Drawing.Color.White);
//            _aggPainter.FillColor = Color.Black;
//            _fontAtlasTextPrinter.DrawString(glRenderVx.buff, glRenderVx.start, glRenderVx.len, 0, 0);
//            //_memBmp.SaveImage("d:\\WImageTest\\t02.png");
//            MeasureString(glRenderVx.buff, glRenderVx.start, glRenderVx.len, out int width, out int height);
//            //3.copy to gl bitmap
//            //byte[] buffer = PixelFarm.Agg.ActualImage.GetBuffer(_actualImage);
//            //------------------------------------------------------
//            //TODO: review here, use reusable-bitmap instead of current new one everytime.
//            GLBitmap glBmp = new GLBitmap(new PixelFarm.Drawing.MemBitmapBinder(_memBmp, false));
//            glBmp.IsYFlipped = false;
//            //TODO: review font height
//            if (StartDrawOnLeftTop)
//            {
//                y -= _fontAtlasTextPrinter.FontLineSpacingPx;
//            }
//            //PixelFarm.CpuBlit.MemBitmapExtensions.SaveImage()
//            //_pcx.DrawSubImage(glBmp, 0, 0, width, height, (float)x, (float)y);
//            _pcx.DrawSubImage(glBmp, 0, 0, width, height, (float)Math.Round(x), (float)Math.Round(y));
//            glBmp.Dispose();
//        }
//    }
//}