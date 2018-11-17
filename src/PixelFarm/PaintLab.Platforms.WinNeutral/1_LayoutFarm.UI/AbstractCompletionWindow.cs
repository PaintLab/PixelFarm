//MIT
//Mike Kruger, ICSharpCode, 
using PixelFarm.Forms;
namespace LayoutFarm.UI
{
    partial class AbstractCompletionWindow : Form
    {
        Form _linkedParentForm;
        Control _linkedParentControl;
        public AbstractCompletionWindow()
        {
            //TODO: review here 
            //InitializeComponent();
            //this.ShowInTaskbar = false;
            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.None;
        }
        public Form LinkedParentForm
        {
            get { return this._linkedParentForm; }
            set { this._linkedParentForm = value; }
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
        //        createParams.ClassStyle |= 0x00020000;//add window shadow
        //        return createParams;
        //    }
        //}
        //protected override bool ShowWithoutActivation
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        public void ShowForm()
        {
            this.Show();
            this._linkedParentControl.Focus();
        }
    }
}
