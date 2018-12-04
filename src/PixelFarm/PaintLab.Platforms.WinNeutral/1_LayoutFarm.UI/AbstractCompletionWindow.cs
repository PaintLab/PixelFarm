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
            get { return _linkedParentForm; }
            set { _linkedParentForm = value; }
        }
        public Control LinkedParentControl
        {
            get { return _linkedParentControl; }
            set
            {
                _linkedParentControl = value;
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
            _linkedParentControl.Focus();
        }
    }
}
