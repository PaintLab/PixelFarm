//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class Label : AbstractRectUI
    {
        string _text;
        Color _textColor;
        Color _backColor;
        CustomTextRun _myTextRun;
        RequestFont _font;
        //
        public Label(int w, int h)
            : base(w, h)
        {
            _textColor = PixelFarm.Drawing.Color.Black; //default?, use Theme?
        }

#if DEBUG
        public bool dbugBreakOnRenderElement;
#endif
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_myTextRun == null)
            {
                var trun = new CustomTextRun(rootgfx, this.Width, this.Height);

#if DEBUG
                trun.dbugBreak = this.dbugBreakOnRenderElement;

#endif
                trun.SetLocation(this.Left, this.Top);
                trun.TextColor = _textColor;
                trun.BackColor = _backColor;
                trun.Text = this.Text;
                trun.PaddingLeft = this.PaddingLeft;
                trun.PaddingTop = this.PaddingTop;
                trun.SetVisible(this.Visible);
                trun.SetController(this);
                //
                if (_font != null)
                {
                    trun.RequestFont = _font;
                }
                _myTextRun = trun;
            }
            //-----------
            return _myTextRun;
        }
        protected override void InvalidatePadding(PaddingName paddingName, byte newValue)
        {
            if (_myTextRun == null) return;
            //
            switch (paddingName)
            {
                case PaddingName.Left:
                    _myTextRun.PaddingLeft = newValue;
                    break;
                case PaddingName.Top:
                    _myTextRun.PaddingTop = newValue;
                    break;
                case PaddingName.Right:
                    _myTextRun.PaddingRight = newValue;
                    break;
                case PaddingName.Bottom:
                    _myTextRun.PaddingBottom = newValue;
                    break;
                case PaddingName.AllSide:
                    _myTextRun.SetPaddings(this.PaddingLeft, this.PaddingTop, this.PaddingRight, this.PaddingBottom);
                    break;
                case PaddingName.AllSideSameValue:
                    _myTextRun.SetPaddings(newValue);
                    break;
            }
        }
        protected override void InvalidateMargin(MarginName marginName, short newValue)
        {
            //TODO:...
        }

        public override void SetFont(RequestFont font)
        {
            if (_myTextRun != null)
            {
                _myTextRun.RequestFont = font;
            }
            else
            {
                _font = font;
            }
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _myTextRun;
        //
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                if (_myTextRun != null)
                {
                    _myTextRun.Text = value;
                }
            }
        }
        /// <summary>
        /// text color
        /// </summary>
        public Color Color
        {
            get => _textColor;
            set
            {
                _textColor = value;
                if (_myTextRun != null)
                {
                    _myTextRun.TextColor = value;
                }
            }
        }
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (_myTextRun != null)
                {
                    _myTextRun.BackColor = value;
                }
            }
        }
        //
        public override int InnerWidth => this.Width;
        public override int InnerHeight => this.Height;

    }
}