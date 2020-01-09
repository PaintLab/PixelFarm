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
        public override RenderVxFormattedString CreateFormattedString(char[] buffer, int startAt, int len, bool delay)
        {
            if (_gpuPainter.TextPrinter == null)
            {
#if DEBUG
                throw new System.Exception("no text printer");
#endif
            }
            //create blank render vx
            var renderVxFmtStr = new DrawingGL.GLRenderVxFormattedString();
            renderVxFmtStr.Delay = delay;
#if DEBUG
            renderVxFmtStr.dbugText = new string(buffer, startAt, len);
#endif
            if (_gpuPainter.TextPrinter != null)
            {
                //we create
                //1. texture coords for this string
                //2. (if not delay) => an image for this string  inside a larger img texture
                _gpuPainter.TextPrinter.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);
            }

            return renderVxFmtStr;
        }

        public void PrepareWordStrips(System.Collections.Generic.List<DrawingGL.GLRenderVxFormattedString> fmtStringList)
        {
            _gpuPainter.CreateWordStrips(fmtStringList);
        }

        public override void DrawRenderVx(RenderVx renderVx, float x, float y)
        {
            if (renderVx is DrawingGL.GLRenderVxFormattedString vxFmtStr)
            {
                y += vxFmtStr.DescendingInPx;
                if (vxFmtStr.UseWithWordPlate && vxFmtStr.OwnerPlate == null)
                {
                    //TODO: review here again!
                    _gpuPainter.TextPrinter.DrawString(vxFmtStr, x, y);
                }
                else
                {
                    _gpuPainter.TextPrinter.DrawString(vxFmtStr, x, y);
                }

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