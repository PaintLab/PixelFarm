//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    /// <summary>
    /// textbox with decoration(eg. placeholder)
    /// </summary>
    public class TextBoxContainer : AbstractBox
    { 
        TextBoxBase _myTextBox;
        bool _isMaskTextBox; 
        CustomTextRun _placeHolder;
        string _placeHolderText = "";
        bool _multiline;
        TextEditing.TextSurfaceEventListener _textEvListener;

        public TextBoxContainer(int w, int h, bool multiline, bool maskTextBox = false)
            : base(w, h)
        {
            this.BackColor = Color.White;
            _multiline = multiline;
            _isMaskTextBox = maskTextBox;

            //NOTE: this version, maskTextBox=> not support multiline
        }
        public string PlaceHolderText
        {
            get => _placeHolderText;
            set
            {
                _placeHolderText = value;
                if (_placeHolder != null)
                {
                    _placeHolder.Text = _placeHolderText;
                    this.InvalidateGraphics();
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                //first time
                RenderElement baseRenderElement = base.GetPrimaryRenderElement(rootgfx);
                //1. add place holder first
                _placeHolder = new CustomTextRun(rootgfx, this.Width - 4, this.Height - 4);
                _placeHolder.Text = _placeHolderText;
                _placeHolder.SetLocation(1, 1);
                _placeHolder.TextColor = Color.FromArgb(180, Color.LightGray);
                baseRenderElement.AddChild(_placeHolder);
                //2. textbox 
                if (_isMaskTextBox)
                {
                    _myTextBox = new MaskTextBox(this.Width - 4, this.Height - 4);
                    _myTextBox.BackgroundColor = Color.Transparent;
                    _myTextBox.SetLocation(2, 2);
                    _textEvListener = _myTextBox.TextSurfaceEventListener;
                    _textEvListener.KeyDown += textEvListener_KeyDown;
                    baseRenderElement.AddChild(_myTextBox);
                }
                else
                {
                    _myTextBox = new TextBox(this.Width - 4, this.Height - 4, _multiline);
                    _myTextBox.BackgroundColor = Color.Transparent;
                    _myTextBox.SetLocation(0, 0);
                    _textEvListener = new TextEditing.TextSurfaceEventListener();
                    _myTextBox.TextEventListener = _textEvListener;
                    _textEvListener.KeyDown += textEvListener_KeyDown;
                    baseRenderElement.AddChild(_myTextBox);
                }

                return baseRenderElement;
            }
            else
            {
                return base.GetPrimaryRenderElement(rootgfx);
            }
        }
        void textEvListener_KeyDown(object sender, TextEditing.TextDomEventArgs e)
        {

            if (!string.IsNullOrEmpty(_placeHolderText))
            {
                bool hasSomeText = _myTextBox.HasSomeText;

                if (hasSomeText)
                {
                    //hide place holder                     
                    if (_placeHolder.Visible)
                    {
                        _placeHolder.SetVisible(false);
                        _placeHolder.InvalidateGraphics();
                    }
                }
                else
                {
                    //show place holder
                    if (!_placeHolder.Visible)
                    {
                        _placeHolder.SetVisible(true);
                        _placeHolder.InvalidateGraphics();
                    }
                }
            }

            ((IEventListener)this).ListenKeyDown(e.OriginalKey);

        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "textbox_container");
            this.Describe(visitor);
            visitor.EndElement();
        }

        public string GetText() => _myTextBox.Text;

        public void SetText(string value) => _myTextBox.Text = value;
        public override void Focus() => _myTextBox?.Focus();
    } 

}