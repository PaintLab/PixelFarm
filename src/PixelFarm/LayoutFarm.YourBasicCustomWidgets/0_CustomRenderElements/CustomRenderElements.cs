//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomRenderBox : RenderBoxBase
    {
        Color _backColor;

        int _paddingLeft, _paddingTop, _paddingRight, _paddingBottom;
        int _borderLeft, _borderTop, _borderRight, _borderBottom;

        bool _hasSomePadding;
        bool _needEvalPaddingAgain;//evaluate padding again

        public CustomRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            _needEvalPaddingAgain = true;
            this.BackColor = Color.LightGray;
        }

        protected void EvalPadding()
        {
            if (_needEvalPaddingAgain)
            {
                _hasSomePadding = _paddingLeft != 0 || _paddingRight != 0 || _paddingBottom != 0 || _paddingTop != 0;
                _needEvalPaddingAgain = false;
            }
        }

        protected bool HasSomePadding => _hasSomePadding;
        public int PaddingTop
        {
            get => _paddingTop;
            set
            {
                _paddingTop = value;
                _needEvalPaddingAgain = true;
            }
        }
        public int PaddingBottom
        {
            get => _paddingBottom;
            set
            {
                _paddingBottom = value;
                _needEvalPaddingAgain = true;
            }
        }
        public int PaddingRight
        {
            get => _paddingRight;
            set
            {
                _paddingRight = value;
                _needEvalPaddingAgain = true;
            }
        }
        public int PaddingLeft
        {
            get => _paddingLeft;
            set
            {
                _paddingLeft = value;
                _needEvalPaddingAgain = true;
            }
        }

        public void SetPadding(int left, int top, int right, int bottom)
        {
            _paddingLeft = left;
            _paddingTop = top;
            _paddingRight = right;
            _paddingBottom = bottom;

            _needEvalPaddingAgain = true;
        }
        public void SetPadding(int sameValue)
        {
            _paddingLeft =
                _paddingTop =
                _paddingRight =
                _paddingBottom = sameValue;

            _needEvalPaddingAgain = true;
        }
        //------------------
        public int BorderTop
        {
            get => _borderTop;
            set
            {
                _borderTop = value;
            }
        }
        public int BorderBottom
        {
            get => _borderBottom;
            set
            {
                _borderBottom = value;
            }
        }
        public int BorderRight
        {
            get => _borderRight;
            set
            {
                _borderRight = value;
            }
        }
        public int BorderLeft
        {
            get => _borderLeft;
            set
            {
                _borderLeft = value;
            }
        }
        public void SetBorders(int left, int top, int right, int bottom)
        {
            _borderLeft = left;
            _borderTop = top;
            _borderRight = right;
            _borderBottom = bottom;
        }
        public void SetBorders(int sameValue)
        {
            _borderLeft =
                _borderTop =
                _borderRight =
                _borderBottom = sameValue;
        }


        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (this.HasParentLink)
                {
                    this.InvalidateGraphics();
                }
            }
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
#if DEBUG
            if (this.dbugBreak)
            {
            }
#endif

            if (this.MayHasViewport)
            {
                //TODO: review here
                //start pos of background fill
                //(0,0) 
                //(viewportX,viewportY)
                //tile or limit
                canvas.FillRectangle(BackColor, ViewportX, ViewportY, this.Width, this.Height);
            }
            else
            {
                canvas.FillRectangle(BackColor, 0, 0, this.Width, this.Height);
            }

            this.DrawDefaultLayer(canvas, ref updateArea);
#if DEBUG
            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));

            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //   new Rectangle(updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height));
#endif
        }
    }
}