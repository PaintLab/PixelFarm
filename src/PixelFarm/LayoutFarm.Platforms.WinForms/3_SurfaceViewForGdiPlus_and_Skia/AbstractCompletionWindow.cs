//MIT
//Mike Kruger, ICSharpCode,


using System;
using System.Windows.Forms;
namespace LayoutFarm.UI
{
    partial class AbstractCompletionWindow : Form
    {
        Form linkedParentForm;
        Control linkedParentControl;
        FormPopupShadow2 formPopupShadow;
        public AbstractCompletionWindow()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
        }
        internal FormPopupShadow2 PopupShadow
        {
            get { return formPopupShadow; }
            set
            {
                formPopupShadow = value;
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
                formPopupShadow.Visible = false;
                _showingPopup = false;
            }
        }

        public Form LinkedParentForm
        {
            get { return this.linkedParentForm; }
            set { this.linkedParentForm = value; }
        }
        public Control LinkedParentControl
        {
            get { return this.linkedParentControl; }
            set
            {
                this.linkedParentControl = value;
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
            if (this.formPopupShadow != null)
            {
                _showingPopup = true;
                formPopupShadow.Show2(this);
            }

            //this.Show();
            this.linkedParentControl.Focus();
        }
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            if (_showingPopup)
            {
                formPopupShadow.MoveRelativeTo(this);
            }
        }
    }
}
