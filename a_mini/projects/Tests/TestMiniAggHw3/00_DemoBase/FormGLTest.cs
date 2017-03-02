//MIT, 2014-2017, WinterDev

using System.Windows.Forms;
using OpenTK;

namespace Mini
{
    partial class FormGLTest : Form
    {

        MyGLControl miniGLControl;
        public FormGLTest()
        {
            InitializeComponent();
        }

        public MyGLControl InitMiniGLControl(int w, int h)
        {
            if (miniGLControl == null)
            {
                miniGLControl = new MyGLControl();
                miniGLControl.Width = w;
                miniGLControl.Height = h;
                this.Controls.Add(miniGLControl);
            }
            return miniGLControl;
        } 
    }
}
