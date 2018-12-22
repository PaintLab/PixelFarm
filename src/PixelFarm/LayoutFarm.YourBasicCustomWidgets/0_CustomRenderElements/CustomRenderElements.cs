//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomRenderBox : RenderBoxBase
    {
        Color _backColor;
        int _paddingLeft, _paddingTop, _paddingRight, _paddingBottom;
        bool _hasSomePadding;
        bool _evalPadding;//evaluate padding again

        public CustomRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            _evalPadding = true;
            this.BackColor = Color.LightGray;
        }
        public int PaddingLeft
        {
            get => _paddingLeft;
            set
            {
                _paddingLeft = value;
                _evalPadding = true;
            }
        }
        protected void EvalPadding()
        {
            if (_evalPadding)
            {
                _hasSomePadding = _paddingLeft != 0 || _paddingRight != 0 || _paddingBottom != 0 || _paddingTop != 0;
                _evalPadding = false;
            }
        }
        protected bool HasSomePadding => _hasSomePadding;
        public int PaddingTop
        {
            get => _paddingTop;
            set
            {
                _paddingTop = value;
                _evalPadding = true;
            }
        }
        public int PaddingBottom
        {
            get => _paddingBottom;
            set
            {
                _paddingBottom = value;
                _evalPadding = true;
            }
        }
        public int PaddingRight
        {
            get => _paddingRight;
            set
            {
                _paddingRight = value;
                _evalPadding = true;
            }
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