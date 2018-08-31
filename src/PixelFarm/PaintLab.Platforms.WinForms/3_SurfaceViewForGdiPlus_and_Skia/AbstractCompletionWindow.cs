//MIT
//Mike Kruger, ICSharpCode,


using System;
using System.Windows.Forms;
namespace LayoutFarm.UI
{
    partial class AbstractCompletionWindow : Form
    {
        Form _linkedParentForm;
        Control _linkedParentControl;
        FormPopupShadow2 _formPopupShadow;

        FormClosingEventHandler _parentFormClosingEventHandler;
        public AbstractCompletionWindow()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;

            _parentFormClosingEventHandler = (s, e) =>
            {
                if (_formPopupShadow != null)
                {
                    _formPopupShadow.Close();
                    _formPopupShadow = null;
                }
                //
                this.Close();
            };

        }
        internal FormPopupShadow2 PopupShadow
        {
            get { return _formPopupShadow; }
            set
            {
                _formPopupShadow = value;
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.Visible)
            {
                //formPopupShadow.Show2(this);
            }
            else
            {
                _formPopupShadow.Visible = false;
                _showingPopup = false;
            }
        }

        public Form LinkedParentForm
        {
            get { return this._linkedParentForm; }
            set
            {
                if (_linkedParentForm != null && _linkedParentForm != value)
                {
                    //unsubscribe old event
                    _linkedParentForm.FormClosing -= _parentFormClosingEventHandler;
                }

                this._linkedParentForm = value;
                if (value != null)
                {
                    //when
                    _linkedParentForm.FormClosing += _parentFormClosingEventHandler;
                }
            }
        }

        public Control LinkedParentControl
        {
            get { return this._linkedParentControl; }
            set
            {
                this._linkedParentControl = value;
            }
        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var createParams = base.CreateParams;
        //        //createParams.ClassStyle |= 0x00020000;//add window shadow
        //        return createParams;
        //    }
        //}
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        bool _showingPopup = false;
        public void ShowForm()
        {


            // Show the window without activating it (i.e. do not take focus)
            PI.ShowWindow(this.Handle, (short)PI.SW_SHOWNOACTIVATE);
            if (this._formPopupShadow != null)
            {
                _showingPopup = true;
                _formPopupShadow.Show2(this);
            }

            //this.Show();
            this._linkedParentControl.Focus();
        }
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            if (_showingPopup)
            {
                _formPopupShadow.MoveRelativeTo(this);
            }
        }
    }
}
