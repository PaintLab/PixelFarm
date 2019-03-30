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
        TextBox _myTextBox;
        MaskTextBox _myMaskTextBox;

        CustomTextRun _placeHolder;
        string _placeHolderText = "";
        bool _multiline;
        TextEditing.TextSurfaceEventListener _textEvListener;
        bool _isMaskTextBox;
        public TextBoxContainer(int w, int h, bool multiline, bool maskTextBox = false)
            : base(w, h)
        {
            this.BackColor = Color.White;
            _multiline = multiline;
            _isMaskTextBox = maskTextBox;
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
                    _myMaskTextBox = new MaskTextBox(this.Width - 4, this.Height - 4);
                    _myMaskTextBox.BackgroundColor = Color.Transparent;
                    _myMaskTextBox.SetLocation(2, 2);
                    _textEvListener = _myMaskTextBox.TextSurfaceEventListener;
                    _textEvListener.KeyDown += new EventHandler<TextEditing.TextDomEventArgs>(textEvListener_KeyDown);
                    baseRenderElement.AddChild(_myMaskTextBox);
                }
                else
                {
                    _myTextBox = new TextBox(this.Width - 4, this.Height - 4, _multiline);
                    _myTextBox.BackgroundColor = Color.Transparent;
                    _myTextBox.SetLocation(0, 0);
                    _textEvListener = new TextEditing.TextSurfaceEventListener();
                    _myTextBox.TextEventListener = _textEvListener;
                    _textEvListener.KeyDown += new EventHandler<TextEditing.TextDomEventArgs>(textEvListener_KeyDown);
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
                bool hasSomeText = _isMaskTextBox ?
                                       _myMaskTextBox.HasSomeText :
                                       _myTextBox.HasSomeText;
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
        public string GetText()
        {
            return _isMaskTextBox ? _myMaskTextBox.Text : _myTextBox.Text;
        }
        public void SetText(string value)
        {
            if (!_isMaskTextBox)
            {
                _myTextBox.Text = value;
            }
        }
        public override void Focus()
        {
            if (!_isMaskTextBox)
            {
                _myTextBox?.Focus();
            }
            else
            {
                _myMaskTextBox?.Focus();
            }

        }
    }



}