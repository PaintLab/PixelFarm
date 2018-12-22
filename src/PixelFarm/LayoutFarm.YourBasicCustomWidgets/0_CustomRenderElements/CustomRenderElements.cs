//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomRenderBox : RenderBoxBase
    {
        Color _backColor;

        //these are NOT CSS borders/margins/paddings***
        //we use pixel unit for our RenderBox
        //with limitation of int8 number

        byte _contentLeft;
        byte _contentTop;
        byte _contentRight;
        byte _contentBottom;

        byte _borderLeft;
        byte _borderTop;
        byte _borderRight;
        byte _borderBottom;

        public CustomRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
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
        public void SetPadding(byte left, byte top, byte right, byte bottom)
        {
            _contentLeft = (byte)(left + _borderLeft);
            _contentTop = (byte)(top + _borderTop);
            _contentRight = (byte)(right + _borderRight);
            _contentBottom = (byte)(bottom + _borderBottom);
        }
        public void SetPadding(byte sameValue)
        {
            _contentLeft = (byte)(sameValue + _borderLeft);
            _contentTop = (byte)(sameValue + _borderTop);
            _contentRight = (byte)(sameValue + _borderRight);
            _contentBottom = (byte)(sameValue + _borderBottom);
        }
        //------------------ 
        public int BorderTop
        {
            get => _borderTop;
            set => _borderTop = (byte)value;

        }
        public int BorderBottom
        {
            get => _borderBottom;
            set => _borderBottom = (byte)value;
        }
        public int BorderRight
        {
            get => _borderRight;
            set => _borderRight = (byte)value;
        }
        public int BorderLeft
        {
            get => _borderLeft;
            set => _borderLeft = (byte)value;

        }
        public void SetBorders(byte left, byte top, byte right, byte bottom)
        {
            _borderLeft = left;
            _borderTop = top;
            _borderRight = right;
            _borderBottom = bottom;
        }
        public void SetBorders(byte sameValue)
        {
            _borderLeft =
                _borderTop =
                _borderRight =
                _borderBottom = sameValue;
        }
        //-------------

        public int ContentWidth => Width - (_contentLeft + _contentRight);
        public int ContentHeight => Height - (_contentTop + _contentBottom);

        public int ContentLeft
        {
            get => _contentLeft;
            set => _contentLeft = (byte)value;
        }
        public int ContentTop
        {
            get => _contentTop;
            set => _contentTop = (byte)value;
        }
        public int ContentRight
        {
            get => _contentRight;
            set => _contentRight = (byte)value;
        }
        public int ContentBottom
        {
            get => _contentBottom;
            set => _contentBottom = (byte)value;
        }
        public void SetContentOffsets(byte contentLeft, byte contentTop, byte contentRight, byte contentBottom)
        {
            _contentLeft = contentLeft;
            _contentTop = contentTop;
            _contentRight = contentRight;
            _contentBottom = contentBottom;
        }
        public void SetContentOffsets(byte allside)
        {
            _contentLeft = allside;
            _contentTop = allside;
            _contentRight = allside;
            _contentBottom = allside;
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