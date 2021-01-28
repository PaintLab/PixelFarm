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

        public CustomTextRun(int width, int height)
            : base(width, height)
        {
#if DEBUG
            //dbugBreak = true;
#endif
            _font = GlobalRootGraphic.CurrentRootGfx.DefaultTextEditFontInfo;
            NeedPreRenderEval = true;
            TextDrawingTech = TextDrawingTech.Stencil;//default
        }
        public TextDrawingTech TextDrawingTech { get; set; }
        public bool DelayFormattedString { get; set; }
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

        public bool HasSpecificWidth { get; set; }
        public bool HasSpecificHeight { get; set; }
        public bool HasSpecificWidthAndHeight => HasSpecificWidth && HasSpecificHeight;

        public string Text
        {
            get => new string(_textBuffer);
            set
            {
                //TODO: review here
                //if input string is null => textBuffer= null               
                _textBuffer = value?.ToCharArray();
                //reset
                if (_renderVxFormattedString != null)
                {
                    _renderVxFormattedString.Dispose();
                    _renderVxFormattedString = null;
                    //----------

                    //new text may change the size of this text run
                    //similar to size change
                    //for gfx-invalidation, we need a size before change and after change 

                    //TODO: review here,... 
                    //use multi-states technique

                    if (_textBuffer == null)
                    {
                        int newW = Width;
                        int newH = Height;
                        if (!this.HasSpecificWidth)
                        {
                            newW = _contentLeft + /*0*/ +_contentRight;
                        }

                        //TODO: review here
                        if (!this.HasSpecificHeight)
                        {
                            //use current font height
                            //newH =
                        }
                        SetSize(newW, newH);
                    }
                    else //if (!this.HasSpecificWidthAndHeight) 
                    {

                        //evaluate new width or height or both


                        //TODO: split into multi-state
                        var textBufferSpan = new Typography.Text.TextBufferSpan(_textBuffer);
                        Size newSize = Typography.Text.GlobalTextService.TxtClient.MeasureString(textBufferSpan, _font);//just measure
                        int newW = Width;
                        int newH = Height;
                        if (!this.HasSpecificWidth)
                        {
                            newW = _contentLeft + (int)System.Math.Ceiling((float)newSize.Width) + _contentRight;
                        }
                        if (!this.HasSpecificHeight)
                        {
                            newH = _contentTop + (int)System.Math.Ceiling((float)newSize.Height) + _contentBottom;
                        }
                        SetSize(newW, newH);
                    }
                }
                NeedPreRenderEval = true;
            }
        }

        public RequestFont RequestFont
        {
            get => _font;
            set
            {
                if (_font != value && !Typography.Text.GlobalTextService.TxtClient.Eq(_font, value))
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

        public bool MayOverlapOther { get; set; }//temp fix

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

        const int USE_STRIP_WHEN_LENGTH_MORE_THAN = 2;
        protected override void PreRenderEvaluation(DrawBoard d)
        {
            //in this case we use formatted string
            //do not draw anything on this stage
            if (_textBuffer == null) { return; }

            if (_textBuffer.Length > USE_STRIP_WHEN_LENGTH_MORE_THAN)
            {
                //for long text =>use formatted string => configurable? 

                if (_renderVxFormattedString == null)
                {
                    if (d == null) { return; }

                    Color prevColor = d.CurrentTextColor;
                    RequestFont prevFont = d.CurrentFont;
                    TextDrawingTech prevTechnique = d.TextDrawingTech;

                    d.CurrentTextColor = _textColor;
                    d.CurrentFont = _font;
                    d.TextDrawingTech = this.TextDrawingTech;

                    //create a render-vx formatted string                     
                    _renderVxFormattedString = d.CreateFormattedString(_textBuffer, 0, _textBuffer.Length, this.DelayFormattedString);

                    d.TextDrawingTech = prevTechnique;
                    d.CurrentFont = prevFont;
                    d.CurrentTextColor = prevColor;
                }

                switch (_renderVxFormattedString.State)
                {
                    default:
                        break;
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

                            SuspendGraphicsUpdate();
                            SetSize(newW, newH);
                            ResumeGraphicsUpdate();


                            NeedPreRenderEval = false; //mark as evalution is finish
                        }
                        break;
                }
            }
            else
            {
                //short content
                //but we must update content size
                //config delay or not 
                if (!this.HasSpecificWidthAndHeight)
                {
                    int newW = this.Width;
                    int newH = this.Height;
                    var buff = new Typography.Text.TextBufferSpan(_textBuffer);
                    Size size = Typography.Text.GlobalTextService.TxtClient.MeasureString(buff, _font);
                    if (!this.HasSpecificWidth)
                    {
                        newW = _contentLeft + size.Width + _contentRight;
                    }
                    if (!this.HasSpecificHeight)
                    {
                        newH = _contentTop + size.Height + _contentBottom;
                    }

                    SuspendGraphicsUpdate();
                    SetSize(newW, newH);
                    ResumeGraphicsUpdate();

                    NeedPreRenderEval = false; //mark as evalution is finish
                }
            }
        }

#if DEBUG
        int _dbugRecreateCount;
        System.DateTime _dbugLatestRecreated;
#endif
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

            if (WaitForStartRenderElement || _textBuffer == null || _textBuffer.Length == 0) { return; } //early exit

            //----------- 

            Color prevColor = d.CurrentTextColor;
            RequestFont prevFont = d.CurrentFont;
            TextDrawingTech prevTechnique = d.TextDrawingTech;
            Color prevBgHint = d.TextBackgroundColorHint;

            d.CurrentTextColor = _textColor;
            d.CurrentFont = _font;
            d.TextDrawingTech = this.TextDrawingTech; //***

            if (_backColor.A == 255)
            {
                //opaque background
                d.FillRectangle(_backColor, 0, 0, this.Width, this.Height);
                //for lcd-subpix, hint will help the performance
                d.TextBackgroundColorHint = _backColor;
            }
            else
            {

                //for lcd-subpix, hint will help the performance
                //label has transparent bg

                //this custom text run may have transparent bg
                //but it may place on host that has opaque color

                //TODO: review this
                //1. we should use latest text color hint or not

                //in that case, we can hint the text-rendering with host color instead
                //so we try to check the host color by policy that configure  on this CustomTextRun

                //TODO: if the  
            }

            if (_textBuffer.Length > USE_STRIP_WHEN_LENGTH_MORE_THAN)
            {
                if (MayOverlapOther && _backColor.A == 0) //configure alpha here**
                {
                    //transparent
                    //short text => run
                    d.DrawText(_textBuffer, _contentLeft, _contentTop);
                }
                else
                {
                    if (_renderVxFormattedString == null)
                    {
                        _renderVxFormattedString = d.CreateFormattedString(_textBuffer, 0, _textBuffer.Length, DelayFormattedString);
                    }

                    if (_renderVxFormattedString.IsReset)
                    {
                        if (_renderVxFormattedString.State != RenderVxFormattedString.VxState.Waiting)
                        {
                            GlobalRootGraphic.CurrentRootGfx.EnqueueRenderRequest(new RenderBoxes.RenderElementRequest(
                                     this,
                                     RenderBoxes.RequestCommand.ProcessFormattedString,
                                     _renderVxFormattedString));
                        }

                    }
                    else
                    {
                        switch (_renderVxFormattedString.State)
                        {
                            case RenderVxFormattedString.VxState.Ready:
                                //bitmap strip is ready
                                d.DrawRenderVx(_renderVxFormattedString, _contentLeft, _contentTop);
                                break;
                            case RenderVxFormattedString.VxState.NoStrip:
                                //put this to the update queue system                            
                                if (_renderVxFormattedString.State != RenderVxFormattedString.VxState.Waiting)
                                {
                                    GlobalRootGraphic.CurrentRootGfx.EnqueueRenderRequest(new RenderBoxes.RenderElementRequest(
                                      this,
                                      RenderBoxes.RequestCommand.ProcessFormattedString,
                                      _renderVxFormattedString));
                                }
                                break;
                        }
                    }

                }
            }
            else
            {
                d.DrawText(_textBuffer, _contentLeft, _contentTop);
            }
            //
#if DEBUG
            d.FillRectangle(Color.Red, 0, 0, 5, 5);
#endif
            //restore
            d.TextDrawingTech = prevTechnique;
            d.CurrentFont = prevFont;
            d.CurrentTextColor = prevColor;
            d.TextBackgroundColorHint = prevBgHint;

        }
    }
}

