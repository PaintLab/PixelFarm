//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomTextRun : RenderElement
    {
        char[] _textBuffer;
        Color _textColor = Color.Black; //default
        Color _backColor = Color.Transparent;
        RequestFont _font;
        RenderVxFormattedString _renderVxFormattedString;
        byte _contentLeft;
        byte _contentTop;
        byte _contentRight;
        byte _contentBottom;

        byte _borderLeft;
        byte _borderTop;
        byte _borderRight;
        byte _borderBottom;

#if DEBUG
        public bool dbugBreak;
#endif
        public CustomTextRun(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            _font = rootgfx.DefaultTextEditFontInfo;
            NeedClipArea = false;
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            DirectSetRootGraphics(this, rootgfx);
        }
        public Color TextColor
        {
            get => _textColor;
            set => _textColor = value;
        }
        public Color BackColor
        {
            get => _backColor;
            set => _backColor = value;
        }
        public string Text
        {
            get => new string(_textBuffer);
            set
            {
                //TODO: review here
                _textBuffer = (value == null) ? null : value.ToCharArray();
                //reset 
                if (_renderVxFormattedString != null)
                {
                    _renderVxFormattedString.Dispose();
                    _renderVxFormattedString = null;
                }
            }
        }

        public RequestFont RequestFont
        {
            get => _font;
            set
            {
                if (_font != value || _font.FontKey != value.FontKey)
                {
                    //font changed
                    //reset 
                    if (_renderVxFormattedString != null)
                    {
                        _renderVxFormattedString.Dispose();
                        _renderVxFormattedString = null;
                    }
                }
                _font = value;
            }
        }

        public int PaddingLeft
        {
            get => _contentLeft - _borderLeft;
            set => _contentLeft = (byte)(value + _borderLeft);
        }
        public int PaddingTop
        {
            get => _contentTop - _borderTop;
            set => _contentTop = (byte)(value + _borderTop);
        }
        public int PaddingRight
        {
            get => _contentRight - _borderRight;
            set => _contentRight = (byte)(value + _borderRight);
        }
        public int PaddingBottom
        {
            get => _contentBottom - _borderBottom;
            set => _contentBottom = (byte)(value + _borderBottom);
        }
        public void SetPaddings(byte left, byte top, byte right, byte bottom)
        {
            _contentLeft = (byte)(left + _borderLeft);
            _contentTop = (byte)(top + _borderTop);
            _contentRight = (byte)(right + _borderRight);
            _contentBottom = (byte)(bottom + _borderBottom);
        }
        public void SetPaddings(byte sameValue)
        {
            _contentLeft = (byte)(sameValue + _borderLeft);
            _contentTop = (byte)(sameValue + _borderTop);
            _contentRight = (byte)(sameValue + _borderRight);
            _contentBottom = (byte)(sameValue + _borderBottom);
        }
        //-------------------------------------------------------------------------------
        public int BoxContentWidth => this.Width - (_contentLeft + _contentRight);
        public int BoxContentHeight => this.Height - (_contentTop + _contentBottom);

        public void SetContentOffsets(byte value)
        {
            //same value
            _contentLeft =
                    _contentTop =
                    _contentRight =
                    _contentBottom = value;
        }
        public void SetContentOffsets(byte left, byte top, byte right, byte bottom)
        {
            //same value
            _contentLeft = left;
            _contentTop = top;
            _contentRight = right;
            _contentBottom = bottom;
        }
        //
        public void SetBorders(byte left, byte top, byte right, byte bottom)
        {
            //same value
            _borderLeft = left;
            _borderTop = top;
            _borderRight = right;
            _borderBottom = bottom;
        }
        public void SetBorders(byte value)
        {
            //same value
            _borderLeft =
                _borderTop =
                _borderRight =
                _borderBottom = value;
        }
        public override void CustomDrawToThisCanvas(DrawBoard drawboard, Rectangle updateArea)
        {
#if DEBUG
            if (dbugBreak)
            {

            }
#endif
            if (_textBuffer != null && _textBuffer.Length > 0)
            {
                Color prevColor = drawboard.CurrentTextColor;
                RequestFont prevFont = drawboard.CurrentFont;
                DrawTextTechnique prevTechnique = drawboard.DrawTextTechnique;

                drawboard.CurrentTextColor = _textColor;
                drawboard.CurrentFont = _font;
                drawboard.DrawTextTechnique = DrawTextTechnique.Stencil;

                if (_backColor.A > 0)
                {
                    drawboard.FillRectangle(_backColor, 0, 0, this.Width, this.Height);
                }

                if (_textBuffer.Length > 2)
                {
                    //for long text ? => configurable?                               

                    //use formatted string
                    if (_renderVxFormattedString == null)
                    {
                        _renderVxFormattedString = drawboard.CreateFormattedString(_textBuffer, 0, _textBuffer.Length);
                    }

                    switch (_renderVxFormattedString.State)
                    {
                        case RenderVxFormattedString.VxState.Ready:
                            drawboard.DrawRenderVx(_renderVxFormattedString, _contentLeft, _contentTop);
                            break;
                        case RenderVxFormattedString.VxState.NoTicket:
                            //put this to the update queue system
                            //(TODO: add extension method for this)
                            Root.EnqueueRenderRequest(new RenderBoxes.RenderElementRequest(
                                  this,
                                  RenderBoxes.RequestCommand.ProcessFormattedString,
                                  _renderVxFormattedString));
                            break;
                    }
                }
                else
                {

                    //short text => run
                    drawboard.DrawText(_textBuffer, _contentLeft, _contentTop);
                }
                drawboard.DrawTextTechnique = prevTechnique;
                drawboard.CurrentFont = prevFont;
                drawboard.CurrentTextColor = prevColor;
            }
        }
    }
}

