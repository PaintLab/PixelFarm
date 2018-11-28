//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{


    public class CustomTextRun : RenderElement
    {
        char[] _textBuffer;
        Color _textColor = Color.Black;
        RequestFont _font;
        RenderVxFormattedString _renderVxFormattedString;
#if DEBUG
        public bool dbugBreak;
#endif
        public CustomTextRun(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            _font = rootgfx.DefaultTextEditFontInfo;
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            DirectSetRootGraphics(this, rootgfx);
        }
        public string Text
        {
            get { return new string(this._textBuffer); }
            set
            {

                if (value == null)
                {
                    this._textBuffer = null;
                }
                else
                {
                    this._textBuffer = value.ToCharArray();
                }

                _renderVxFormattedString = null;
            }
        }
        public Color TextColor
        {
            get { return this._textColor; }
            set { this._textColor = value; }
        }
        public RequestFont RequestFont
        {
            get { return _font; }
            set
            {
                _font = value;
            }
        }
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            if (this._textBuffer != null)
            {
                var prevColor = canvas.CurrentTextColor;
                canvas.CurrentTextColor = _textColor;
                canvas.CurrentFont = _font;

                //for faster text drawing
                //we create a formatted-text 
                //canvas.DrawText(this.textBuffer, this.X, this.Y);
                if (_renderVxFormattedString == null)
                {
                    _renderVxFormattedString = canvas.CreateFormattedString(_textBuffer, 0, _textBuffer.Length);
                }
                canvas.DrawRenderVx(_renderVxFormattedString, 0, 0); //X=0,Y=0 because  we offset the canvas to this Y before drawing this
                canvas.CurrentTextColor = prevColor;
            }
        }
    }
}