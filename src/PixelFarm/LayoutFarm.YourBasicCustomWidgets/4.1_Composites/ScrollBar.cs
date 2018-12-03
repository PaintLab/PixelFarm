//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public delegate void ScrollBarEvaluator(SliderBox scBar, out double onePixelFore, out int scrollBoxHeight);



    struct ScrollRangeLogic
    {
        float _maxValue;
        float _minValue;
        float _largeChange;
        float _smallChange;
        float _currentValue;


        public ScrollRangeLogic(float min, float max, float largeChange, float smallChange)
        {
            _maxValue = max;
            _minValue = min;
            _currentValue = min;//start at min value
            _largeChange = largeChange;
            _smallChange = smallChange;
            //validate values
        }
        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }
        //
        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }
        //
        public float LargeChange
        {
            get => _largeChange;
            set => _largeChange = value;
        }
        //
        public float SmallChange
        {
            get => _smallChange;
            set => _smallChange = value;
        }
        //
        public float CurrentValue => _currentValue;
        //
        public bool SetValue(float value)
        {
            //how to handle over-range value
            //throw ?
            //auto reset?
            float tmpValue = value;
            if (tmpValue < _minValue)
            {
                tmpValue = _minValue;
            }
            else if (tmpValue > _maxValue)
            {
                tmpValue = _maxValue;
            }
            //changed?
            bool valueChanged = tmpValue != _currentValue;
            _currentValue = tmpValue;
            return valueChanged;
        }

        //
        public float ValueRange => _maxValue - _minValue;
        //
        public bool SmallStepToMax() => SetValue(_currentValue + _smallChange);
        //
        public bool SmallStepToMin() => SetValue(_currentValue - _smallChange);
        //
        public bool LargeStepToMax() => SetValue(_currentValue + _largeChange);
        //
        public bool LargeStepToMin() => SetValue(_currentValue - _largeChange);
        //
    }

    public delegate void UIEventHandler<S, T>(S sender, T arg);

    public class SliderBox : AbstractRectUI
    {

        ScrollRangeLogic _scrollRangeLogic;
        CustomRenderBox _mainBox;
        //

        ScrollBarEvaluator _customScrollBarEvaluator;
        ScrollBarButton _scrollButton;

        public event UIEventHandler<SliderBox, bool> NeedScollBoxEvent;

        double _onePixelFor = 1;
        const int SCROLL_BOX_SIZE_LIMIT = 10;
        public SliderBox(int width, int height)
            : base(width, height)
        {

        }
        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            ReEvaluateScrollBar();

            //evaluate scrollButton position and size again
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _mainBox;
        //
        protected override bool HasReadyRenderElement => _mainBox != null;
        //
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_mainBox == null)
            {
                switch (this.ScrollBarType)
                {
                    case ScrollBarType.Horizontal:
                        {
                            CreateHScrollbarContent(rootgfx);
                        }
                        break;
                    default:
                        {
                            CreateVScrollbarContent(rootgfx);
                        }
                        break;
                }
            }
            return _mainBox;
        }
        public ScrollBarType ScrollBarType
        {
            get;
            set;
        }
        //--------------------------------------------------------------------------

        public int ScrollBoxSizeLimit => SCROLL_BOX_SIZE_LIMIT;

        public int PhysicalScrollLength
        {
            get
            {
                if (ScrollBarType == ScrollBarType.Vertical)
                {
                    return this.Height;
                }
                else
                {
                    return this.Width;
                }
            }
        }



        public void StepSmallToMax()
        {

            _scrollRangeLogic.SmallStepToMax();
            //---------------------------
            //update visual presentation             
            UpdateScrollButtonPosition();
            if (this.UserScroll != null)
            {
                this.UserScroll(this, EventArgs.Empty);
            }
        }
        public void StepSmallToMin()
        {

            _scrollRangeLogic.SmallStepToMin();
            //---------------------------
            //update visual presentation   
            UpdateScrollButtonPosition();
            if (this.UserScroll != null)
            {
                this.UserScroll(this, EventArgs.Empty);
            }
        }
        public void StepLargeToMax()
        {

            _scrollRangeLogic.LargeStepToMax();
            //---------------------------
            //update visual presentation             
            UpdateScrollButtonPosition();
            if (this.UserScroll != null)
            {
                this.UserScroll(this, EventArgs.Empty);
            }
        }
        public void StepLargeToMin()
        {

            _scrollRangeLogic.LargeStepToMin();
            //---------------------------
            //update visual presentation   
            UpdateScrollButtonPosition();
            if (this.UserScroll != null)
            {
                this.UserScroll(this, EventArgs.Empty);
            }
        }

        void UpdateScrollButtonPosition()
        {
            if (_scrollButton == null) return;
            switch (this.ScrollBarType)
            {
                default:
                case ScrollBarType.Vertical:
                    {
                        int thumbPosY = CalculateThumbPosition();
                        _scrollButton.SetLocation(0, thumbPosY);
                    }
                    break;
                case ScrollBarType.Horizontal:
                    {
                        int thumbPosX = CalculateThumbPosition();
                        _scrollButton.SetLocation(thumbPosX, 0);
                    }
                    break;
            }
        }


        //--------------------------------------------------------------------------
        void CreateVScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificWidthAndHeight = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);

            SetupVerticalScrollButtonProperties(bgBox);
            //--------------
            this._mainBox = bgBox;
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificWidthAndHeight = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);

            SetupHorizontalScrollButtonProperties(bgBox);
            //--------------
            this._mainBox = bgBox;
        }

        //----------------------------------------------------------------------- 
        int CalculateThumbPosition()
        {
            return (int)Math.Round(_scrollRangeLogic.CurrentValue / this._onePixelFor);
        }


        public void ReEvaluateScrollBar()
        {
            if (this._scrollButton == null)
            {
                return;
            }
            //-------------------------
            switch (this.ScrollBarType)
            {
                default:
                case ScrollBarType.Vertical:
                    {
                        EvaluateVerticalScrollBarProperties();
                    }
                    break;
                case ScrollBarType.Horizontal:
                    {
                        EvaluateHorizontalScrollBarProperties();
                    }
                    break;
            }
        }
        public void SetCustomScrollBarEvaluator(ScrollBarEvaluator scrollBarEvaluator)
        {
            _customScrollBarEvaluator = scrollBarEvaluator;
        }
        void EvaluateVerticalScrollBarProperties()
        {
            int scrollBoxLength = 1;
            //--------------------------
            //if use external evaluator
            if (_customScrollBarEvaluator != null)
            {
                _customScrollBarEvaluator(this, out this._onePixelFor, out scrollBoxLength);
            }
            else
            {
                //--------------------------
                //calculate scroll length ratio
                //scroll button height is ratio with real scroll length
                float contentLength = _scrollRangeLogic.ValueRange;
                //2. 
                float physicalScrollLength = this.Height;
                //3.  
                if (contentLength < physicalScrollLength)
                {
                    int nsteps = (int)Math.Round(contentLength / _scrollRangeLogic.SmallChange);
                    //small change value reflect thumbbox size 
                    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                    scrollBoxLength = eachStepLength * 2;
                    this._onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    scrollBoxLength = (int)((physicalScrollLength * physicalScrollLength) / contentLength);
                    //small change value reflect thumbbox size
                    // scrollBoxLength = (int)(ratio1 * this.SmallChange);
                    //thumbbox should not smaller than minimum limit 
                    if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                    {
                        scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                    }

                    this._onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
            }
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                throw new NotSupportedException();
            }
            else
            {
                //vertical scrollbar
                _scrollButton.SetSize(
                   _scrollButton.Width,
                   scrollBoxLength);
                this.InvalidateOuterGraphics();
            }

            //---------------------------------
            EvalNeedScrollBox();

        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (e.X < _scrollButton.Left)
            {
                //to min
                StepSmallToMin();
            }
            else if (e.X > _scrollButton.Right)
            {
                //to max
                StepSmallToMax();
            }

            base.OnMouseDown(e);
        }
        void SetupVerticalScrollButtonProperties(RenderElement container)
        {
            //var scroll_button = new ScrollBarButton(this.Width+10, SCROLL_BOX_SIZE_LIMIT, this); //for test
            var scroll_button = new ScrollBarButton(this.Width, SCROLL_BOX_SIZE_LIMIT, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosY = CalculateThumbPosition();
            scroll_button.SetLocation(0, thumbPosY);
            container.AddChild(scroll_button);
            this._scrollButton = scroll_button;
            //----------------------------
            EvaluateVerticalScrollBarProperties();
            //----------------------------
            //3. drag
            scroll_button.MouseDrag += (s, e) =>
            {
                //dragging ...
                //find y-diff   

                Point pos = scroll_button.Position;
                //if vscroll bar then move only y axis 
                int newYPos = (int)(pos.Y + e.DiffCapturedY);
                //clamp!
                if (newYPos >= this.Height - (_scrollButton.Height))
                {
                    newYPos = this.Height - (_scrollButton.Height);
                }
                else if (newYPos < 0)
                {
                    newYPos = 0;
                }

                //calculate value from position 

                int currentMarkAt = newYPos;
                _scrollRangeLogic.SetValue((float)(_onePixelFor * currentMarkAt));
                newYPos = CalculateThumbPosition();
                scroll_button.SetLocation(pos.X, newYPos);
                if (this.UserScroll != null)
                {
                    this.UserScroll(this, EventArgs.Empty);
                }

                e.StopPropagation();
            };
            //----------------------------
            //scroll_button.MouseLeave += (s, e) =>
            //{
            //    if (e.IsDragging)
            //    {
            //        Point pos = scroll_button.Position;
            //        //if vscroll bar then move only y axis 
            //        int newYPos = (int)(pos.Y + e.YDiff);
            //        //clamp!
            //        if (newYPos >= this.Height - (minmax_boxHeight + scrollButton.Height))
            //        {
            //            newYPos = this.Height - (minmax_boxHeight + scrollButton.Height);
            //        }
            //        else if (newYPos < minmax_boxHeight)
            //        {
            //            newYPos = minmax_boxHeight;
            //        }

            //        //calculate value from position 

            //        int currentMarkAt = (newYPos - minmax_boxHeight);
            //        this.scrollValue = (float)(onePixelFor * currentMarkAt);
            //        newYPos = CalculateThumbPosition() + minmax_boxHeight;
            //        scroll_button.SetLocation(pos.X, newYPos);
            //        if (this.UserScroll != null)
            //        {
            //            this.UserScroll(this, EventArgs.Empty);
            //        }

            //        e.StopPropagation();
            //    }
            //};
        }

        //---------------------------------------------------------------------------
        //horizontal scrollbar
        void EvaluateHorizontalScrollBarProperties()
        {
            int scrollBoxLength = 1;
            //--------------------------
            //if use external evaluator
            if (_customScrollBarEvaluator != null)
            {
                _customScrollBarEvaluator(this, out this._onePixelFor, out scrollBoxLength);
            }
            else
            {
                //calculate scroll length ratio
                //scroll button height is ratio with real scroll length
                float contentLength = _scrollRangeLogic.ValueRange;
                //2. 
                float physicalScrollLength = this.Width;
                //3. 
                //double ratio1 = physicalScrollLength / contentLength;
                //if (contentLength < physicalScrollLength)
                //{
                //    int nsteps = (int)Math.Round(contentLength / smallChange);
                //    //small change value reflect thumbbox size
                //    // thumbBoxLength = (int)(ratio1 * this.SmallChange);
                //    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                //    scrollBoxLength = eachStepLength * 2;
                //    //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                //    //this.onePixelFor = contentLength / (physicalScrollLength);
                //    this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                //}
                //else
                //{
                //    scrollBoxLength = (int)(ratio1 * this.SmallChange);
                //    //thumbbox should not smaller than minimum limit 
                //    if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                //    {
                //        scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                //        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                //    }
                //    else
                //    {
                //        float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                //        this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                //    }
                //}
                //3.  
                if (contentLength < physicalScrollLength)
                {
                    int nsteps = (int)Math.Round(contentLength / _scrollRangeLogic.SmallChange);
                    //small change value reflect thumbbox size 
                    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                    scrollBoxLength = eachStepLength * 2;
                    this._onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    scrollBoxLength = (int)((physicalScrollLength * physicalScrollLength) / contentLength);
                    //small change value reflect thumbbox size
                    // scrollBoxLength = (int)(ratio1 * this.SmallChange);
                    //thumbbox should not smaller than minimum limit 
                    if (scrollBoxLength < SCROLL_BOX_SIZE_LIMIT)
                    {
                        scrollBoxLength = SCROLL_BOX_SIZE_LIMIT;
                    }
                    this._onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                }
            }
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                this._scrollButton.SetSize(
                    scrollBoxLength,
                    this._scrollButton.Height);
            }
            else
            {
                throw new NotSupportedException();
            }
            //---------------------------------
            EvalNeedScrollBox();
        }

        void SetupHorizontalScrollButtonProperties(RenderElement container)
        {
            //TODO: use 'theme-concept' eg. css

            var scroll_button = new ScrollBarButton(SCROLL_BOX_SIZE_LIMIT, this.Height, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.Gray);
            int thumbPosX = CalculateThumbPosition();
            scroll_button.SetLocation(thumbPosX, 0);
            container.AddChild(scroll_button);
            this._scrollButton = scroll_button;
            //----------------------------

            EvaluateHorizontalScrollBarProperties();
            //----------------------------
            //3. drag 
            scroll_button.MouseDrag += (s, e) =>
            {
                //dragging ...

                Point pos = scroll_button.Position;

                int newXPos = (int)(pos.X + e.DiffCapturedX);
                //clamp!
                if (newXPos >= this.Width - (_scrollButton.Width))
                {
                    newXPos = this.Width - (_scrollButton.Width);
                }
                else if (newXPos < 0)
                {
                    newXPos = 0;
                }

                //calculate value from position 

                int currentMarkAt = newXPos;
                this._scrollRangeLogic.SetValue((float)(_onePixelFor * currentMarkAt));

                newXPos = CalculateThumbPosition();
                scroll_button.SetLocation(newXPos, pos.Y);
                if (this.UserScroll != null)
                {
                    this.UserScroll(this, EventArgs.Empty);
                }
                e.StopPropagation();
            };
            ////-------------------------------------------
            ////4.
            //scroll_button.MouseLeave += (s, e) =>
            //{
            //    if (e.IsDragging)
            //    {
            //        Point pos = scroll_button.Position;
            //        //if vscroll bar then move only y axis 
            //        int newXPos = (int)(pos.X + e.XDiff);
            //        //clamp!
            //        if (newXPos >= this.Width - (minmax_boxHeight + scrollButton.Width))
            //        {
            //            newXPos = this.Width - (minmax_boxHeight + scrollButton.Width);
            //        }
            //        else if (newXPos < minmax_boxHeight)
            //        {
            //            newXPos = minmax_boxHeight;
            //        }

            //        //calculate value from position 

            //        int currentMarkAt = (newXPos - minmax_boxHeight);
            //        this.scrollValue = (float)(onePixelFor * currentMarkAt);
            //        newXPos = CalculateThumbPosition() + minmax_boxHeight;
            //        scroll_button.SetLocation(newXPos, pos.Y);
            //        if (this.UserScroll != null)
            //        {
            //            this.UserScroll(this, EventArgs.Empty);
            //        }
            //        e.StopPropagation();
            //    }
            //};
        }
        //----------------------------------------------------------------------- 
        void EvalNeedScrollBox()
        {
            if (this.MaxValue == 0)
            {
                //not need scrollbox
                if (_scrollButton.Visible)
                {
                    //hide scroll box
                    _scrollButton.Visible = false;
                    //only raise event when visibility change
                    NeedScollBoxEvent?.Invoke(this, false);
                }
            }
            else
            {
                if (!_scrollButton.Visible)
                {
                    //only raise event when visibility change
                    _scrollButton.Visible = true;
                    NeedScollBoxEvent?.Invoke(this, true);

                }
            }
        }
        public void SetupScrollBar(ScrollBarCreationParameters creationParameters)
        {
            this.MaxValue = creationParameters.maximum;
            this.MinValue = creationParameters.minmum;
        }
        public float MaxValue
        {
            get => _scrollRangeLogic.MaxValue;
            set => _scrollRangeLogic.MaxValue = value;
            //need update 
        }
        public float MinValue
        {
            get => _scrollRangeLogic.MinValue;
            set => _scrollRangeLogic.MinValue = value;
            //need update 
        }
        public float SmallChange
        {
            get => _scrollRangeLogic.SmallChange;
            set => _scrollRangeLogic.SmallChange = value;

        }
        public float LargeChange
        {
            get => _scrollRangeLogic.LargeChange;
            set => _scrollRangeLogic.LargeChange = value;
        }
        public float ScrollValue
        {
            get => _scrollRangeLogic.CurrentValue;
            set
            {
                if (_scrollRangeLogic.SetValue(value))
                {
                    //need update 
                    ReEvaluateScrollBar();
                    UpdateScrollButtonPosition();
                }

            }
        }
        //-----------------------------------------------------------------------

        public event EventHandler<EventArgs> UserScroll;
        //tempfix here
        internal void ChildNotifyMouseWheel(UIMouseEventArgs e)
        {
            if (e.Delta < 0)
            {   //scroll down
                this.StepSmallToMax();
            }
            else
            {
                //up
                this.StepSmallToMin();
            }
        }
        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            if (e.Delta < 0)
            {   //scroll down
                this.StepSmallToMax();
            }
            else
            {
                //up
                this.StepSmallToMin();
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "scrollbar");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
    public class ScrollBar : AbstractRectUI
    {

        ScrollBarButton _minButton;
        ScrollBarButton _maxButton;
        SliderBox _slideBox;
        CustomRenderBox _mainBox;

        int _minmax_boxHeight = 15;

        public ScrollBar(int width, int height)
            : base(width, height)
        {
            _slideBox = new SliderBox(_minmax_boxHeight, _minmax_boxHeight);
            _slideBox.NeedScollBoxEvent += (s, need) =>
            {
                if (need)
                {
                    //if (!this.Visible)
                    //{
                    //    this.Visible = true;
                    //}
                }
                else
                {
                    //if (this.Visible)
                    //{
                    //    this.Visible = false;
                    //} 
                }
            };
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _mainBox;
        protected override bool HasReadyRenderElement => _mainBox != null;
        //
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_mainBox == null)
            {
                switch (this.ScrollBarType)
                {
                    case ScrollBarType.Horizontal:
                        {
                            CreateHScrollbarContent(rootgfx);
                        }
                        break;
                    default:
                        {
                            CreateVScrollbarContent(rootgfx);
                        }
                        break;
                }
            }
            return _mainBox;
        }
        public ScrollBarType ScrollBarType
        {
            get => _slideBox.ScrollBarType;
            set => _slideBox.ScrollBarType = value;
        }
        //--------------------------------------------------------------------------

        public SliderBox SliderBox => _slideBox;
        public int MinMaxButtonHeight => _minmax_boxHeight;
        public int ScrollBoxSizeLimit => _slideBox.ScrollBoxSizeLimit;

        public int PhysicalScrollLength
        {
            get
            {
                if (ScrollBarType == ScrollBarType.Vertical)
                {
                    return this.Height - (_minmax_boxHeight + _minmax_boxHeight);
                }
                else
                {
                    return this.Width - (_minmax_boxHeight + _minmax_boxHeight);
                }
            }
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            //layout scrollbar component too.

            if (_minButton == null) return;
            //
            //
            _mainBox.SetSize(width, height);
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                //horizontal
                _maxButton.SetLocation(this.Width - _minmax_boxHeight, 0);
                _slideBox.SetSize(this.Width - _minmax_boxHeight * 2, this.Height);
            }
            else
            {
                _maxButton.SetLocation(0, this.Height - _minmax_boxHeight);
                _slideBox.SetSize(this.Width, this.Height - _minmax_boxHeight * 2);
            }
        }

        public void StepSmallToMax()
        {
            _slideBox.StepSmallToMax();
        }
        public void StepSmallToMin()
        {
            _slideBox.StepSmallToMin();
        }

        //--------------------------------------------------------------------------
        void CreateVScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificWidthAndHeight = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);
            //---------------------------------------------------------
            _slideBox.ScrollBarType = ScrollBarType.Vertical;
            _slideBox.SetLocation(0, _minmax_boxHeight);
            _slideBox.SetSize(this.Width, this.Height - _minmax_boxHeight * 2);

            RenderElement sliderRenderE = _slideBox.GetPrimaryRenderElement(rootgfx);
            bgBox.AddChild(sliderRenderE);

            //MinButton
            SetupMinButtonProperties(bgBox);
            //MaxButton
            SetupMaxButtonProperties(bgBox);
            //ScrollButton
            SetupVerticalScrollButtonProperties(bgBox);
            //--------------
            this._mainBox = bgBox;
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {


            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificWidthAndHeight = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);
            //---------------------------------------------------------
            _slideBox.ScrollBarType = ScrollBarType.Horizontal;
            _slideBox.SetLocation(_minmax_boxHeight, 0);
            _slideBox.SetSize(this.Width - _minmax_boxHeight * 2, this.Height);
            RenderElement sliderRenderE = _slideBox.GetPrimaryRenderElement(rootgfx);

            bgBox.AddChild(sliderRenderE);

            //MinButton
            SetupMinButtonProperties(bgBox);
            //MaxButton
            SetupMaxButtonProperties(bgBox);
            //ScrollButton
            SetupHorizontalScrollButtonProperties(bgBox);
            //--------------
            this._mainBox = bgBox;
        }

        void SetupMinButtonProperties(RenderElement container)
        {
            ScrollBarButton min_button;
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                min_button = new ScrollBarButton(_minmax_boxHeight, this.Height, this);
            }
            else
            {
                min_button = new ScrollBarButton(this.Width, _minmax_boxHeight, this);
            }
            min_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            min_button.MouseUp += (s, e) => this.StepSmallToMin();
            container.AddChild(min_button);
            this._minButton = min_button;
        }
        void SetupMaxButtonProperties(RenderElement container)
        {
            ScrollBarButton max_button;
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                max_button = new ScrollBarButton(_minmax_boxHeight, this.Height, this);
                max_button.SetLocation(this.Width - _minmax_boxHeight, 0);
            }
            else
            {
                max_button = new ScrollBarButton(this.Width, _minmax_boxHeight, this);
                max_button.SetLocation(0, this.Height - _minmax_boxHeight);
            }


            max_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            max_button.MouseUp += (s, e) => this.StepSmallToMax();
            container.AddChild(max_button);
            this._maxButton = max_button;
        }

        //---------------------------------------------------------------------------
        //vertical scrollbar

        public void SetCustomScrollBarEvaluator(ScrollBarEvaluator scrollBarEvaluator)
        {
            _slideBox.SetCustomScrollBarEvaluator(scrollBarEvaluator);
        }

        void SetupVerticalScrollButtonProperties(RenderElement container)
        {
        }

        void SetupHorizontalScrollButtonProperties(RenderElement container)
        {

        }
        //----------------------------------------------------------------------- 
        public void SetupScrollBar(ScrollBarCreationParameters creationParameters)
        {
            this.MaxValue = creationParameters.maximum;
            this.MinValue = creationParameters.minmum;
        }
        public float MaxValue
        {
            get => _slideBox.MaxValue;
            set => _slideBox.MaxValue = value;
            //need update 
        }
        public float MinValue
        {
            get => _slideBox.MinValue;
            set => _slideBox.MinValue = value;
            //need update 
        }
        public float SmallChange
        {
            get => _slideBox.SmallChange;
            set => _slideBox.SmallChange = value;
            //need update 
        }
        public float LargeChange
        {
            get => _slideBox.LargeChange;
            set => _slideBox.LargeChange = value;
            //need update 
        }
        public float ScrollValue
        {
            get => _slideBox.ScrollValue;
            set => _slideBox.ScrollValue = value;
        }

        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            if (e.Delta < 0)
            {   //scroll down
                this.StepSmallToMax();
            }
            else
            {
                //up
                this.StepSmallToMin();
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "scrollbar");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }

    public class ScrollBarCreationParameters
    {
        public Rectangle elementBound;
        public Size arrowBoxSize;
        public int thumbBoxLimit;
        public float maximum;
        public float minmum;
        public float largeChange;
        public float smallChange;
        public ScrollBarType scrollBarType;
    }


    public enum ScrollBarType
    {
        Vertical,
        Horizontal
    }

    class ScrollBarButton : AbstractBox
    {
        public ScrollBarButton(int w, int h, IUIEventListener owner)
            : base(w, h)
        {
            this.OwnerScrollBar = owner;
        }
        IUIEventListener OwnerScrollBar
        {
            get;
            set;
        }
        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            this.OwnerScrollBar.ListenMouseWheel(e);
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "scrollbutton");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }



    /// <summary>
    /// instance that create and hold 'scrolling' relation between SliderBox and IScrollableElement
    /// </summary>
    public class ScrollingRelation
    {
        SliderBox _slideBox;
        IScrollable _scrollableSurface;
        public ScrollingRelation(SliderBox slideBox, IScrollable scrollableSurface)
        {
            _slideBox = slideBox;
            _scrollableSurface = scrollableSurface;
            switch (_slideBox.ScrollBarType)
            {
                case ScrollBarType.Vertical:
                    {
                        SetupVerticalScrollRelation();
                    }
                    break;
                case ScrollBarType.Horizontal:
                    {
                        SetupHorizontalScrollRelation();
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        void SetupVerticalScrollRelation()
        {
            _slideBox.SetCustomScrollBarEvaluator((SliderBox sc, out double onePixelFor, out int scrollBoxLength) =>
            {
                float physicalScrollLength = sc.PhysicalScrollLength;
                onePixelFor = 1;
                scrollBoxLength = 1;
                //1. 

                float contentLength = _scrollableSurface.InnerHeight;
                if (contentLength == 0)
                {
                    return;
                }
                scrollBoxLength = (int)Math.Round((physicalScrollLength * _scrollableSurface.ViewportHeight) / contentLength);
                if (scrollBoxLength < sc.ScrollBoxSizeLimit)
                {
                    scrollBoxLength = sc.ScrollBoxSizeLimit;
                    //viewport ratio
                    onePixelFor = (contentLength - _scrollableSurface.ViewportHeight) / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    onePixelFor = contentLength / physicalScrollLength;
                }

                //temp fix 
                sc.MaxValue = (contentLength > _scrollableSurface.ViewportHeight) ?
                   contentLength - _scrollableSurface.ViewportHeight :
                   0;

            });
            //--------------------------------------------------------------------------------------
            //1st evaluate  
            _slideBox.MaxValue = _scrollableSurface.InnerHeight;
            _slideBox.ReEvaluateScrollBar();
            _scrollableSurface.LayoutFinished += (s, e) =>
            {
                _slideBox.MaxValue = _scrollableSurface.InnerHeight;
                _slideBox.ReEvaluateScrollBar();
            };
            _scrollableSurface.ViewportChanged += (s, e) =>
            {
                if (s != _slideBox)
                {
                    //change scrollbar
                    _slideBox.ScrollValue = _scrollableSurface.ViewportY;
                }
            };
            _slideBox.UserScroll += (s, e) =>
            {
                _scrollableSurface.SetViewport(_scrollableSurface.ViewportX, (int)_slideBox.ScrollValue, _slideBox);
            };
        }
        void SetupHorizontalScrollRelation()
        {
            this._slideBox.SetCustomScrollBarEvaluator((SliderBox sc, out double onePixelFor, out int scrollBoxLength) =>
            {
                //horizontal scroll bar
                float physicalScrollLength = sc.PhysicalScrollLength;
                onePixelFor = 1;
                scrollBoxLength = 1;
                //1. 
                float contentLength = _scrollableSurface.InnerWidth;
                if (contentLength == 0) return;
                scrollBoxLength = (int)Math.Round((physicalScrollLength * _scrollableSurface.ViewportWidth) / contentLength);
                if (scrollBoxLength < sc.ScrollBoxSizeLimit)
                {
                    scrollBoxLength = sc.ScrollBoxSizeLimit;
                    //viewport ratio
                    onePixelFor = (contentLength - _scrollableSurface.ViewportWidth) / (physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    onePixelFor = contentLength / physicalScrollLength;
                }

                sc.MaxValue = (contentLength > _scrollableSurface.ViewportWidth) ?
                    contentLength - _scrollableSurface.ViewportWidth :
                    0;

            });
            //--------------------------------------------------------------------------------------
            //1st evaluate  
            _slideBox.MaxValue = _scrollableSurface.InnerWidth;
            _slideBox.ReEvaluateScrollBar();

            _scrollableSurface.LayoutFinished += (s, e) =>
            {
                _slideBox.MaxValue = _scrollableSurface.InnerWidth;
                _slideBox.ReEvaluateScrollBar();
            };

            _scrollableSurface.ViewportChanged += (s, e) =>
            {
                if (s != _slideBox)
                {
                    //change value
                    _slideBox.ScrollValue = _scrollableSurface.ViewportX;
                }
            };

            _slideBox.UserScroll += (s, e) =>
            {
                _scrollableSurface.SetViewport((int)_slideBox.ScrollValue, _scrollableSurface.ViewportY, _slideBox);
            };

        }
    }
}