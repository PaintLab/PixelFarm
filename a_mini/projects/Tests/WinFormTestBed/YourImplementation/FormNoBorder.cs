using System;
using System.Windows.Forms;

namespace LayoutFarm.Dev
{
    //sample

    public partial class FormNoBorder : Form
    {
        public FormNoBorder()
        {
            InitializeComponent();
        }
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        ///
        /// Handling the window messages
        ///
        protected override void WndProc(ref Message message)
        {
            //from https://stackoverflow.com/questions/7482922/remove-the-title-bar-in-windows-forms
            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }
    }
}
