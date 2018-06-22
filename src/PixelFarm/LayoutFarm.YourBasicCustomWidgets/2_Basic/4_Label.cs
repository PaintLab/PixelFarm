//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class Label : AbstractRect
    {
        string text;
        Color textColor;
        CustomTextRun myTextRun;
        RequestFont _font;
        public Label(int w, int h)
            : base(w, h)
        {
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (this.myTextRun == null)
            {
                var trun = new CustomTextRun(rootgfx, this.Width, this.Height);
                trun.SetLocation(this.Left, this.Top);
                trun.TextColor = this.textColor;
                trun.Text = this.Text;
                if(_font != null)
                {
                    trun.RequestFont = _font;
                }
                this.myTextRun = trun;
            }
            //-----------
            return myTextRun;
        }
        public override void SetFont(RequestFont font)
        {
            if (myTextRun != null)
            {
                myTextRun.RequestFont = font;
            }
            else
            {
                _font = font;
            }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.myTextRun; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.myTextRun != null; }
        }
        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                if (this.myTextRun != null)
                {
                    this.myTextRun.Text = value;
                }
            }
        }
        public Color Color
        {
            get { return this.textColor; }
            set
            {
                this.textColor = value;
                if (myTextRun != null)
                {
                    myTextRun.TextColor = value;
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