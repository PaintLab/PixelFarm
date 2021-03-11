//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{

    public class TextBoxContainer : AbstractControlBox
    {

        TextBoxSwitcher _textboxSwitcher;
        TextBoxBase _myTextBox;
        bool _isMaskTextBox;
        CustomTextRun _placeHolder;
        CustomTextRun _textBoxPlaceHolder; //this version we use with single line text only

        string _placeHolderText = "";
        bool _isMultiLine;
        bool _isEditable;
        LayoutFarm.TextFlow.TextSurfaceEventListener _textSurfaceEventListener;
        public event EventHandler<UIKeyEventArgs> KeyDown;
        public event EventHandler<UIKeyEventArgs> KeyUp;
        public TextBoxContainer(int w, int h, bool multiline, bool maskTextBox = false)
            : base(w, h)
        {
            this.BackColor = Color.White;
            _isMultiLine = multiline;
            _isMaskTextBox = maskTextBox;
            _isEditable = true;

            //NOTE: this version, maskTextBox=> not support multiline
        }
        protected override void OnKeyDown(UIKeyEventArgs e) => KeyDown?.Invoke(this, e);
        protected override void OnKeyUp(UIKeyEventArgs e) => KeyUp?.Invoke(this, e);

        public bool IsMultilineTextBox => _isMultiLine;
        public bool IsEditable
        {
            get => _isEditable;
            set
            {
                _isEditable = value;
            }
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
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            base.OnMouseDown(e);
            //when mousedown on control
            //then do focus()
            //evaluate before Focus()
            bool needTextBoxSwitching = _textboxSwitcher != null && _myTextBox == null;

            Focus();

            if (needTextBoxSwitching)
            {
                ((IEventListener)_myTextBox).ListenMouseDown(e);
            }

        }
        public override RenderElement GetPrimaryRenderElement()
        {
            if (!this.HasReadyRenderElement)
            {
                //first time

                RenderElement renderE = base.GetPrimaryRenderElement();
                if (renderE is IContainerRenderElement baseRenderElement)
                {
                    //1. add place holder first
                    _placeHolder = new CustomTextRun(this.Width - 4, this.Height - 4);
                    _placeHolder.Text = _placeHolderText;
                    _placeHolder.SetLocation(1, 1);
                    _placeHolder.TextColor = Color.FromArgb(180, KnownColors.LightGray);
                    baseRenderElement.AddChild(_placeHolder);

                    _textSurfaceEventListener = new LayoutFarm.TextFlow.TextSurfaceEventListener();
                    _textSurfaceEventListener.KeyDown += textEvListener_KeyDown;

                    TextBoxSwitcher textboxSwitcher = this.TextBoxSwitcher;
                    if (textboxSwitcher != null)
                    {
                        //has textbox switcher
                        //use special 'light-weight' textbox
                        _textBoxPlaceHolder = new CustomTextRun(this.Width - 4, this.Height - 4);
                        baseRenderElement.AddChild(_textBoxPlaceHolder);

                        if (_userText != null)
                        {
                            _textBoxPlaceHolder.Text = _userText;
                            _userText = null;
                        }
                    }
                    else
                    {
                        //no switcher 
                        //so use actual text box or mask textbox 
                        //2. textbox  
                        _myTextBox = _isMaskTextBox ?
                                        (TextBoxBase)(new MaskTextBox(this.Width - 4, this.Height - 4)) :
                                        new TextBox(this.Width - 4, this.Height - 4, _isMultiLine);
                        _myTextBox.BackgroundColor = Color.Transparent;
                        _myTextBox.TextEventListener = _textSurfaceEventListener;
                        baseRenderElement.AddChild(_myTextBox);

                        if (_userText != null)
                        {
                            _myTextBox.Text = _userText;
                            _userText = null;
                        }
                    }
                }
                return renderE;
            }
            else
            {
                return base.GetPrimaryRenderElement();
            }
        }
        void textEvListener_KeyDown(object sender, LayoutFarm.TextFlow.TextDomEventArgs e)
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

            //-------------------
            if (e.OriginalKeyEventArgs != null)
            {
                ((IEventListener)this).ListenKeyDown(e.OriginalKeyEventArgs);
            }
        }


        string _userText;

        public string GetText()
        {
            if (_myTextBox != null)
            {
                return _myTextBox.Text;
            }
            else if (_textBoxPlaceHolder != null)
            {
                return _textBoxPlaceHolder.Text;
            }
            else
            {
                return _userText;
            }
        }
        public void SetText(string value)
        {
            if (_myTextBox != null)
            {
                _myTextBox.Text = value;
            }
            else if (_textBoxPlaceHolder != null)
            {
                _textBoxPlaceHolder.Text = value;
            }
            else
            {
                _userText = value;
            }
        }
         
        public override void Focus()
        {
            if (_textboxSwitcher != null)
            {
                //on focus              
                if (_isEditable && _myTextBox == null)
                {
                    if (_textboxSwitcher.UsedBy != null)
                    {
                        _textboxSwitcher.UsedBy.ReleaseSwitchableTextBox();
                        _textboxSwitcher.UsedBy = null;
                    }

                    if (_isMaskTextBox)
                    {
                        _myTextBox = _textboxSwitcher.BorrowMaskTextBox(this.Width - 4, this.Height - 4);

                    }
                    else
                    {
                        _myTextBox = _textboxSwitcher.BorrowTextBox(this.Width - 4, this.Height - 4);
                        _myTextBox.TextEventListener = _textSurfaceEventListener;
                    }


                    if (base.GetPrimaryRenderElement() is IContainerRenderElement baseRenderE)
                    {
                        baseRenderE.AddChild(_myTextBox);
                    }

                    _textBoxPlaceHolder.SetVisible(false);
                    _myTextBox.Text = _textBoxPlaceHolder.Text;
                    _myTextBox.Focus();

                    _textboxSwitcher.UsedBy = this;
                }
            }
            else
            {
                _myTextBox?.Focus();
            }
        }

        protected override void OnLostKeyboardFocus(UIFocusEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
        }
        protected override void OnLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            base.OnLostMouseFocus(e);
        }
        //
        public TextBoxSwitcher TextBoxSwitcher
        {
            get => _textboxSwitcher;
            set => _textboxSwitcher = value;
        }

        void ReleaseSwitchableTextBox()
        {
            //copy text
            _textBoxPlaceHolder.Text = _myTextBox.Text;
            _myTextBox.TextEventListener = null;


            if (base.GetPrimaryRenderElement() is IContainerRenderElement baseRenderElement)
            {
                baseRenderElement.RemoveChild(_myTextBox.CurrentPrimaryRenderElement);
            }

            _myTextBox.Text = "";//clear 

            if (_isMaskTextBox)
            {
                _textboxSwitcher.ReleaseMaskTextBox((MaskTextBox)_myTextBox);
            }
            else
            {
                _textboxSwitcher.ReleaseTextBox((TextBox)_myTextBox);
            }

            _myTextBox = null;
            _textBoxPlaceHolder.SetVisible(true);

        }
    }

    public class TextBoxSwitcher
    {
        Stack<TextBox> _textBoxPool = new Stack<TextBox>();
        Stack<MaskTextBox> _maskTextBoxPool = new Stack<MaskTextBox>();
        TextBoxContainer _usedBy;

        public MaskTextBox BorrowMaskTextBox(int w, int h)
        {
            if (_maskTextBoxPool.Count == 0)
            {
                MaskTextBox maskTextBox = new MaskTextBox(w, h);
                return maskTextBox;
            }
            else
            {
                MaskTextBox maskTextBox = _maskTextBoxPool.Pop();
                maskTextBox.SetSize(w, h);
                return maskTextBox;
            }
        }
        public TextBox BorrowTextBox(int w, int h)
        {
            if (_textBoxPool.Count == 0)
            {
                //create a new one                
                //this version support only editable, single line text
                TextBox textbox = new TextBox(w, h, false, true);
                return textbox;
            }
            else
            {
                TextBox box = _textBoxPool.Pop();
                box.SetSize(w, h);
                return box;
            }
        }


        public void ReleaseTextBox(TextBox textbox)
        {
            textbox.TextEventListener = null;//release 
            _textBoxPool.Push(textbox);
        }
        public void ReleaseMaskTextBox(MaskTextBox maskTextbox)
        {
            maskTextbox.TextEventListener = null;//release 
            _maskTextBoxPool.Push(maskTextbox);
        }

        internal TextBoxContainer UsedBy
        {
            get => _usedBy;
            set
            {
                _usedBy = value;
            }
        }
    }
}