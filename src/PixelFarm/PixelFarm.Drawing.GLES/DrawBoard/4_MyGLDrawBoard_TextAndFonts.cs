//BSD, 2014-present, WinterDev

namespace PixelFarm.Drawing.GLES2
{
    partial class MyGLDrawBoard
    {
        //TODO: review drawstring again ***
        Color _currentTextColor;
        public override RequestFont CurrentFont
        {
            get => _gpuPainter.CurrentFont;
            set => _gpuPainter.CurrentFont = value;
        }

        /// <summary>
        /// current text fill color
        /// </summary>
        public override Color CurrentTextColor
        {
            get => _currentTextColor;
            set
            {
                _currentTextColor = value;
                //set this to 
                _gpuPainter.TextPrinter?.ChangeFillColor(value);
            }
        }
        public override RenderVxFormattedString CreateFormattedString(int[] buffer, int startAt, int len, bool delay)
        {
            if (_gpuPainter.TextPrinter == null)
            {
#if DEBUG
                throw new System.Exception("no text printer");
#endif
            }
            //create blank render vx
            var fmtstr = new DrawingGL.GLRenderVxFormattedString();
            fmtstr.Delay = delay;
#if DEBUG
            //fmtstr.dbugText = new string(buffer, startAt, len);
#endif
            if (_gpuPainter.TextPrinter != null)
            {
                //we create
                //1. texture coords for this string
                //2. (if not delay) => an image for this string  inside a larger img texture
                _gpuPainter.TextPrinter.PrepareStringForRenderVx(fmtstr, buffer, startAt, len);
                if (!fmtstr.Delay)
                {
                    fmtstr.ReleaseIntermediateStructures();
                }
                return fmtstr;
            }
            else
            {
#if DEBUG
                throw new System.NotSupportedException();
#else
                return null;
#endif
            }
        }
        public override RenderVxFormattedString CreateFormattedString(char[] buffer, int startAt, int len, bool delay)
        {
            if (_gpuPainter.TextPrinter == null)
            {
#if DEBUG
                throw new System.Exception("no text printer");
#endif
            }
            //create blank render vx
            var fmtstr = new DrawingGL.GLRenderVxFormattedString();
            fmtstr.Delay = delay;
#if DEBUG
            fmtstr.dbugText = new string(buffer, startAt, len);
#endif
            if (_gpuPainter.TextPrinter != null)
            {
                //we create
                //1. texture coords for this string
                //2. (if not delay) => an image for this string  inside a larger img texture
                _gpuPainter.TextPrinter.PrepareStringForRenderVx(fmtstr, buffer, startAt, len);
                if (!fmtstr.Delay)
                {
                    fmtstr.ReleaseIntermediateStructures();
                }
                return fmtstr;
            }
            else
            {
#if DEBUG
                throw new System.NotSupportedException();
#else
                return null;
#endif
            }
        }
        public void PrepareWordStrips(System.Collections.Generic.List<DrawingGL.GLRenderVxFormattedString> fmtStringList)
        {
            _gpuPainter.CreateWordStrips(fmtStringList);
        }

        public override void DrawRenderVx(RenderVx renderVx, float x, float y)
        {
            if (renderVx is DrawingGL.GLRenderVxFormattedString vxFmtStr)
            {
                _gpuPainter.TextPrinter.DrawString(vxFmtStr, x, y);

                //if (vxFmtStr.BmpOnTransparentBackground)
                //{
                //    DrawingGL.GlyphTexturePrinterDrawingTechnique prevTech = _gpuPainter.TextPrinterDrawingTechnique; //save
                //    _gpuPainter.TextPrinterDrawingTechnique = DrawingGL.GlyphTexturePrinterDrawingTechnique.Copy;
                //    _gpuPainter.TextPrinter.DrawString(vxFmtStr, x, y);
                //    _gpuPainter.TextPrinterDrawingTechnique = prevTech;//restore
                //}
                //else
                //{
                //    _gpuPainter.TextPrinter.DrawString(vxFmtStr, x, y);
                //}

            }
        }

        public override void DrawText(char[] buffer, int left, int top)
        {
#if DEBUG
            if (_gpuPainter.FontFillColor.A == 0)
            {

            }
#endif
            _gpuPainter.TextPrinter.DrawString(buffer, 0, buffer.Length, left, top);

        }

        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
#if DEBUG
            if (_gpuPainter.FontFillColor.A == 0)
            {

            }
#endif
            _gpuPainter.TextPrinter.DrawString(buffer, 0, buffer.Length, logicalTextBox.X, logicalTextBox.Y);
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
#if DEBUG
            if (_gpuPainter.FontFillColor.A == 0)
            {

            }
#endif

            _gpuPainter.TextPrinter.DrawString(str, startAt, len, logicalTextBox.X, logicalTextBox.Y);
        }
    }
}