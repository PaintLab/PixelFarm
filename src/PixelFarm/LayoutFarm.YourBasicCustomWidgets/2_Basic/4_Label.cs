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
        }
        
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (this._myTextRun == null)
            {
                var trun = new CustomTextRun(rootgfx, this.Width, this.Height);
                trun.SetLocation(this.Left, this.Top);
                trun.TextColor = this._textColor;
                trun.Text = this.Text;
                //
                trun.SetController(this);
                //
                if (_font != null)
                {
                    trun.RequestFont = _font;
                }
                this._myTextRun = trun;
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
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this._myTextRun; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this._myTextRun != null; }
        }
        public string Text
        {
            get { return this._text; }
            set
            {
                this._text = value;
                if (this._myTextRun != null)
                {
                    this._myTextRun.Text = value;
                }
            }
        }
        public Color Color
        {
            get { return this._textColor; }
            set
            {
                this._textColor = value;
                if (_myTextRun != null)
                {
                    _myTextRun.TextColor = value;
                }
            }
        }
        public override int InnerHeight
        {
            get
            {
                return this.Height;
            }
        }
        public override int InnerWidth
        {
            get
            {
                return this.Width;
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "label");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}