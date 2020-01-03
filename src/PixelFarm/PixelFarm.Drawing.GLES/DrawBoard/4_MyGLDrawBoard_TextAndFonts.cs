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
        public override RenderVxFormattedString CreateFormattedString(char[] buffer, int startAt, int len)
        {
            if (_gpuPainter.TextPrinter == null)
            {
#if DEBUG
                throw new System.Exception("no text printer");
#endif
            }
            //create blank render vx
            var renderVxFmtStr = new DrawingGL.GLRenderVxFormattedString();
#if DEBUG
            //renderVxFmtStr.dbugText = new string(buffer, startAt, len); 

#endif
            if (_gpuPainter.TextPrinter != null)
            {
                //we create an image for this string 
                //inside a larger img texture 
                _gpuPainter.SetCurrentCanvasForTextPrinter(this);
                _gpuPainter.TextPrinter.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);
                _gpuPainter.SetCurrentCanvasForTextPrinter(null);
            }
            return renderVxFmtStr;
        }
        public override void DrawRenderVx(RenderVx renderVx, float x, float y)
        {
            if (renderVx is DrawingGL.GLRenderVxFormattedString formattedString)
            {
                if (formattedString.UseWithWordPlate && formattedString.WordPlateId == 0)
                {
                    if (formattedString.PreparingWordTicket)
                    {
                        //should not occure here
                        throw new System.NotSupportedException();
                        //_gpuPainter.SetCurrentCanvasForTextPrinter(null);//***
                        //_gpuPainter.TextPrinter.DrawString(formattedString, x, y);
                        //formattedString.PreparingWordTicket = false;
                    }
                    else
                    {
                        formattedString.PreparingWordTicket = true;
                        _gpuPainter.SetCurrentCanvasForTextPrinter(this);
                        _gpuPainter.TextPrinter.DrawString(formattedString, x, y);
                        _gpuPainter.SetCurrentCanvasForTextPrinter(null);
                        formattedString.PreparingWordTicket = false;
                    }
                }
                else
                {
                    _gpuPainter.TextPrinter.DrawString(formattedString, x, y);
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