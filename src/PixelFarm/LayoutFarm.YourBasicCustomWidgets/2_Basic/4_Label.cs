//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class Label : AbstractRectUI
    {
        string _text;
        Color _textColor;
        CustomTextRun _myTextRun;
        RequestFont _font;
        //
        public Label(int w, int h)
            : base(w, h)
        {
            _textColor = PixelFarm.Drawing.Color.Black;
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_myTextRun == null)
            {
                var trun = new CustomTextRun(rootgfx, this.Width, this.Height);
                trun.SetLocation(this.Left, this.Top);
                trun.TextColor = _textColor;
                trun.Text = this.Text;
                //
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
        protected override bool HasReadyRenderElement => _myTextRun != null;
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
        //
        public override int InnerWidth => this.Width;
        public override int InnerHeight => this.Height;
        //

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "label");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}