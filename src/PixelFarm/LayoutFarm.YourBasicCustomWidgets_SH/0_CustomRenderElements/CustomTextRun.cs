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




        public CustomTextRun(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
#if DEBUG
            //dbugBreak = true;
#endif
            _font = rootgfx.DefaultTextEditFontInfo;
            NeedPreRenderEval = true;
            DrawTextTechnique = DrawTextTechnique.Stencil;//default
        }

        public DrawTextTechnique DrawTextTechnique { get; set; }
        public bool DelayFormattedString { get; set; }

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
            set
            {
                _backColor = value;
                BgIsNotOpaque = value.A < 255;
            }
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
                NeedPreRenderEval = true;
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
        protected override void PreRenderEvaluation(DrawBoard d)
        {
            //in this case we use formatted string
            //do not draw anything on this stage
            if (_textBuffer != null && _textBuffer.Length > 2)
            {
                //for long text ? => configurable? 
                //use formatted string
                if (_renderVxFormattedString == null)
                {
                    if (d == null) { return; }

                    Color prevColor = d.CurrentTextColor;
                    RequestFont prevFont = d.CurrentFont;
                    DrawTextTechnique prevTechnique = d.DrawTextTechnique;

                    d.CurrentTextColor = _textColor;
                    d.CurrentFont = _font;
                    d.DrawTextTechnique = this.DrawTextTechnique;

                    //config delay or not
                    _renderVxFormattedString = d.CreateFormattedString(_textBuffer, 0, _textBuffer.Length, this.DelayFormattedString);

                    d.DrawTextTechnique = prevTechnique;
                    d.CurrentFont = prevFont;
                    d.CurrentTextColor = prevColor;
                }

                switch (_renderVxFormattedString.State)
                {
                    case RenderVxFormattedString.VxState.Ready:
                        {
                            //newsize
                            //...
                            int newW = this.Width;
                            int newH = this.Height;

                            if (!this.HasSpecificWidth)
                            {
                                newW = _contentLeft + (int)System.Math.Ceiling(_renderVxFormattedString.Width) + _contentRight;
                            }
                            if (!this.HasSpecificHeight)
                            {
                                newH = _contentTop + (int)System.Math.Ceiling(_renderVxFormattedString.SpanHeight) + _contentBottom;
                            }

                            PreRenderSetSize(newW, newH);
                            //after set this 
                            NeedPreRenderEval = false;
                        }
                        break;
                }
            }
        }
        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
#if DEBUG
            if (dbugBreak)
            {

            }
#endif
            //this render element dose not have child node, so
            //if WaitForStartRenderElement == true,
            //then we skip rendering its content
            //else if this renderElement has more child, we need to walk down)
            if (WaitForStartRenderElement)
            {
                return;
            }

            if (_textBuffer != null && _textBuffer.Length > 0)
            {
                Color prevColor = d.CurrentTextColor;
                RequestFont prevFont = d.CurrentFont;
                DrawTextTechnique prevTechnique = d.DrawTextTechnique;
                Color prevBgHint = d.TextBackgroundColorHint;

                d.CurrentTextColor = _textColor;
                d.CurrentFont = _font;
                d.DrawTextTechnique = this.DrawTextTechnique;


                if (_backColor.A > 0)
                {
                    d.FillRectangle(_backColor, 0, 0, this.Width, this.Height);
                    //for lcd-subpix, hint will help the performance
                    d.TextBackgroundColorHint = _backColor;
                }
                else
                {

                    //for lcd-subpix, hint will help the performance
                    //label has transparent bg
                    //
                    d.TextBackgroundColorHint = Color.Transparent;
                }



                if (_textBuffer.Length > 2)
                {
                    //for long text ? => configurable? 
                    //use formatted string
                    if (_renderVxFormattedString == null)
                    {
                        _renderVxFormattedString = d.CreateFormattedString(_textBuffer, 0, _textBuffer.Length, DelayFormattedString);
                    }
                    //-------------
                    switch (_renderVxFormattedString.State)
                    {
                        case RenderVxFormattedString.VxState.Ready:
                            {
                                d.DrawRenderVx(_renderVxFormattedString, _contentLeft, _contentTop);

                                ////-----
                                //d.PopClipAreaRect();
                                //Rectangle prevRect = d.CurrentClipRect;
                                ////-----
                                //d.DrawRenderVx(_renderVxFormattedString, _contentLeft, _contentTop);
                                ////drawboard.FillRectangle(Color.Yellow, _contentLeft, _contentTop, this.Width, this.Height);


                                ////-----
                                //d.PushClipAreaRect(this.Width, this.Height, ref updateArea);
                                ////-----
                            }
                            break;
                        case RenderVxFormattedString.VxState.NoStrip:
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
                    d.DrawText(_textBuffer, _contentLeft, _contentTop);
                }
                //
#if DEBUG 
                d.FillRectangle(Color.Red, 0, 0, 5, 5);
#endif
                //restore
                d.DrawTextTechnique = prevTechnique;
                d.CurrentFont = prevFont;
                d.CurrentTextColor = prevColor;
                d.TextBackgroundColorHint = prevBgHint;
            }
        }
    }
}

