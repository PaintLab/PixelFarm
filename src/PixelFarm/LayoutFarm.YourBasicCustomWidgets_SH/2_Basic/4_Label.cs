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
        RequestFont _font;

        CustomTextRun _myTextRun;
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
                var t_run = new CustomTextRun(rootgfx, this.Width, this.Height);

#if DEBUG
                t_run.dbugBreak = this.dbugBreakOnRenderElement;

#endif
                t_run.SetLocation(this.Left, this.Top);
                t_run.TextColor = _textColor;
                t_run.BackColor = _backColor;
                t_run.Text = this.Text;
                t_run.PaddingLeft = this.PaddingLeft;
                t_run.PaddingTop = this.PaddingTop;
                t_run.SetVisible(this.Visible);
                t_run.SetController(this);
                //
                if (_font != null)
                {
                    t_run.RequestFont = _font;
                }
                _myTextRun = t_run;
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
                    _myTextRun.InvalidateGraphics();
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