//Apache2, 2014-2018, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public delegate void ScrollBarEvaluator(SliderBox scBar, out double onePixelFore, out int scrollBoxHeight);



    struct ScrollRangeLogic
    {
        float maxValue;
        float minValue;
        float largeChange;
        float smallChange;
        float currentValue;


        public ScrollRangeLogic(float min, float max, float largeChange, float smallChange)
        {
            this.maxValue = max;
            this.minValue = min;
            this.currentValue = min;//start at min value
            this.largeChange = largeChange;
            this.smallChange = smallChange;
            //validate values
        }

        public float MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
        public float MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        public float LargeChange
        {
            get { return largeChange; }
            set
            {
                largeChange = value;
            }
        }
        public float SmallChange
        {
            get { return smallChange; }
            set
            {
                smallChange = value;
            }
        }
        public float CurrentValue
        {
            get
            {
                return currentValue;
            }
        }
        public bool SetValue(float value)
        {
            //how to handle over-range value
            //throw ?
            //auto reset?
            float tmpValue = value;
            if (tmpValue < minValue)
            {
                tmpValue = minValue;
            }
            else if (tmpValue > maxValue)
            {
                tmpValue = maxValue;
            }
            //changed?
            bool valueChanged = tmpValue != currentValue;
            this.currentValue = tmpValue;
            return valueChanged;
        }

        public float ValueRange
        {
            get { return maxValue - minValue; }
        }

        public bool SmallStepToMax()
        {
            return SetValue(this.currentValue + smallChange);

        }
        public bool SmallStepToMin()
        {
            return SetValue(this.currentValue - smallChange);
        }
        public bool LargeStepToMax()
        {
            return SetValue(this.currentValue + largeChange);
        }
        public bool LargeStepToMin()
        {
            return SetValue(this.currentValue - largeChange);
        }
    }


    public class SliderBox : UIBox
    {

        ScrollRangeLogic scrollRangeLogic;
        CustomRenderBox mainBox;
        //

        ScrollBarEvaluator customScrollBarEvaluator;
        ScrollBarButton scrollButton;

        double onePixelFor = 1;
        const int SCROLL_BOX_SIZE_LIMIT = 10;
        public SliderBox(int width, int height)
            : base(width, height)
        {

        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.mainBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.mainBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (mainBox == null)
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
            return mainBox;
        }
        public ScrollBarType ScrollBarType
        {
            get;
            set;
        }
        //--------------------------------------------------------------------------


        public int ScrollBoxSizeLimit { get { return SCROLL_BOX_SIZE_LIMIT; } }

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

            scrollRangeLogic.SmallStepToMax();
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

            scrollRangeLogic.SmallStepToMin();
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
            if (scrollButton == null) return;
            switch (this.ScrollBarType)
            {
                default:
                case ScrollBarType.Vertical:
                    {
                        int thumbPosY = CalculateThumbPosition();
                        scrollButton.SetLocation(0, thumbPosY);
                    }
                    break;
                case ScrollBarType.Horizontal:
                    {
                        int thumbPosX = CalculateThumbPosition();
                        scrollButton.SetLocation(thumbPosX, 0);
                    }
                    break;
            }
        }


        //--------------------------------------------------------------------------
        void CreateVScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificSize = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);

            SetupVerticalScrollButtonProperties(bgBox);
            //--------------
            this.mainBox = bgBox;
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificSize = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);

            SetupHorizontalScrollButtonProperties(bgBox);
            //--------------
            this.mainBox = bgBox;
        }

        //----------------------------------------------------------------------- 
        int CalculateThumbPosition()
        {
            return (int)(scrollRangeLogic.CurrentValue / this.onePixelFor);
        }


        public void ReEvaluateScrollBar()
        {
            if (this.scrollButton == null)
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
            this.customScrollBarEvaluator = scrollBarEvaluator;
        }
        void EvaluateVerticalScrollBarProperties()
        {
            int scrollBoxLength = 1;
            //--------------------------
            //if use external evaluator
            if (customScrollBarEvaluator != null)
            {
                customScrollBarEvaluator(this, out this.onePixelFor, out scrollBoxLength);
            }
            else
            {
                //--------------------------
                //calculate scroll length ratio
                //scroll button height is ratio with real scroll length
                float contentLength = scrollRangeLogic.ValueRange;
                //2. 
                float physicalScrollLength = this.Height;
                //3.  
                if (contentLength < physicalScrollLength)
                {
                    int nsteps = (int)Math.Round(contentLength / scrollRangeLogic.SmallChange);
                    //small change value reflect thumbbox size 
                    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                    scrollBoxLength = eachStepLength * 2;
                    this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
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
                        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                    else
                    {
                        //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                        //this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                }
            }
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                throw new NotSupportedException();
            }
            else
            {
                //vertical scrollbar
                this.scrollButton.SetSize(
                    this.scrollButton.Width,
                    scrollBoxLength);
                this.InvalidateOuterGraphics();
            }
        }
        void SetupVerticalScrollButtonProperties(RenderElement container)
        {
            //var scroll_button = new ScrollBarButton(this.Width+10, SCROLL_BOX_SIZE_LIMIT, this); //for test
            var scroll_button = new ScrollBarButton(this.Width, SCROLL_BOX_SIZE_LIMIT, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosY = CalculateThumbPosition();
            scroll_button.SetLocation(0, thumbPosY);
            container.AddChild(scroll_button);
            this.scrollButton = scroll_button;
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
                if (newYPos >= this.Height - (scrollButton.Height))
                {
                    newYPos = this.Height - (scrollButton.Height);
                }
                else if (newYPos < 0)
                {
                    newYPos = 0;
                }

                //calculate value from position 

                int currentMarkAt = newYPos;
                scrollRangeLogic.SetValue((float)(onePixelFor * currentMarkAt));
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
            if (customScrollBarEvaluator != null)
            {
                customScrollBarEvaluator(this, out this.onePixelFor, out scrollBoxLength);
            }
            else
            {
                //calculate scroll length ratio
                //scroll button height is ratio with real scroll length
                float contentLength = scrollRangeLogic.ValueRange;
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
                    int nsteps = (int)Math.Round(contentLength / scrollRangeLogic.SmallChange);
                    //small change value reflect thumbbox size 
                    int eachStepLength = (int)(physicalScrollLength / (float)(nsteps + 2));
                    scrollBoxLength = eachStepLength * 2;
                    this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
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
                        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                    else
                    {
                        //float physicalSmallEach = (physicalScrollLength / contentLength) * smallChange;
                        //this.onePixelFor = contentLength / (physicalScrollLength - physicalSmallEach);
                        this.onePixelFor = contentLength / (physicalScrollLength - scrollBoxLength);
                    }
                }
            }
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                this.scrollButton.SetSize(
                    scrollBoxLength,
                    this.scrollButton.Height);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        void SetupHorizontalScrollButtonProperties(RenderElement container)
        {
            //TODO: use 'theme-concept' eg. css

            var scroll_button = new ScrollBarButton(SCROLL_BOX_SIZE_LIMIT, this.Height, this); //create with default value
            scroll_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkBlue);
            int thumbPosX = CalculateThumbPosition();
            scroll_button.SetLocation(thumbPosX, 0);
            container.AddChild(scroll_button);
            this.scrollButton = scroll_button;
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
                if (newXPos >= this.Width - (scrollButton.Width))
                {
                    newXPos = this.Width - (scrollButton.Width);
                }
                else if (newXPos < 0)
                {
                    newXPos = 0;
                }

                //calculate value from position 

                int currentMarkAt = newXPos;
                this.scrollRangeLogic.SetValue((float)(onePixelFor * currentMarkAt));

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
        public void SetupScrollBar(ScrollBarCreationParameters creationParameters)
        {
            this.MaxValue = creationParameters.maximum;
            this.MinValue = creationParameters.minmum;
        }
        public float MaxValue
        {
            get { return this.scrollRangeLogic.MaxValue; }
            set
            {
                this.scrollRangeLogic.MaxValue = value;
                //need update 
            }
        }
        public float MinValue
        {
            get { return this.scrollRangeLogic.MinValue; }
            set
            {
                this.scrollRangeLogic.MinValue = value;
                //need update 
            }
        }
        public float SmallChange
        {
            get { return scrollRangeLogic.SmallChange; }
            set
            {
                scrollRangeLogic.SmallChange = value;
                //need update 
            }
        }
        public float LargeChange
        {
            get { return scrollRangeLogic.LargeChange; }
            set
            {
                scrollRangeLogic.LargeChange = value;
                //need update 
            }
        }
        public float ScrollValue
        {
            get { return scrollRangeLogic.CurrentValue; }
            set
            {
                scrollRangeLogic.SetValue(value);
                //need update 
                ReEvaluateScrollBar();
                UpdateScrollButtonPosition();
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
    public class ScrollBar : UIBox
    {
        ScrollBarButton minButton;
        ScrollBarButton maxButton;
        SliderBox slideBox;

        CustomRenderBox mainBox;
        protected int minmax_boxHeight = 15;

        public ScrollBar(int width, int height)
            : base(width, height)
        {

            slideBox = new SliderBox(minmax_boxHeight, minmax_boxHeight);

        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.mainBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.mainBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (mainBox == null)
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
            return mainBox;
        }
        public ScrollBarType ScrollBarType
        {
            get { return slideBox.ScrollBarType; }
            set { slideBox.ScrollBarType = value; }
        }
        //--------------------------------------------------------------------------

        public SliderBox SliderBox { get { return slideBox; } }
        public int MinMaxButtonHeight { get { return minmax_boxHeight; } }
        public int ScrollBoxSizeLimit { get { return slideBox.ScrollBoxSizeLimit; } }

        public int PhysicalScrollLength
        {
            get
            {
                if (ScrollBarType == ScrollBarType.Vertical)
                {
                    return this.Height - (this.minmax_boxHeight + this.minmax_boxHeight);
                }
                else
                {
                    return this.Width - (this.minmax_boxHeight + this.minmax_boxHeight);
                }
            }
        }



        public void StepSmallToMax()
        {
            slideBox.StepSmallToMax();
        }
        public void StepSmallToMin()
        {
            slideBox.StepSmallToMin();
        }

        //--------------------------------------------------------------------------
        void CreateVScrollbarContent(RootGraphic rootgfx)
        {
            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificSize = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);
            //---------------------------------------------------------
            slideBox.ScrollBarType = ScrollBarType.Vertical;
            slideBox.SetLocation(0, minmax_boxHeight);
            slideBox.SetSize(this.Width, this.Height - minmax_boxHeight * 2);

            RenderElement sliderRenderE = slideBox.GetPrimaryRenderElement(rootgfx);
            bgBox.AddChild(sliderRenderE);

            //MinButton
            SetupMinButtonProperties(bgBox);
            //MaxButton
            SetupMaxButtonProperties(bgBox);
            //ScrollButton
            SetupVerticalScrollButtonProperties(bgBox);
            //--------------
            this.mainBox = bgBox;
        }
        void CreateHScrollbarContent(RootGraphic rootgfx)
        {


            CustomRenderBox bgBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
            bgBox.HasSpecificSize = true;
            bgBox.SetController(this);
            bgBox.SetLocation(this.Left, this.Top);
            //---------------------------------------------------------
            slideBox.ScrollBarType = ScrollBarType.Horizontal;
            slideBox.SetLocation(minmax_boxHeight, 0);
            slideBox.SetSize(this.Width - minmax_boxHeight * 2, this.Height);
            RenderElement sliderRenderE = slideBox.GetPrimaryRenderElement(rootgfx);

            bgBox.AddChild(sliderRenderE);

            //MinButton
            SetupMinButtonProperties(bgBox);
            //MaxButton
            SetupMaxButtonProperties(bgBox);
            //ScrollButton
            SetupHorizontalScrollButtonProperties(bgBox);
            //--------------
            this.mainBox = bgBox;
        }

        void SetupMinButtonProperties(RenderElement container)
        {
            ScrollBarButton min_button;
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                min_button = new ScrollBarButton(minmax_boxHeight, this.Height, this);
            }
            else
            {
                min_button = new ScrollBarButton(this.Width, minmax_boxHeight, this);
            }
            min_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            min_button.MouseUp += (s, e) => this.StepSmallToMin();
            container.AddChild(min_button);
            this.minButton = min_button;
        }
        void SetupMaxButtonProperties(RenderElement container)
        {
            ScrollBarButton max_button;
            if (this.ScrollBarType == ScrollBarType.Horizontal)
            {
                max_button = new ScrollBarButton(minmax_boxHeight, this.Height, this);
                max_button.SetLocation(this.Width - minmax_boxHeight, 0);
            }
            else
            {
                max_button = new ScrollBarButton(this.Width, minmax_boxHeight, this);
                max_button.SetLocation(0, this.Height - minmax_boxHeight);
            }


            max_button.BackColor = KnownColors.FromKnownColor(KnownColor.DarkGray);
            max_button.MouseUp += (s, e) => this.StepSmallToMax();
            container.AddChild(max_button);
            this.maxButton = max_button;
        }

        //---------------------------------------------------------------------------
        //vertical scrollbar

        public void ReEvaluateScrollBar()
        {
            slideBox.ReEvaluateScrollBar();
        }
        public void SetCustomScrollBarEvaluator(ScrollBarEvaluator scrollBarEvaluator)
        {
            slideBox.SetCustomScrollBarEvaluator(scrollBarEvaluator);
        }

        void SetupVerticalScrollButtonProperties(RenderElement container)
        {
        }

        //---------------------------------------------------------------------------
        //horizontal scrollbar
        void EvaluateHorizontalScrollBarProperties()
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
            get { return slideBox.MaxValue; }
            set
            {
                slideBox.MaxValue = value;
                //need update 
            }
        }
        public float MinValue
        {
            get { return slideBox.MinValue; }
            set
            {
                slideBox.MinValue = value;
                //need update 
            }
        }
        public float SmallChange
        {
            get { return slideBox.SmallChange; }
            set
            {
                slideBox.SmallChange = value;
                //need update 
            }
        }
        public float LargeChange
        {
            get { return slideBox.LargeChange; }
            set
            {
                slideBox.LargeChange = value;
                //need update 
            }
        }
        public float ScrollValue
        {
            get { return slideBox.ScrollValue; }
            set
            {
                slideBox.ScrollValue = value;
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




    class ScrollBarButton : EaseBox
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




    public class ScrollingRelation
    {
        SliderBox scBar;
        IScrollable scrollableSurface;
        public ScrollingRelation(SliderBox scBar, IScrollable scrollableSurface)
        {
            this.scBar = scBar;
            this.scrollableSurface = scrollableSurface;
            switch (scBar.ScrollBarType)
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
            this.scBar.SetCustomScrollBarEvaluator((SliderBox sc, out double onePixelFor, out int scrollBoxLength) =>
            {
                int physicalScrollLength = sc.PhysicalScrollLength;
                onePixelFor = 1;
                scrollBoxLength = 1;
                //1. 
                int contentLength = scrollableSurface.DesiredHeight;
                if (contentLength == 0)
                {
                    return;
                }
                scrollBoxLength = (int)((physicalScrollLength * scrollableSurface.ViewportHeight) / contentLength);
                if (scrollBoxLength < sc.ScrollBoxSizeLimit)
                {
                    scrollBoxLength = sc.ScrollBoxSizeLimit;
                    onePixelFor = (double)contentLength / (double)(physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    onePixelFor = (double)contentLength / (double)physicalScrollLength;
                }

                //temp fix 
                sc.MaxValue = (contentLength > scrollableSurface.ViewportHeight) ?
                    contentLength - scrollableSurface.ViewportHeight :
                    0;
            });
            //--------------------------------------------------------------------------------------
            //1st evaluate  
            scBar.MaxValue = scrollableSurface.DesiredHeight;
            scBar.ReEvaluateScrollBar();
            scrollableSurface.LayoutFinished += (s, e) =>
            {
                scBar.MaxValue = scrollableSurface.DesiredHeight;
                scBar.ReEvaluateScrollBar();
            };
            scBar.UserScroll += (s, e) =>
            {
                scrollableSurface.SetViewport(scrollableSurface.ViewportX, (int)scBar.ScrollValue);
            };
        }
        void SetupHorizontalScrollRelation()
        {
            this.scBar.SetCustomScrollBarEvaluator((SliderBox sc, out double onePixelFor, out int scrollBoxLength) =>
            {
                //horizontal scroll bar
                int physicalScrollLength = sc.PhysicalScrollLength;
                onePixelFor = 1;
                scrollBoxLength = 1;
                //1. 
                int contentLength = scrollableSurface.DesiredWidth;
                if (contentLength == 0) return;
                scrollBoxLength = (int)((physicalScrollLength * scrollableSurface.ViewportWidth) / contentLength);
                if (scrollBoxLength < sc.ScrollBoxSizeLimit)
                {
                    scrollBoxLength = sc.ScrollBoxSizeLimit;
                    onePixelFor = (double)contentLength / (double)(physicalScrollLength - scrollBoxLength);
                }
                else
                {
                    onePixelFor = (double)contentLength / (double)physicalScrollLength;
                }
                //sc.MaxValue = contentLength - scrollableSurface.ViewportWidth;
                sc.MaxValue = (contentLength > scrollableSurface.ViewportWidth) ?
                    contentLength - scrollableSurface.ViewportWidth :
                    0;
            });
            //--------------------------------------------------------------------------------------
            //1st evaluate  
            scBar.MaxValue = scrollableSurface.DesiredWidth;
            scBar.ReEvaluateScrollBar();
            scrollableSurface.LayoutFinished += (s, e) =>
            {
                scBar.MaxValue = scrollableSurface.DesiredWidth;
                scBar.ReEvaluateScrollBar();
            };
            scBar.UserScroll += (s, e) =>
            {
                scrollableSurface.SetViewport((int)scBar.ScrollValue, scrollableSurface.ViewportY);
            };
        }
    }
}