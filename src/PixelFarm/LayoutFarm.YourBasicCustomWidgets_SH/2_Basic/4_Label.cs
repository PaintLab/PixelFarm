//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{

    public class Label : AbstractRectUI
    {
        string _text; //temp text during no text-run
        Color _textColor; //text color
        Color _backColor;//actual filling color


        //some time the label background is transparent
        //but its host has solid color, so this value will hint

        RequestFont _font;
        CustomTextRun _myTextRun;
        TextDrawingTech _textDrawingTech;

        public Label() : this(10, 10)
        {
            //if user does not provide width and height,
            //we use default first, and set HasSpecificWidthAndHeight=false
            //(this feature  found in label, image box, and text-flow-label)
            HasSpecificWidthAndHeight = false; //***
        }
        public Label(int w, int h)
            : base(w, h)
        {
            _textColor = PixelFarm.Drawing.Color.Black;
            TextDrawingTech = TextDrawingTech.Stencil;
        }

#if DEBUG
        public bool dbugBreakOnRenderElement;
#endif
        public override RenderElement GetPrimaryRenderElement()
        {
            if (_myTextRun == null)
            {
                var t_run = new CustomTextRun(this.Width, this.Height);
                t_run.TextDrawingTech = _textDrawingTech;
                t_run.TextColor = _textColor;
                t_run.BackColor = _backColor;
                t_run.PaddingLeft = this.PaddingLeft;
                t_run.PaddingTop = this.PaddingTop;
                t_run.MayOverlapOther = this.MayOverlapOther;
                t_run.SetVisible(this.Visible);
                t_run.DelayFormattedString = _delayTextEvalution;

                if (_font != null)
                {
                    t_run.RequestFont = _font;
                }
                t_run.Text = this.Text;

                t_run.SetLocation(this.Left, this.Top);
                t_run.SetController(this);
                t_run.TransparentForMouseEvents = this.TransparentForMouseEvents;
                //


                //IMPORTANT, place here in last-step
                _myTextRun = t_run;
            }
            //-----------
            return _myTextRun;
        }
        protected override void InvalidatePadding(PaddingName paddingName, ushort newValue)
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
                    //for Label, padding is limit to 0-255
                    _myTextRun.SetPaddings((byte)this.PaddingLeft, (byte)this.PaddingTop, (byte)this.PaddingRight, (byte)this.PaddingBottom);
                    break;
                case PaddingName.AllSideSameValue:
                    //for Label, padding is limit to 0-255
                    _myTextRun.SetPaddings((byte)newValue);
                    break;
            }
        }
        protected override void InvalidateMargin(MarginName marginName, ushort newValue)
        {
            //TODO:...
        }

        public override void SetFont(RequestFont font)
        {
            _font = font;
            if (_myTextRun != null)
            {
                _myTextRun.RequestFont = font;
            }
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _myTextRun;
        //
        public string Text
        {
            get => (_myTextRun != null) ? _myTextRun.Text : _text;
            set
            {
                if (_myTextRun != null)
                {
                    _myTextRun.Text = value;
                }
                else
                {
                    _text = value;
                }
            }
        }
        public TextDrawingTech TextDrawingTech
        {
            get => _textDrawingTech;
            set
            {
                _textDrawingTech = value;
                if (_myTextRun != null)
                {
                    _myTextRun.TextDrawingTech = value;
                }
            }
        }

        bool _delayTextEvalution;
        public bool DelayTextEvalution
        {
            get => _delayTextEvalution;
            set
            {
                _delayTextEvalution = value;
                if (_myTextRun != null)
                {
                    _myTextRun.DelayFormattedString = value;
                }
            }
        }

        bool _mayOverlapOther;
        public bool MayOverlapOther
        {
            get => _mayOverlapOther;
            set
            {
                _mayOverlapOther = value;
                if (_myTextRun != null)
                {
                    _myTextRun.MayOverlapOther = value;
                }
            }
        }


        /// <summary>
        /// text color
        /// </summary>
        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                if (_myTextRun != null && _myTextRun.TextColor != value)
                {
                    _myTextRun.TextColor = value;
                    _myTextRun.InvalidateGraphics();
                }
            }
        }
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (_myTextRun != null && _myTextRun.BackColor != value)
                {
                    _myTextRun.BackColor = value;
                    _myTextRun.InvalidateGraphics();
                }
            }
        }
        //
        public override int InnerWidth => this.Width;
        public override int InnerHeight => this.Height;

    }
}